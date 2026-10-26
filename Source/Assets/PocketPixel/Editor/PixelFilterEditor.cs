using UnityEditor;
using UnityEngine;

namespace PocketPixel.Editor
{
    [CustomEditor(typeof(PixelFilter)), CanEditMultipleObjects]
    public sealed class PixelFilterEditor : UnityEditor.Editor
    {
        void Field(string name, string label) => EditorGUILayout.PropertyField(serializedObject.FindProperty(name), new GUIContent(label));
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.HelpBox("Globaler Szenenfilter: Objekt oder Komponente deaktivieren = Filter aus. Die Position im Raum spielt keine Rolle. Farben werden auf diesem Objekt gespeichert.", MessageType.Info);
            Field("priority","Priorität"); Field("showInSceneView","Auch in Scene-Ansicht");
            EditorGUILayout.Space();
            var palette = serializedObject.FindProperty("palette");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(palette,new GUIContent("Farbvorlage"));
            bool changed = EditorGUI.EndChangeCheck();
            serializedObject.ApplyModifiedProperties();
            bool applyPalette = GUILayout.Button("Farbvorlage erneut anwenden");
            if (changed || applyPalette)
            {
                foreach (PixelFilter filter in targets)
                {
                    Undo.RecordObject(filter,"Apply pixel palette"); filter.ApplyPalette(filter.palette); EditorUtility.SetDirty(filter);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(filter);
                }
                serializedObject.Update();
                SceneView.RepaintAll();
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
            Field("shadow","1 – Schatten"); Field("midtone","2 – Mittelton"); Field("lightTone","3 – Hell"); Field("highlight","4 – Glanzlicht");
            EditorGUILayout.Space();
            Field("pixelHeight","Vertikale Pixelanzahl"); Field("exposure","Belichtung"); Field("contrast","Kontrast"); Field("dithering","Dithering");
            Field("outlines","Umrandung aktiviert");
            EditorGUILayout.HelpBox("Konturen werden automatisch angepasst: Universal Renderer = Tiefe/Normalen; 2D Renderer = sichtbare Farbkontraste (auch innerhalb von Sprites).", MessageType.Info);
            var width=serializedObject.FindProperty("outlineWidth");
            EditorGUI.showMixedValue=width.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            float value=EditorGUILayout.FloatField("Linienstärke (Filterpixel)",width.floatValue);
            if(EditorGUI.EndChangeCheck()) width.floatValue=Mathf.Clamp(value,0,4);
            EditorGUI.showMixedValue=false;
            Field("outlineColor","Linienfarbe");
            if (serializedObject.ApplyModifiedProperties()) { SceneView.RepaintAll(); UnityEditorInternal.InternalEditorUtility.RepaintAllViews(); }
            if (GUILayout.Button("Alle Filterwerte zurücksetzen"))
            {
                foreach(PixelFilter filter in targets) { Undo.RecordObject(filter,"Reset pixel filter"); filter.ResetSettings(); EditorUtility.SetDirty(filter); PrefabUtility.RecordPrefabInstancePropertyModifications(filter); }
                SceneView.RepaintAll();
            }
            if (GUILayout.Button("URP-Einbindung prüfen / reparieren")) PixelFilterSetup.Install(true);
        }
    }
}
