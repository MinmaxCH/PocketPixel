# PocketPixel 1.0.2-preview

[English](README.md) | **Deutsch**

Globaler Vierfarben-Pixelfilter für Unity 6 / Universal Render Pipeline.
Version 1.0.1 wurde mit Unity 6000.4.0f1 und URP 17.4.0 im Universal Renderer geprüft.
Diese Vorschau ergänzt den URP 2D Renderer. Der neue Code wurde noch nicht im Unity Editor oder in einem Player-Build getestet; siehe [2D-Testablauf](TEST-2D.md). Built-in und HDRP werden nicht unterstützt. Andere Versionen sind nicht geprüft.

## Vorschauen

| Warmgelb | Eisblau |
| --- | --- |
| ![PocketPixel – Warmgelb](Previews/01-Warmgelb.png) | ![PocketPixel – Eisblau](Previews/Eisblau.png) |
| Grün | Schwarzweiss |
| ![PocketPixel – Grün](Previews/Gruen.png) | ![PocketPixel – Schwarzweiss](Previews/Schwarzweiss.png) |
| Violett | |
| ![PocketPixel – Violett](Previews/Violett.png) | |

## Download

**[PocketPixel 1.0.2 Vorschau für Unity herunterladen (.unitypackage)](https://github.com/MinmaxCH/PocketPixel/raw/refs/heads/main/PocketPixel-1.0.2-preview.unitypackage)**

[Komplettpaket mit Quellcode, Vorschauen und Dokumentation herunterladen (.zip)](https://github.com/MinmaxCH/PocketPixel/raw/refs/heads/main/PocketPixel-1.0.2-preview-komplett.zip)

Das Repository ist privat; für den Download ist ein GitHub-Konto mit Zugriff erforderlich.

## Installation in ein neues Projekt

1. Universal-3D- oder Universal-2D-Projekt (URP) öffnen.
2. PocketPixel-1.0.2-preview.unitypackage über Assets > Import Package > Custom Package importieren.
3. Kompilierung abwarten. Die Renderer-Einbindung wird automatisch eingerichtet.
4. Assets/PocketPixel/Pixel Filter.prefab in die Hierarchy ziehen.
5. Objekt auswählen: alle Regler stehen direkt im Inspector.

Alternativ: GameObject > Effects > Pixel Filter erzeugt ein neues Filter-Objekt.
Bei einem späteren Wechsel der Render-Pipeline: Tools > Pixel Filter > Installieren oder reparieren.
Die Einrichtung verändert ausschließlich die Renderer der in Graphics/Quality Settings verwendeten URP-Assets.

## Verhalten wie ein globales Post-Process-Volume

- Kein aktives Filter-Objekt: kein Effekt.
- Objekt oder Pixel-Filter-Komponente deaktivieren: Effekt aus.
- Position, Rotation und Skalierung des Objekts sind egal; es wirkt global.
- Keine räumliche Box, Eintrittszone oder Überblendung an Volumengrenzen.
- Mehrere Objekte: höchste Priorität gewinnt. Gleiche Priorität vermeiden.
- Der Effekt gilt für die geladenen Szenen; Scene-Vorschau lässt sich separat ausschalten.
- Kamera-Stacks werden am Ende gefiltert; Screen-Space-Overlay-UI bleibt ungefiltert.
- Im Prefab-Modus sind Vorschauobjekte nicht als Szenenfilter aktiv.

## Vorlagen und eigene Farben

Verfügbare Farbvorlagen: Warmgelb, Grün, Eisblau, Violett und Schwarzweiss.
Das gelbe Standard-Prefab liegt direkt im PocketPixel-Ordner; weitere Vorlagen liegen unter Prefabs als eigene Prefabs. Alle Paletten sind auch im Inspector unter Farbvorlage auswählbar.
Eine Vorlage überschreibt nur die vier Farben und die Linienfarbe, nicht Pixelgröße/Belichtung.
Farbfelder können danach frei geändert werden. "Farbvorlage erneut anwenden" stellt die Vorlagenfarben wieder her.
"Alle Filterwerte zurücksetzen" stellt Warmgelb und die ursprünglichen Effektwerte wieder her.
Eigene Vorlage: ein eingestelltes Objekt aus der Hierarchy in deinen eigenen Project-Ordner ziehen.
Alle Werte sind pro Objekt gespeichert; ein Objekt verändert kein gemeinsam verwendetes Material.

## Regler

- Vertikale Pixelanzahl: kleinere Zahl = gröbere Blöcke. Tatsächliche Pixelgröße hängt von der Fensterauflösung ab.
- Belichtung/Kontrast: Verteilung auf vier Helligkeitsstufen.
- Dithering: Rasterübergänge zwischen Farben.
- Umrandung: Im Universal Renderer Tiefen-/Normalenkanten, im 2D Renderer sichtbare Farbkontraste im Kamerabild (auch innerhalb von Sprites). Kontrastarme Grenzen werden möglicherweise nicht umrandet.
- Linienstärke: Dezimalwerte 0–4 in Filterpixeln; beispielsweise 0.25. Unterhalb eines Bildschirmpixels wird die Linie abgeschwächt.
- Im Universal Renderer bekommen transparente Materialien ohne Tiefen-/Normalenpass keine zuverlässigen Umrandungen. Der 2D-Farbmodus benötigt diese Pässe nicht.

## Technik und Grenzen

Die Installation ergänzt ein eigenes URP Renderer Feature. Wenn kein Filter-Objekt aktiv ist, wird dessen Filterpass nicht eingeplant.
Das Material wird bei Bedarf intern erzeugt, wiederverwendet und beim Aufräumen des Features freigegeben.
Shaderreferenzen werden im Renderer gespeichert, damit der Shader im Build enthalten bleibt.
Der Filter verpixelt das gerenderte Bild und reduziert nicht die interne 3D-Renderauflösung.
URP kann zusätzliche Farbkopien oder Tiefen-/Normalenpasses benötigen; ein Filterpass bedeutet daher nicht nur einen GPU-Arbeitsschritt. Er ist für SDR-Desktop vorgesehen;
XR, HDR-Ausgabe und dynamische Auflösung sind nicht geprüft. Nachgeschaltetes Antialiasing oder Upscaling kann Pixel glätten.

## Weitergeben

Nur den Ordner Assets/PocketPixel über Assets > Export Package exportieren, keine Projektszenen,
Pipeline-Assets, projektfremde Spielobjekte oder Library-Ordner mitliefern. Das beiliegende unitypackage ist bereits so zusammengestellt.
Vor einer öffentlichen Store-Veröffentlichung sind zusätzliche Plattformtests und Produktdokumentation erforderlich.

## Auswahl in der Scene-Ansicht
Das vierfarbige Pixel-Symbol ist anklickbar und bleibt im Spiel unsichtbar. Scene-Gizmos einschalten. Es markiert nur das Filterobjekt, keine räumliche Effektgrenze.


## Lizenz und Quellcode
Privat und kommerziell nutzbar, veränderbar und weitergebbar. Veröffentlichte Produkte müssen PocketPixel - minmax.ch in den Credits nennen. Vollständige Bedingungen: LICENSE.txt (eigene Lizenz, nicht unverändertes MIT).
Alle C#- und Shaderquellen liegen bei. [TECHNICAL.md](TECHNICAL.md) beschreibt Implementierung und Wartung; [TROUBLESHOOTING.md](TROUBLESHOOTING.md) die Fehlerdiagnose.

