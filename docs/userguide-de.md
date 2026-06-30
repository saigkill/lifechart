# LifeChart – Benutzerhandbuch

LifeChart hilft dir, deine tägliche Stimmung und Funktionsfähigkeit zu dokumentieren, Medikamente zu verwalten und Verlaufsberichte für deinen Psychiater zu erstellen. Es ist ein persönliches Selbstbeobachtungstool – kein Medizinprodukt.

**Wenn du dich in einer Krise befindest, wende dich bitte an eine Krisenhotline.**
Deutschland: Telefonseelsorge **0800 111 0 111** (kostenlos, 24/7)

---

## Inhaltsverzeichnis

1. [Erste Schritte – Einrichtung](#1-erste-schritte--einrichtung)
2. [Tageseintrag](#2-tageseintrag)
3. [Verlaufsdiagramm](#3-verlaufsdiagramm)
4. [Medikamente](#4-medikamente)
5. [Krisenressourcen](#5-krisenressourcen)
6. [Einstellungen](#6-einstellungen)
7. [Datenschutz und Sicherheit](#7-datenschutz-und-sicherheit)
8. [Über LifeChart](#8-über-lifechart)

---

## 1. Erste Schritte – Einrichtung

Beim ersten Start führt dich LifeChart durch eine kurze Einrichtung.

<!-- BILD: Screenshot des Onboarding-Bildschirms -->

### Haftungsausschluss

Vor Beginn zeigt LifeChart einen wichtigen Hinweis: Die App ist ein persönliches Hilfsmittel, kein Ersatz für medizinische oder psychiatrische Behandlung. Lies ihn sorgfältig durch.

### Abend-Erinnerung einrichten

LifeChart kann dich jeden Abend daran erinnern, deinen Tageseintrag zu machen.

- **Erinnerung aktivieren:** Schalter auf „Ein" stellen.
- **Uhrzeit wählen:** Die Standardzeit ist 20:00 Uhr. Tippe auf die Zeitanzeige, um sie anzupassen.
- **Ohne Erinnerung:** Schalter auf „Aus" lassen – du kannst die Erinnerung jederzeit in den Einstellungen nachträglich aktivieren.

<!-- BILD: Detailansicht des Erinnerungs-Schalters und der Zeitauswahl -->

Tippe auf **„Loslegen"**, um die Einrichtung abzuschließen. Die App startet anschließend neu und öffnet den Hauptbereich.

---

## 2. Tageseintrag

Der Tageseintrag ist die zentrale Funktion von LifeChart. Öffne ihn über den Tab **„Heute"**.

<!-- BILD: Screenshot des Tageseintrags im Schnellmodus (nur Stimmung und Funktionsfähigkeit) -->

### Schnellmodus

Im Schnellmodus siehst du zwei Schieberegler:

| Regler | Skala | Bedeutung |
|---|---|---|
| **Stimmung** | −5 bis +5 | Dein subjektives Befinden (−5 = schwere Depression, +5 = schwere Manie) |
| **Funktionsfähigkeit** | −5 bis +5 | Wie gut du deinen Alltag bewältigen konntest |

Der aktuelle Wert wird rechts neben dem Regler angezeigt (z. B. `+2` oder `−3`). Der Wert 0 bedeutet neutral.

Tippe auf **„Speichern"**, um den Eintrag zu sichern.

### Vollständiger Modus

Tippe auf **„Mehr"**, um den erweiterten Bereich einzublenden.

<!-- BILD: Screenshot des Tageseintrags im vollständigen Modus mit allen Feldern -->

Zusätzliche Felder:

| Feld | Beschreibung |
|---|---|
| **Schlafdauer** | Geschlafene Stunden (Schieberegler 0–24 h) |
| **Medikamente genommen** | Hast du deine Medikamente heute eingenommen? |
| **Menstruationszyklus** | Optionale Markierung für hormonell beeinflusste Phasen |
| **Hypomanie** | Markierung für hypomane Phasen (erscheint im Diagramm als lila Markierung) |
| **Symptome** | Freitext für körperliche oder psychische Symptome |
| **Notizen** | Freitext für alles Weitere (Ereignisse, Gedanken) |

> **Tipp:** In den Einstellungen kannst du den Standardmodus festlegen (Schnell, Vollständig oder immer fragen).

### Kritische Werte

Wenn sowohl Stimmung als auch Funktionsfähigkeit bei −4 oder darunter liegen, wertet LifeChart den Eintrag als kritisch und zeigt einen Hinweis sowie einen Button zu den Krisenressourcen an.

<!-- BILD: Screenshot der Warnung bei kritischen Werten und des Krisenressourcen-Buttons -->

### Bestehenden Eintrag bearbeiten

Öffnest du den Tab „Heute" erneut am selben Tag, werden die bereits eingetragenen Werte automatisch geladen. Ein erneutes Speichern überschreibt den vorhandenen Eintrag.

---

## 3. Verlaufsdiagramm

Öffne den Tab **„Diagramm"**, um deine Stimmung und Funktionsfähigkeit über die Zeit zu sehen.

<!-- BILD: Screenshot des Verlaufsdiagramms mit Legende -->

### Zeitraum wählen

Mit den Schaltflächen **30**, **60** und **90** wählst du aus, wie viele vergangene Tage angezeigt werden sollen. Der aktuell aktive Zeitraum ist blau hervorgehoben.

### Diagramm lesen

| Farbe | Bedeutung |
|---|---|
| Blau (`#0072B2`) | Stimmung |
| Amber (`#E69F00`) | Funktionsfähigkeit |
| Lila (`#CC79A7`) | Hypomanie-Markierung |

- Die Y-Achse zeigt die Skala von −5 bis +5.
- Die X-Achse zeigt Datumsangaben (ca. 5 gleichmäßig verteilte Beschriftungen).
- Lila Markierungen erscheinen als senkrechter Band mit einem Diamant-Punkt oben, wenn an diesem Tag „Hypomanie" aktiviert war.
- Wenn noch keine Einträge für den gewählten Zeitraum vorhanden sind, erscheint ein entsprechender Hinweistext.

### PDF-Export

Tippe auf **„PDF exportieren"**, um einen Verlaufsbericht zu erstellen.

<!-- BILD: Screenshot des PDF-Export-Dialogs -->

Das PDF wird im Ordner `~/Dokumente/LifeChart/` gespeichert (Dateiname: `LifeChart_JJJJ-MM-TT_JJJJ-MM-TT.pdf`). Nach dem Speichern fragt LifeChart, ob du die Datei sofort öffnen möchtest.

> **Hinweis:** Zum Öffnen wird ein installierter PDF-Viewer benötigt.

---

## 4. Medikamente

Öffne den Tab **„Medikamente"**, um deine aktiven Medikamente zu verwalten.

<!-- BILD: Screenshot der Medikamentenliste mit einer Beispielkarte -->

### Medikament hinzufügen

Tippe auf **„Hinzufügen"** (oben rechts). Es öffnet sich das Formular für neue Medikamente.

<!-- BILD: Screenshot des Formulars zum Hinzufügen eines Medikaments -->

Fülle folgende Felder aus:

| Feld | Beschreibung |
|---|---|
| **Name** | Wirkstoff oder Handelsname (Pflichtfeld) |
| **Dosierung** | z. B. „200 mg" oder „1 Tablette" |
| **Mindestbestand** | Anzahl der Tabletten, ab der eine Warnung erscheint |
| **Aktueller Bestand** | Wie viele Tabletten du aktuell hast |
| **Einnahmezeiten** | Eine oder mehrere Uhrzeiten mit Dosisanzahl pro Einnahme |

Mit **„+ Zeit hinzufügen"** kannst du mehrere Einnahmezeiten pro Medikament anlegen. Die X-Schaltfläche neben jeder Zeile entfernt sie wieder.

Tippe auf **„Speichern"**, um das Medikament zu sichern.

### Medikament bearbeiten

Tippe auf **„Bearbeiten"** in der jeweiligen Medikamentenkarte. Das Formular öffnet sich mit den vorhandenen Daten vorausgefüllt.

### Medikament löschen

Tippe auf **„Löschen"** in der Karte. LifeChart fragt zur Bestätigung, bevor das Medikament deaktiviert wird.

### Bestandswarnung

Die Bestandsangabe in der Karte erscheint **grün**, wenn der Vorrat ausreicht, und **rot**, wenn der aktuelle Bestand unter den Mindestbestand gefallen ist.

<!-- BILD: Detailansicht der Medikamentenkarte mit roter Bestandswarnung -->

### Einnahme-Erinnerungen

Wenn Einnahmezeiten eingetragen sind und die Abend-Erinnerung aktiv ist, richtet LifeChart automatisch systemd-Timer ein (Linux), die dich zur eingestellten Zeit per Desktop-Benachrichtigung erinnern.

---

## 5. Krisenressourcen

Öffne den Tab **„Krise"**, wenn du Unterstützung brauchst oder Krisentelefonnummern nachschlagen möchtest.

<!-- BILD: Screenshot der Krisenressourcen-Seite mit einer Länderauswahl und einer Beispielkarte -->

### Land wählen

Wähle dein Land aus dem Dropdown-Menü. LifeChart erkennt beim ersten Öffnen automatisch deine Systemregion und wählt das passende Land vor. Verfügbare Länder:

- Deutschland, Österreich, Schweiz
- United Kingdom, United States, Australia, Canada

### Krisenhotline anrufen

Tippe auf **„Anrufen"**, um die Telefon-App mit der Nummer der Krisenhotline zu öffnen. Wenn kein Telefon-App verfügbar ist, wird die Nummer in die Zwischenablage kopiert.

Über **„Website"** (falls vorhanden) öffnet sich die Webseite der jeweiligen Einrichtung im Browser.

---

## 6. Einstellungen

Öffne den Tab **„Einstellungen"**, um LifeChart an deine Bedürfnisse anzupassen.

<!-- BILD: Screenshot der Einstellungsseite (oberer Bereich) -->

### Backup

| Einstellung | Beschreibung |
|---|---|
| **Cloud-Anbieter** | Kein Backup, Nextcloud oder lokaler Export |
| **Nextcloud-Zugangsdaten** | URL, Benutzername und Passwort (erscheint nur bei Nextcloud) |
| **Automatisches Backup** | Backup nach jedem Speichern |
| **Backup-Warnschwelle** | Anzahl Tage, nach denen eine Warnung erscheint, wenn kein Backup gemacht wurde |

### Erinnerungen

| Einstellung | Beschreibung |
|---|---|
| **Abend-Erinnerung** | Ein/Aus + Uhrzeit |
| **Krisenhinweis** | Zeigt bei kritischen Einträgen automatisch einen Hinweis auf die Krisenressourcen |

### Darstellung

| Einstellung | Beschreibung |
|---|---|
| **Eingabemodus** | Schnell (nur Stimmung/Funktionsfähigkeit), Vollständig (alle Felder) oder immer fragen |
| **Farbenblinder Modus** | Passt die Diagrammfarben für häufige Farbsehschwächen an |

### App

| Einstellung | Beschreibung |
|---|---|
| **Sprache** | Systemsprache, Deutsch oder Englisch |

Tippe auf **„Einstellungen speichern"**, um die Änderungen zu übernehmen.

<!-- BILD: Screenshot der Einstellungsseite (unterer Bereich mit Speichern-Button) -->

> **Hinweis:** Eine Sprachänderung wird erst nach einem Neustart der App vollständig wirksam.

---

## 7. Datenschutz und Sicherheit

- **Keine Konten, keine Cloud-Pflicht.** LifeChart speichert alle Daten lokal auf deinem Gerät. Es werden keine Daten ohne deine ausdrückliche Zustimmung übertragen.
- **Lokale Datenbank.** Alle Einträge werden in einer SQLite-Datenbank unter `~/.local/share/LifeChart/lifechart.db` gespeichert.
- **Backup ist optional.** Wenn du Nextcloud oder lokalen Export aktivierst, entscheidest du selbst, wohin deine Daten gehen.
- **Keine Werbung, kein Tracking.**

---

## 8. Über LifeChart

Öffne den Tab **„Über"**, um Versionsinformationen, Lizenz und Links zur Projektseite zu sehen.

<!-- BILD: Screenshot der Über-Seite -->

| Angabe | Inhalt |
|---|---|
| Entwickler | Sascha Manns |
| Lizenz | GNU General Public License v3.0 |
| Projektseite | https://github.com/saigkill/lifechart |
| Fehler melden | https://github.com/saigkill/lifechart/issues |

> LifeChart für Linux (GTK4) ist eine experimentelle Version. Fehler und unvollständige Funktionen sind möglich. Rückmeldungen über den Fehler-Tracker sind willkommen.

---

*LifeChart ist kein Medizinprodukt im Sinne der EU-Verordnung 2017/745 (MDR). Die App stellt keine medizinischen Diagnosen und ersetzt keine ärztliche oder psychiatrische Behandlung.*
