using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PocketPixel.Editor
{
    internal enum PixelFilterEditorLanguage { Automatic, English, German }

    internal static class PixelFilterLocalization
    {
        const string PreferenceKey = "PocketPixel.Editor.Language";
        const BindingFlags StaticProperties = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        static readonly Type LocalizationType = FindLocalizationType();
        static readonly PropertyInfo LanguageProperty = LocalizationType?.GetProperty("currentEditorLanguage", StaticProperties);
        static readonly PropertyInfo EnabledProperty = LocalizationType?.GetProperty("enableEditorLocalization", StaticProperties);

        public static PixelFilterEditorLanguage Language
        {
            get
            {
                int value = EditorPrefs.GetInt(PreferenceKey, 0);
                return value >= 0 && value <= 2 ? (PixelFilterEditorLanguage)value : PixelFilterEditorLanguage.Automatic;
            }
            set
            {
                int selected = (int)value;
                EditorPrefs.SetInt(PreferenceKey, selected >= 0 && selected <= 2 ? selected : 0);
            }
        }

        public static bool IsGerman
        {
            get
            {
                var mode = Language;
                if (mode != PixelFilterEditorLanguage.Automatic)
                    return mode == PixelFilterEditorLanguage.German;
                // Unity has no public editor-language getter. Keep this read-only
                // compatibility adapter isolated; never infer it from the OS language.
                // Missing/changed APIs and disabled localization safely mean English.
                try
                {
                    return EnabledProperty?.PropertyType == typeof(bool) &&
                        LanguageProperty?.PropertyType == typeof(SystemLanguage) &&
                        (bool)EnabledProperty.GetValue(null, null) &&
                        (SystemLanguage)LanguageProperty.GetValue(null, null) == SystemLanguage.German;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static string Text(string english, string german) => IsGerman ? german : english;

        static Type FindLocalizationType()
        {
            // The localization module can be a separate Editor assembly.
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType("UnityEditor.LocalizationDatabase", false);
                if (type != null) return type;
            }
            return null;
        }
    }
}
