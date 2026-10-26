# PocketPixel 1.0.1

[English](README.md) | **Deutsch**

Globaler Vierfarben-Pixelfilter für Unity 6 URP 3D mit dem Universal Renderer.
Entwicklungsumgebung: Unity 6000.4.0f1, URP 17.4.0, Universal Renderer, Windows-/DX12-Editor.
Built-in, HDRP und URP 2D Renderer werden nicht unterstützt. Andere Versionen und eigenständige Player-Builds sind nicht verifiziert. Siehe [Prüfstand](VALIDATION.md).

## Installation

1. Dein **Universal-3D-Projekt (URP)** in Unity öffnen. Die Kamera muss einen **Universal Renderer** verwenden.
2. **PocketPixel-1.0.1.unitypackage** herunterladen und die Datei aus dem Datei-Explorer in Unitys **Project-Fenster**, in den **Assets-Bereich**, ziehen.
3. Im Fenster **Import Unity Package** alle enthaltenen Dateien ausgewählt lassen und auf **Import** klicken.
4. Warten, bis Unity den Import und die Kompilierung abgeschlossen hat. Das Renderer Feature wird automatisch eingerichtet.
5. Im Project-Fenster **Assets/PocketPixel** öffnen und **Pixel Filter.prefab** in die **Hierarchy** (oder Scene-Ansicht) ziehen.
6. Das neue Filterobjekt auswählen und im **Inspector** einstellen. Der Filter wirkt auf das gesamte Kamerabild.

**Nur der Paketimport aktiviert den Effekt noch nicht. Dafür muss ein Filterobjekt in der Szene vorhanden und aktiviert sein.**

Falls beim Ziehen der Datei kein Importfenster erscheint: **Assets > Import Package > Custom Package...** wählen, dieselbe `.unitypackage` öffnen und im Importfenster auf **Import** klicken.
Die komplette ZIP enthält zusätzlich Quellcode, Vorschauen und Dokumentation; für die Installation des Unity-Pakets wird sie nicht benötigt.

Alternativ erzeugt **GameObject > Effects > Pixel Filter** ein Filterobjekt und prüft die Renderer-Einbindung.
Falls kein Effekt erscheint, ausserhalb des Play Modes **Tools > Pixel Filter > Installieren oder reparieren** ausführen.
Die Einrichtung verändert nur Universal-Renderer-Assets, die von den URP-Assets in Graphics und Quality Settings verwendet werden.

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
- Umrandung: Tiefen- und Normalenkanten; wirkt unabhängig von der Oberflächenfarbe.
- Linienstärke: Dezimalwerte 0–4 in Filterpixeln; beispielsweise 0.25. Unterhalb eines Bildschirmpixels wird die Linie abgeschwächt.
- Transparente Materialien ohne Tiefen-/Normalenpass bekommen keine zuverlässigen Umrandungen.

## Technik und Grenzen

Die Installation ergänzt ein eigenes URP Renderer Feature. Wenn kein Filter-Objekt aktiv ist, wird dessen Filterpass nicht eingeplant.
Das Material wird bei Bedarf intern erzeugt, wiederverwendet und beim Aufräumen des Features freigegeben.
Shaderreferenzen werden im Renderer gespeichert, damit der Shader im Build enthalten bleibt.
Der Filter verpixelt das gerenderte Bild und reduziert nicht die interne 3D-Renderauflösung.
URP kann zusätzliche Farbkopien oder Tiefen-/Normalenpasses benötigen; ein Filterpass bedeutet daher nicht nur einen GPU-Arbeitsschritt. Der Filter ist für SDR-Desktop vorgesehen;
XR, HDR-Ausgabe und dynamische Auflösung sind nicht geprüft. Nachgeschaltetes Antialiasing oder Upscaling kann Pixel glätten.

## Weitergeben

Nur den Ordner Assets/PocketPixel über Assets > Export Package exportieren, keine Projektszenen,
Pipeline-Assets, projektfremde Spielobjekte oder Library-Ordner mitliefern. Das beiliegende unitypackage ist bereits so zusammengestellt.
Vor einer öffentlichen Store-Veröffentlichung sind zusätzliche Plattformtests und Produktdokumentation erforderlich.

## Auswahl in der Scene-Ansicht
Das vierfarbige Pixel-Symbol ist anklickbar und bleibt im Spiel unsichtbar. Scene-Gizmos einschalten. Es markiert nur das Filterobjekt, keine räumliche Effektgrenze.


## Lizenz und Quellcode
Privat und kommerziell nutzbar, veränderbar und weitergebbar. Veröffentlichte Produkte müssen den in der beiliegenden Lizenz vorgeschriebenen Credit enthalten. Vollständige Bedingungen: [LICENSE.txt](LICENSE.txt) (eigene Lizenz, nicht unverändertes MIT).
Alle C#- und Shaderquellen liegen bei. [TECHNICAL.md](TECHNICAL.md) beschreibt Implementierung und Wartung; [TROUBLESHOOTING.md](TROUBLESHOOTING.md) die Fehlerdiagnose.
