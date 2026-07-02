| What                   | Status                                                                                                                                                                                  |
|------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Continuous Integration | [![Build & Release](https://github.com/saigkill/lifechart/actions/workflows/build.yml/badge.svg)](https://github.com/saigkill/lifechart/actions/workflows/build.yml)                    |
| Static Code Analysis   | [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=saigkill_lifechart&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=saigkill_lifechart) |
| Bugreports             | [![GitHub issues](https://img.shields.io/github/issues/saigkill/lifechart)](https://github.com/saigkill/lifechart/issues)                                                               |
| Downloads all          | ![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/saigkill/lifechart/total)                                                                        |
| Docs                   | https://writebook.saschamanns.de/6/lifechart                                                                                                                                            |
| Build Report           | https://saigkill.github.io/lifechart/                                                                                                                                                   |
| Blog                   | [![Blog](https://img.shields.io/badge/Blog-Saigkill-blue)](https://saschamanns.de)                                                                                                      |

File a bug report [on Github](https://github.com/saigkill/lifechart/issues).

# LifeChart

A mood and wellness tracker for people with bipolar disorder, based on the [NIMH Life Chart methodology](https://www.nimh.nih.gov/).

Developed by someone affected, for those affected.

## About

LifeChart helps you document your daily mood and functionality, track medication, and generate progress reports for your psychiatrist. It is a personal self-observation tool — not a medical device.

**If you are in crisis, please contact a crisis helpline in your country.**
Germany: Telefonseelsorge **0800 111 0 111** (free, 24/7)

## Features

- Daily entry wizard (quick mode: 2 sliders; full mode: complete form)
- Mood and functionality tracking on a −5 to +5 scale
- Sleep hours, medication tracking, symptom notes
- Medication management with stock levels and intake alarms
- Chart view (30/60/90-day trends)
- PDF export including chart, daily table, notes, and medication list
- Automatic backup to Google Drive or Nextcloud
- Full anonymity — no account required, no data leaves your device without your consent

## Artifacts

| Platform    | Format                  |
|-------------|-------------------------|
| Android     | APK                     |
| Windows     | ZIP                     |
| Linux Gtk 4 | ZIP, RPM, DEB, AppImage |

Download the latest version on [Releases](https://github.com/saigkill/lifechart/releases)

## Architecture

Clean Architecture with DDD-Lite:

```
LifeChart/              ← MAUI UI + ViewModels
LifeChart.Domain/       ← Entities, Value Objects, Repository Interfaces
LifeChart.Application/  ← Use Cases, DTOs, Service Interfaces
LifeChart.Infrastructure/ ← EF Core, Repositories, Backup, PDF
LifeChart.Linux/        ← Linux UI + ViewModels
LifeChart.Tests/        ← Unit Tests
```

## Building

Requires .NET 10 with MAUI workload on Windows or macOS:

```bash
dotnet workload install maui
dotnet build LifeChart.sln
```

Android APK:
```bash
dotnet publish LifeChart/LifeChart.csproj -f net10.0-android -c Release
```

Windows:
```bash
dotnet publish LifeChart/LifeChart.csproj -f net10.0-windows10.0.19041.0 -c Release
```

## Usage

See the official documentation: https://writebook.saschamanns.de/6/lifechart

## Disclaimer

LifeChart is a personal self-monitoring tool. It is not a medical device as defined by EU Regulation 2017/745 (MDR). The app does not provide medical diagnoses and does not replace medical or psychiatric treatment. Use at your own risk.

## License

GNU General Public License v3 or later — see [LICENSE](LICENSE).

## Related

- [GNOME LifeChart](https://gitlab.gnome.org/saigkill/gnome-lifechart) — the Linux desktop version (Vala/GTK4)
