// Standalone adapter tests with Unity API doubles. Not a Unity Editor test.
using System;
using System.Collections.Generic;
using PocketPixel.Editor;

namespace UnityEngine
{
    public enum SystemLanguage { English, German, French }
    public static class Application
    {
        // The detector must never use the OS language for an Editor UI choice.
        public static SystemLanguage systemLanguage { get { throw new Exception("OS language was read"); } }
    }
}

namespace UnityEditor
{
    public static class EditorPrefs
    {
        static readonly Dictionary<string, int> Values = new Dictionary<string, int>();
        public static int GetInt(string key, int fallback) => Values.ContainsKey(key) ? Values[key] : fallback;
        public static void SetInt(string key, int value) { Values[key] = value; }
    }
#if !API_MISSING
    internal static class LocalizationDatabase
    {
        public static bool Enabled;
        public static bool ThrowOnRead;
        public static UnityEngine.SystemLanguage EditorLanguage;
        public static bool enableEditorLocalization
        {
            get { if (ThrowOnRead) throw new Exception("Editor language unavailable"); return Enabled; }
        }
#if API_WRONG_TYPE
        public static string currentEditorLanguage => "German";
#else
        public static UnityEngine.SystemLanguage currentEditorLanguage => EditorLanguage;
#endif
    }
#endif
}

internal static class EditorLanguageTests
{
    static int assertions;
    static void Expect(bool condition, string name)
    {
        assertions++;
        if (!condition) throw new Exception(name);
    }

    public static void Main()
    {
        Expect(PixelFilterLocalization.Language == PixelFilterEditorLanguage.Automatic, "Default is Automatic");
        Expect(!PixelFilterLocalization.IsGerman, "Unlocalized default is English");
        PixelFilterLocalization.Language = PixelFilterEditorLanguage.German;
        Expect(PixelFilterLocalization.IsGerman, "Manual German");
        Expect(PixelFilterLocalization.Text("Outline", "Umrandung") == "Umrandung", "German text");
        Expect(PixelFilterLocalization.Language == PixelFilterEditorLanguage.German, "Per-user preference retained");
        PixelFilterLocalization.Language = PixelFilterEditorLanguage.English;
        Expect(!PixelFilterLocalization.IsGerman, "Manual English");
        Expect(PixelFilterLocalization.Text("Outline", "Umrandung") == "Outline", "English text");
        UnityEditor.EditorPrefs.SetInt("PocketPixel.Editor.Language", 99);
        Expect(PixelFilterLocalization.Language == PixelFilterEditorLanguage.Automatic, "Invalid stored preference");
        PixelFilterLocalization.Language = (PixelFilterEditorLanguage)(-3);
        Expect(PixelFilterLocalization.Language == PixelFilterEditorLanguage.Automatic, "Invalid selected mode");

#if !API_MISSING
        UnityEditor.LocalizationDatabase.Enabled = true;
        UnityEditor.LocalizationDatabase.EditorLanguage = UnityEngine.SystemLanguage.German;
#if !API_WRONG_TYPE
        Expect(PixelFilterLocalization.IsGerman, "Automatic German Editor");
        UnityEditor.LocalizationDatabase.EditorLanguage = UnityEngine.SystemLanguage.English;
        Expect(!PixelFilterLocalization.IsGerman, "Editor language changes to English");
        UnityEditor.LocalizationDatabase.EditorLanguage = UnityEngine.SystemLanguage.French;
        Expect(!PixelFilterLocalization.IsGerman, "Untranslated Editor language uses English");
        UnityEditor.LocalizationDatabase.EditorLanguage = UnityEngine.SystemLanguage.German;
        UnityEditor.LocalizationDatabase.Enabled = false;
        Expect(!PixelFilterLocalization.IsGerman, "Disabled editor localization uses English");
        UnityEditor.LocalizationDatabase.Enabled = true;
#else
        Expect(!PixelFilterLocalization.IsGerman, "Changed Unity property type uses English");
#endif
        UnityEditor.LocalizationDatabase.ThrowOnRead = true;
        Expect(!PixelFilterLocalization.IsGerman, "Failing Unity getter uses English");
        PixelFilterLocalization.Language = PixelFilterEditorLanguage.German;
        Expect(PixelFilterLocalization.IsGerman, "Manual German bypasses failing detector");
        PixelFilterLocalization.Language = PixelFilterEditorLanguage.English;
        Expect(!PixelFilterLocalization.IsGerman, "Manual English bypasses failing detector");
#else
        Expect(!PixelFilterLocalization.IsGerman, "Missing Unity localization API uses English");
#endif
        Console.WriteLine("PASS: " + assertions + " editor-language assertions.");
    }
}
