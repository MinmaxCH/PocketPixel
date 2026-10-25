using UnityEditor;
using UnityEngine;

namespace PocketPixel.Editor
{
    // Editor-only selectable marker. No renderer or collider is added to the game.
    public static class PixelFilterGizmo
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
        static void Draw(PixelFilter filter, GizmoType type)
        {
            var position = filter.transform.position;
            var view = SceneView.currentDrawingSceneView;
            var rotation = view != null ? view.camera.transform.rotation : Quaternion.identity;
            float size = HandleUtility.GetHandleSize(position) * 0.18f;
            var matrix = Gizmos.matrix;
            var color = Gizmos.color;
            Gizmos.matrix = Matrix4x4.TRS(position,rotation,Vector3.one * size);
            var colors = new[] { filter.shadow, filter.midtone, filter.lightTone, filter.highlight };
            for(int i=0;i<4;i++)
            {
                Gizmos.color = colors[i];
                Gizmos.DrawCube(new Vector3((i%2-0.5f)*0.52f,(0.5f-i/2)*0.52f,0),new Vector3(0.48f,0.48f,0.06f));
            }
            Gizmos.color = (type & GizmoType.Selected) != 0 ? Color.white : new Color(1,0.75f,0.2f);
            Gizmos.DrawWireCube(Vector3.zero,new Vector3(1.12f,1.12f,0.08f));
            Gizmos.matrix = matrix;
            Gizmos.color = color;
            Handles.Label(position + rotation * Vector3.down * size * 0.75f,"Pixel Filter");
        }
    }
}
