using UnityEditor;
using UnityEngine;

namespace PocketPixel.Editor
{
    [CustomEditor(typeof(PixelFilter)), CanEditMultipleObjects]
    public sealed class PixelFilterEditor : UnityEditor.Editor
    {
        static string T(string english, string german) => PixelFilterLocalization.Text(english, german);
        void Field(string name, string english, string german, string englishTip = "", string germanTip = "") =>
            EditorGUILayout.PropertyField(serializedObject.FindProperty(name), new GUIContent(T(english, german), T(englishTip, germanTip)));

        void LanguageSelector()
        {
            var previous = PixelFilterLocalization.Language;
            var selected = (PixelFilterEditorLanguage)EditorGUILayout.Popup(
                new GUIContent("Language / Sprache", T(
                    "PocketPixel UI only. Automatic follows the Unity Editor language when available; otherwise English. The setting is stored per user.",
                    "Nur die PocketPixel-Oberfläche. Automatisch folgt der Unity-Editorsprache, falls verfügbar; sonst Englisch. Die Auswahl wird pro Benutzer gespeichert.")),
                (int)previous, new[] { new GUIContent("Automatic · Automatisch"), new GUIContent("English"), new GUIContent("Deutsch") });
            if (selected != previous)
            {
                PixelFilterLocalization.Language = selected;
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            LanguageSelector();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(T(
                "Global scene filter: disable the object or component to turn it off. Position does not matter. Colours are stored on this object.",
                "Globaler Szenenfilter: Objekt oder Komponente deaktivieren = Filter aus. Die Position im Raum spielt keine Rolle. Farben werden auf diesem Objekt gespeichert."), MessageType.Info);
            Field("priority", "Priority", "Priorität", "The highest priority wins when multiple filters are active.", "Bei mehreren aktiven Filtern gewinnt die höchste Priorität.");
            Field("showInSceneView", "Show in Scene view", "Auch in Scene-Ansicht");
            EditorGUILayout.Space();
            var palette = serializedObject.FindProperty("palette");
            EditorGUI.showMixedValue = palette.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            int selectedPalette = EditorGUILayout.Popup(T("Palette", "Farbvorlage"), palette.enumValueIndex,
                PixelFilterLocalization.IsGerman ?
                new[] { "Warmgelb", "Grün", "Eisblau", "Violett", "Schwarzweiss" } :
                new[] { "Warm yellow", "Green", "Ice blue", "Violet", "Black and white" });
            bool changed = EditorGUI.EndChangeCheck();
            EditorGUI.showMixedValue = false;
            if (changed) palette.enumValueIndex = selectedPalette;
            serializedObject.ApplyModifiedProperties();
            bool applyPalette = GUILayout.Button(T("Reapply palette", "Farbvorlage erneut anwenden"));
            if (changed || applyPalette)
            {
                foreach (PixelFilter filter in targets)
                {
                    Undo.RecordObject(filter, T("Apply pixel palette", "Farbvorlage anwenden"));
                    filter.ApplyPalette(filter.palette);
                    EditorUtility.SetDirty(filter);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(filter);
                }
                serializedObject.Update();
                SceneView.RepaintAll();
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
            Field("shadow", "1 – Shadow", "1 – Schatten");
            Field("midtone", "2 – Midtone", "2 – Mittelton");
            Field("lightTone", "3 – Light", "3 – Hell");
            Field("highlight", "4 – Highlight", "4 – Glanzlicht");
            EditorGUILayout.Space();
            Field("pixelHeight", "Vertical pixel count", "Vertikale Pixelanzahl");
            Field("exposure", "Exposure", "Belichtung");
            Field("contrast", "Contrast", "Kontrast");
            Field("dithering", "Dithering", "Dithering");
            Field("outlines", "Enable outlines", "Umrandung aktiviert");
            var width = serializedObject.FindProperty("outlineWidth");
            EditorGUI.showMixedValue = width.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            float value = EditorGUILayout.FloatField(T("Outline width (filter pixels)", "Linienstärke (Filterpixel)"), width.floatValue);
            if (EditorGUI.EndChangeCheck()) width.floatValue = Mathf.Clamp(value, 0, 4);
            EditorGUI.showMixedValue = false;
            Field("outlineColor", "Outline colour", "Linienfarbe");
            if (serializedObject.ApplyModifiedProperties())
            {
                SceneView.RepaintAll();
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
            if (GUILayout.Button(T("Reset all filter settings", "Alle Filterwerte zurücksetzen")))
            {
                foreach (PixelFilter filter in targets)
                {
                    Undo.RecordObject(filter, T("Reset pixel filter", "Pixel-Filter zurücksetzen"));
                    filter.ResetSettings();
                    EditorUtility.SetDirty(filter);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(filter);
                }
                SceneView.RepaintAll();
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
            if (GUILayout.Button(T("Check / repair URP integration", "URP-Einbindung prüfen / reparieren")))
                PixelFilterSetup.Install(true);
        }
    }
}
