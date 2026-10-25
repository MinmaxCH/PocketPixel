using System.Collections.Generic;
using UnityEngine;

namespace PocketPixel
{
    public enum Palette { Warmgelb, Gruen, Eisblau, Violett, Schwarzweiss }

    [ExecuteAlways, DisallowMultipleComponent, AddComponentMenu("Rendering/Pixel Filter")]
    public sealed class PixelFilter : MonoBehaviour
    {
        static readonly List<PixelFilter> active = new List<PixelFilter>();
        [Tooltip("Bei mehreren aktiven Filtern gewinnt die höchste Priorität.")]
        public int priority;
        public bool showInSceneView = true;
        public Palette palette = Palette.Warmgelb;
        [Range(90,720)] public float pixelHeight = 270;
        [Range(0.1f,4)] public float exposure = 1;
        [Range(0.2f,3)] public float contrast = 1.2f;
        [Range(0,1)] public float dithering = 0.12f;
        public bool outlines = true;
        [Range(0,4)] public float outlineWidth = 0.5f;
        public Color shadow = new Color(0.247f,0.165f,0.216f,1);
        public Color midtone = new Color(0.647f,0.369f,0.282f,1);
        public Color lightTone = new Color(0.867f,0.635f,0.310f,1);
        public Color highlight = new Color(1,0.898f,0.745f,1);
        public Color outlineColor = new Color(0.12f,0.07f,0.10f,1);

        void OnEnable() { if (!active.Contains(this)) active.Add(this); }
        void OnDisable() { active.Remove(this); }
        void OnDestroy() { active.Remove(this); }

        public static PixelFilter Current
        {
            get
            {
                PixelFilter best = null;
                for (int i = active.Count - 1; i >= 0; i--)
                {
                    var item = active[i];
                    if (item == null) { active.RemoveAt(i); continue; }
                    if (!item.isActiveAndEnabled || !item.gameObject.scene.IsValid() || !item.gameObject.scene.isLoaded) continue;
#if UNITY_EDITOR
                    if (UnityEditor.SceneManagement.EditorSceneManager.IsPreviewScene(item.gameObject.scene)) continue;
#endif
                    if (best == null || item.priority > best.priority) best = item;
                }
                return best;
            }
        }

        static Color Hex(string value) { ColorUtility.TryParseHtmlString("#" + value, out var c); return c; }
        public void ApplyPalette(Palette value)
        {
            palette = value;
            switch (value)
            {
                case Palette.Gruen: shadow=Hex("0F281F"); midtone=Hex("306B45"); lightTone=Hex("88B850"); highlight=Hex("E0F8B0"); break;
                case Palette.Eisblau: shadow=Hex("19233D"); midtone=Hex("365B87"); lightTone=Hex("70B6CA"); highlight=Hex("DCF4ED"); break;
                case Palette.Violett: shadow=Hex("25162F"); midtone=Hex("71416E"); lightTone=Hex("BD80AC"); highlight=Hex("FFE0DC"); break;
                case Palette.Schwarzweiss: shadow=Hex("171717"); midtone=Hex("606060"); lightTone=Hex("AAAAAA"); highlight=Hex("F5F5F5"); break;
                default: shadow=new Color(0.247f,0.165f,0.216f,1); midtone=new Color(0.647f,0.369f,0.282f,1); lightTone=new Color(0.867f,0.635f,0.310f,1); highlight=new Color(1,0.898f,0.745f,1); break;
            }
            outlineColor = new Color(shadow.r * 0.5f,shadow.g * 0.5f,shadow.b * 0.5f,1);
        }

        public void ResetSettings()
        {
            pixelHeight=270; exposure=1; contrast=1.2f; dithering=0.12f; outlines=true; outlineWidth=0.5f;
            ApplyPalette(Palette.Warmgelb);
        }

        public void WriteMaterial(Material material)
        {
            material.SetFloat("_EffectEnabled",1);
            material.SetFloat("_PixelHeight",pixelHeight);
            material.SetFloat("_Exposure",exposure);
            material.SetFloat("_Contrast",contrast);
            material.SetFloat("_Dither",dithering);
            material.SetFloat("_OutlineEnabled",outlines?1:0);
            material.SetFloat("_OutlineWidth",outlineWidth);
            material.SetColor("_Dark",shadow); material.SetColor("_Mid",midtone);
            material.SetColor("_Light",lightTone); material.SetColor("_Bright",highlight);
            material.SetColor("_OutlineColor",outlineColor);
        }
    }
}
