Shader "PocketPixel/Four Colour Camera"
{
    Properties
    {
        [ToggleUI] _EffectEnabled ("Filter aktiviert", Float) = 1
        _PixelHeight ("Vertical pixel count", Range(90,720)) = 270
        _Exposure ("Exposure", Range(0.1,4)) = 1.0
        _Contrast ("Contrast", Range(0.2,3)) = 1.2
        _Dither ("Dithering", Range(0,1)) = 0.12
        [ToggleUI] _OutlineEnabled ("Umrandung aktiviert", Float) = 1
        _OutlineWidth ("Linienstaerke in Filterpixeln", Float) = 0.5
        _OutlineColor ("Linienfarbe", Color) = (0.12,0.07,0.10,1)
        _Dark ("1 - Shadow", Color) = (0.247,0.165,0.216,1)
        _Mid ("2 - Rust", Color) = (0.647,0.369,0.282,1)
        _Light ("3 - Gold", Color) = (0.867,0.635,0.310,1)
        _Bright ("4 - Cream", Color) = (1,0.898,0.745,1)
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Name "WarmPixel"
            ZWrite Off ZTest Always Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
            CBUFFER_START(UnityPerMaterial)
                float _EffectEnabled;
                float _OutlineEnabled, _OutlineWidth;
                float4 _OutlineColor;
                float _PixelHeight, _Exposure, _Contrast, _Dither;
                float4 _Dark, _Mid, _Light, _Bright;
            CBUFFER_END
            float EyeDepth(float2 uv)
            {
                float raw = SampleSceneDepth(uv);
                if (unity_OrthoParams.w > 0.5)
                {
                    #if UNITY_REVERSED_Z
                        raw = 1.0 - raw;
                    #endif
                    return lerp(_ProjectionParams.y, _ProjectionParams.z, raw);
                }
                return LinearEyeDepth(raw, _ZBufferParams);
            }
            float3 SurfacePosition(float2 uv)
            {
                float rawDepth = SampleSceneDepth(uv);
                #if !UNITY_REVERSED_Z
                    rawDepth = lerp(UNITY_NEAR_CLIP_VALUE, 1.0, rawDepth);
                #endif
                return ComputeWorldSpacePosition(uv, rawDepth, UNITY_MATRIX_I_VP);
            }
            float OutlineEdge(float2 uv, float2 offset, float depth, float3 normal)
            {
                float2 neighbourUV = saturate(uv + offset);
                float neighbourDepth = EyeDepth(neighbourUV);
                float depthEdge = step(max(0.08, min(depth, neighbourDepth) * 0.025), abs(depth - neighbourDepth));
                float3 neighbourNormal = SampleSceneNormals(neighbourUV);
                float bothSurfaces = step(depth, _ProjectionParams.z * 0.99) * step(neighbourDepth, _ProjectionParams.z * 0.99);
                if (bothSurfaces > 0.5)
                {
                    // Compare distance from the surface plane, not raw depth slope.
                    // A receding flat floor must not turn into a wide black band.
                    float3 separation = SurfacePosition(neighbourUV) - SurfacePosition(uv);
                    float planeDistance = max(abs(dot(separation, normal)), abs(dot(separation, neighbourNormal)));
                    depthEdge *= step(max(0.05, min(depth, neighbourDepth) * 0.01), planeDistance);
                }
                float normalEdge = step(0.25, 1.0 - dot(normal, neighbourNormal));
                // The sky has no surface normal and must not become a solid outline.
                normalEdge *= bothSurfaces;
                return max(depthEdge, normalEdge);
            }
            float OutlineMask(float2 uv, float2 texel, float radius, float depth, float3 normal)
            {
                if (radius <= 0) return 0;
                float2 offset = texel * radius;
                float edge = max(OutlineEdge(uv, float2(offset.x, 0), depth, normal), OutlineEdge(uv, float2(-offset.x, 0), depth, normal));
                edge = max(edge, OutlineEdge(uv, float2(0, offset.y), depth, normal));
                return max(edge, OutlineEdge(uv, float2(0, -offset.y), depth, normal));
            }
            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                if (_EffectEnabled < 0.5)
                    return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, input.texcoord);
                // Integer output blocks keep pixels square, including when resizing the Game view.
                float2 size = _BlitTexture_TexelSize.zw;
                float block = max(1.0, floor(size.y / max(1.0, _PixelHeight) + 0.5));
                float2 cell = floor(input.texcoord * size / block);
                float2 uv = min((cell + 0.5) * block, size - 0.5) / size;
                float3 source = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uv).rgb;
                #ifndef UNITY_COLORSPACE_GAMMA
                    source = LinearToSRGB(source);
                #endif
                float value = dot(source, float3(0.2126, 0.7152, 0.0722));
                value = saturate((value * _Exposure - 0.5) * _Contrast + 0.5);
                const float bayer[16] = {0,8,2,10,12,4,14,6,3,11,1,9,15,7,13,5};
                uint2 p = uint2(cell) & 3;
                value += ((bayer[p.y * 4 + p.x] + 0.5) / 16.0 - 0.5) * _Dither / 3.0;
                float4 colour = value < 0.25 ? _Dark : value < 0.5 ? _Mid : value < 0.75 ? _Light : _Bright;
                if (_OutlineEnabled > 0.5 && _OutlineWidth > 0)
                {
                    // Evaluate outlines at screen resolution so fractions of a filter pixel remain thin.
                    float2 edgeUV = input.texcoord;
                    float depth = EyeDepth(edgeUV);
                    float3 normal = SampleSceneNormals(edgeUV);
                    float radius = block * clamp(_OutlineWidth, 0, 4);
                    float lower = floor(radius);
                    float edge = OutlineMask(edgeUV, 1.0 / size, lower, depth, normal);
                    if (frac(radius) > 0.001)
                        edge = lerp(edge, OutlineMask(edgeUV, 1.0 / size, lower + 1, depth, normal), frac(radius));
                    colour.rgb = lerp(colour.rgb, _OutlineColor.rgb, edge);
                }
                return float4(colour.rgb, 1);
            }
            ENDHLSL
        }
    }

}

