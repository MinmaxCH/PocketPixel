# PocketPixel 1.0.2-preview: Unity-Abnahme

Status: Noch nicht in Unity kompiliert oder gerendert. Die mitgelieferten
TEST-ENVIRONMENT.txt / verification.json dokumentieren ausschliesslich 1.0.1 in 3D.
Dieser Ablauf ist eine Prüfliste, kein erfolgreicher Testbericht.

## Installation und Kamera

1. Universal-2D-Projekt mit Unity 6000.4.0f1 / URP 17.4.0 öffnen.
2. PocketPixel-1.0.2-preview.unitypackage vollständig importieren (bestehende Dateien aktualisieren).
3. Kompilierung abwarten. Den ersten Console-Fehler festhalten, falls einer auftritt.
4. Tools > Pixel Filter > Installieren oder reparieren ausführen, danach nochmals.
   Auf jedem in Graphics/Quality verwendeten 2D Renderer muss genau ein
   aktives Pixel Filter Feature vorhanden sein: Shader zugewiesen, Use Color Outlines an.
5. Pixel Filter.prefab in die Szene ziehen. Game View und Play Mode prüfen.
6. Kamera-Postprocessing aus- und einschalten. Der Filter muss in beiden Fällen funktionieren.
7. Diagnosebericht speichern. Für die Spielkamera muss Renderer2D und für das
   zugehörige Feature outlineSource=2D colour erscheinen.

## Bild und Bedienung

- Sprite-Lit mit Global Light 2D, Sprite-Unlit und eine Tilemap prüfen.
- Mindestens ein Sprite mit transparentem Rand vor einem andersfarbigen Hintergrund verwenden.
- Alle fünf Paletten prüfen; Belichtung, Kontrast, Dithering und Pixelanzahl ändern.
- Konturen aus/ein; Breite 0, 0.25, 0.5 und 4 sowie eine auffällige Linienfarbe prüfen.
  2D-Konturen folgen sichtbaren Farbkontrasten, auch innerhalb von Sprites.
  Gleichfarbige oder sehr kontrastarme Grenzen werden nicht garantiert erkannt.
- Objekt deaktivieren, Komponente deaktivieren und Objekt löschen:
  Das ungefilterte Bild muss jeweils zurückkehren. Im Frame Debugger darf dann
  kein PocketPixel-Filterpass mehr eingeplant sein.
- Scene-Vorschau aus/ein, zwei Filter mit unterschiedlichen Prioritäten,
  Szene neu laden, Play Mode verlassen und erneut starten.
- Pixel Perfect Camera separat prüfen: aus/ein, mit und ohne Upscale Render Texture.
  Pixelgrösse bei verschiedenen Game-View-Auflösungen kontrollieren.
- Screen Space - Overlay Canvas muss ungefiltert bleiben.

## Gegenprobe und Build

- Dasselbe Paket in einem Universal-3D-Projekt importieren. Beim zugehörigen
  Feature muss Use Color Outlines aus sein. Vorhandene 3D-Konturen und alle
  Deaktivierungsprüfungen wiederholen.
- In einem Projekt mit Universal- und 2D-Renderer verschiedene Kameras/Qualitätsstufen
  verwenden. Die Konturenquelle darf nicht zwischen den Renderer-Features übersprechen.
- Windows-Player bauen und starten: Farben, Konturen, Deaktivierung und Szenenwechsel prüfen.
- Erst nach diesen Tests 2D-Unterstützung als bestätigt dokumentieren.
