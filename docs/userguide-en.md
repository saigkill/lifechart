# LifeChart – User Guide

LifeChart helps you document your daily mood and functionality, manage medications, and generate progress reports for your psychiatrist. It is a personal self-monitoring tool — not a medical device.

**If you are in crisis, please contact a crisis helpline in your country.**
Germany: Telefonseelsorge **0800 111 0 111** (free, 24/7)

---

## Table of Contents

1. [Getting Started – Setup](#1-getting-started--setup)
2. [Daily Entry](#2-daily-entry)
3. [Chart](#3-chart)
4. [Medications](#4-medications)
5. [Crisis Resources](#5-crisis-resources)
6. [Settings](#6-settings)
7. [Privacy and Security](#7-privacy-and-security)
8. [About LifeChart](#8-about-lifechart)

---

## 1. Getting Started – Setup

On first launch, LifeChart guides you through a short setup.

<!-- IMAGE: Screenshot of the onboarding screen -->

### Disclaimer

Before you begin, LifeChart displays an important notice: the app is a personal aid, not a replacement for medical or psychiatric treatment. Please read it carefully.

### Setting Up an Evening Reminder

LifeChart can remind you every evening to make your daily entry.

- **Enable reminder:** Toggle the switch to on.
- **Choose a time:** The default time is 8:00 PM. Tap the time display to adjust it.
- **Without a reminder:** Leave the switch off — you can enable the reminder at any time in Settings.

<!-- IMAGE: Detail view of the reminder toggle and time picker -->

Tap **"Get Started"** to complete the setup. The app will restart and open the main area.

---

## 2. Daily Entry

The daily entry is the core feature of LifeChart. Open it via the **"Today"** tab.

<!-- IMAGE: Screenshot of the daily entry in quick mode (mood and functionality only) -->

### Quick Mode

In quick mode you see two sliders:

| Slider | Scale | Meaning |
|---|---|---|
| **Mood** | −5 to +5 | Your subjective well-being (−5 = severe depression, +5 = severe mania) |
| **Functionality** | −5 to +5 | How well you were able to manage your daily life |

The current value is displayed to the right of each slider (e.g. `+2` or `−3`). A value of 0 means neutral.

Tap **"Save"** to store the entry.

### Full Mode

Tap **"More"** to expand the extended fields.

<!-- IMAGE: Screenshot of the daily entry in full mode with all fields visible -->

Additional fields:

| Field | Description |
|---|---|
| **Sleep hours** | Hours slept (slider 0–24 h) |
| **Medication taken** | Did you take your medications today? |
| **Menstrual cycle** | Optional marker for hormonally influenced phases |
| **Hypomania** | Marker for hypomanic phases (appears as a purple marker in the chart) |
| **Symptoms** | Free text for physical or psychological symptoms |
| **Notes** | Free text for anything else (events, thoughts) |

> **Tip:** In Settings you can configure the default input mode (Quick, Full, or always ask).

### Critical Values

If both mood and functionality are at −4 or below, LifeChart flags the entry as critical and shows a warning along with a button to the crisis resources.

<!-- IMAGE: Screenshot of the critical values warning and the crisis resources button -->

### Editing an Existing Entry

If you open the "Today" tab again on the same day, the existing values are loaded automatically. Saving again overwrites the existing entry.

---

## 3. Chart

Open the **"Chart"** tab to see your mood and functionality over time.

<!-- IMAGE: Screenshot of the chart with legend -->

### Choosing a Time Range

Use the **30**, **60**, and **90** buttons to select how many past days to display. The currently active range is highlighted in blue.

### Reading the Chart

| Colour | Meaning |
|---|---|
| Blue (`#0072B2`) | Mood |
| Amber (`#E69F00`) | Functionality |
| Purple (`#CC79A7`) | Hypomania marker |

- The Y-axis shows the scale from −5 to +5.
- The X-axis shows dates (approximately 5 evenly spaced labels).
- Purple markers appear as a vertical band with a diamond dot at the top on days where "Hypomania" was enabled.
- If there are no entries for the selected period, an informational message is displayed instead.

### PDF Export

Tap **"Export PDF"** to generate a progress report.

<!-- IMAGE: Screenshot of the PDF export dialog -->

The PDF is saved to `~/Documents/LifeChart/` (filename: `LifeChart_YYYY-MM-DD_YYYY-MM-DD.pdf`). After saving, LifeChart asks whether you want to open the file immediately.

> **Note:** A PDF viewer must be installed to open the file.

---

## 4. Medications

Open the **"Medications"** tab to manage your active medications.

<!-- IMAGE: Screenshot of the medication list with an example card -->

### Adding a Medication

Tap **"Add"** (top right). The form for a new medication opens.

<!-- IMAGE: Screenshot of the add medication form -->

Fill in the following fields:

| Field | Description |
|---|---|
| **Name** | Active ingredient or brand name (required) |
| **Dosage** | e.g. "200 mg" or "1 tablet" |
| **Minimum stock** | Number of tablets at which a warning appears |
| **Current stock** | How many tablets you currently have |
| **Intake times** | One or more times of day with the dose count per intake |

Use **"+ Add time"** to add multiple intake times per medication. The X button next to each row removes it.

Tap **"Save"** to store the medication.

### Editing a Medication

Tap **"Edit"** on the relevant medication card. The form opens with the existing data pre-filled.

### Deleting a Medication

Tap **"Delete"** on the card. LifeChart asks for confirmation before deactivating the medication.

### Stock Warning

The stock display on the card appears **green** when the supply is sufficient, and **red** when the current stock has fallen below the minimum stock level.

<!-- IMAGE: Detail view of a medication card with a red stock warning -->

### Intake Reminders

If intake times are entered and the evening reminder is active, LifeChart automatically sets up systemd timers (Linux) that notify you at the configured time via a desktop notification.

---

## 5. Crisis Resources

Open the **"Crisis"** tab when you need support or want to look up crisis helpline numbers.

<!-- IMAGE: Screenshot of the crisis resources page with country selector and an example card -->

### Choosing a Country

Select your country from the dropdown menu. LifeChart automatically detects your system region on first open and pre-selects the matching country. Available countries:

- Germany, Austria, Switzerland
- United Kingdom, United States, Australia, Canada

### Calling a Crisis Helpline

Tap **"Call"** to open the phone app with the helpline number. If no phone app is available, the number is copied to the clipboard instead.

Tap **"Website"** (if shown) to open the organisation's website in your browser.

---

## 6. Settings

Open the **"Settings"** tab to customise LifeChart to your needs.

<!-- IMAGE: Screenshot of the settings page (upper section) -->

### Backup

| Setting | Description |
|---|---|
| **Cloud provider** | No backup, Nextcloud, or local export |
| **Nextcloud credentials** | URL, username, and password (shown only when Nextcloud is selected) |
| **Automatic backup** | Back up after every save |
| **Backup warning threshold** | Number of days after which a warning appears if no backup has been made |

### Reminders

| Setting | Description |
|---|---|
| **Evening reminder** | On/off + time |
| **Crisis hint** | Automatically shows a pointer to the crisis resources on critical entries |

### Display

| Setting | Description |
|---|---|
| **Input mode** | Quick (mood/functionality only), Full (all fields), or always ask |
| **Colour-blind mode** | Adjusts chart colours for common colour vision deficiencies |

### App

| Setting | Description |
|---|---|
| **Language** | System language, German, or English |

Tap **"Save Settings"** to apply your changes.

<!-- IMAGE: Screenshot of the settings page (lower section with save button) -->

> **Note:** A language change takes full effect only after restarting the app.

---

## 7. Privacy and Security

- **No accounts, no mandatory cloud.** LifeChart stores all data locally on your device. No data is transmitted without your explicit consent.
- **Local database.** All entries are stored in a SQLite database at `~/.local/share/LifeChart/lifechart.db`.
- **Backup is optional.** If you enable Nextcloud or local export, you decide where your data goes.
- **No advertising, no tracking.**

---

## 8. About LifeChart

Open the **"About"** tab to see version information, licence details, and links to the project page.

<!-- IMAGE: Screenshot of the About page -->

| Detail | Content |
|---|---|
| Developer | Sascha Manns |
| Licence | GNU General Public License v3.0 |
| Project page | https://github.com/saigkill/lifechart |
| Report a bug | https://github.com/saigkill/lifechart/issues |

> LifeChart for Linux (GTK4) is an experimental version. Bugs and incomplete features are possible. Feedback via the bug tracker is welcome.

---

*LifeChart is not a medical device as defined by EU Regulation 2017/745 (MDR). The app does not provide medical diagnoses and does not replace medical or psychiatric treatment.*
