# Project Health Check for Unity

[![EditMode Tests](https://github.com/ludioo/Unity-Project-Health-Check/actions/workflows/editmode-tests.yml/badge.svg)](https://github.com/ludioo/Unity-Project-Health-Check/actions/workflows/editmode-tests.yml)
[![Unity 2022.3 LTS+](https://img.shields.io/badge/Unity-2022.3%20LTS%2B-222c37)](https://unity.com/releases/editor/whats-new/2022.3.23)
[![Release](https://img.shields.io/github/v/release/ludioo/Unity-Project-Health-Check?label=release)](https://github.com/ludioo/Unity-Project-Health-Check/releases/tag/v0.1.2)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

**Early MVP · Read-only · Safe to try**

A lightweight Unity Editor tool that scans your project for common health issues and helps you
locate affected assets — without modifying anything.

![Project Health Check Editor window](docs/images/project-health-check.png)

## What is this?

Project Health Check is an Editor-only package that runs a manual scan of your `Assets/` folder
and reports problems such as missing scripts, oversized textures, and invalid build scenes. It is
designed as a simple diagnostic step you can run before a milestone, release, or cleanup pass.

## Why use it?

Unity projects accumulate broken references and oversized assets quietly. These issues are easy to
miss during daily work and expensive to find late in production. This tool gives you one clear
report with paths you can ping in the Editor — read-only, no surprises.

## Is it safe?

Yes. Version 0.1 is intentionally **read-only**:

- Does not modify scenes, prefabs, import settings, or Build Settings
- Does not delete or rewrite assets
- Does not run in the background or hook into builds

The window states this before you scan. If you cancel a scan, your previous results are kept.

## Install

**Requirements:** Unity 2022.3 LTS or newer, and [Git](https://git-scm.com/) installed on your
machine (required for Package Manager Git URLs).

1. Open your Unity project.
2. Go to **Window → Package Manager**.
3. Click **+ → Add package from git URL…**
4. Paste this URL and confirm:

```text
https://github.com/ludioo/Unity-Project-Health-Check.git?path=/Packages/com.ludioo.project-health-check#v0.1.2
```

Unity downloads the package from the tagged release. No other dependencies are required.

<details>
<summary>Troubleshooting</summary>

- **Git not found** — Install Git and restart Unity.
- **Package resolution failed** — Confirm you are on Unity 2022.3+ and the URL includes `#v0.1.2`.
- **Want the latest from main** — Replace `#v0.1.2` with `#main` (may be less stable than a tagged release).

</details>

## Use it

1. Open **Tools → Project Health Check**
2. Optionally expand **Scan Settings** to adjust thresholds (defaults: 4096 px textures, 50 MiB files)
3. Click **Scan Project**
4. Review summary cards and grouped results
5. Click a row or **Ping** to locate an asset in the Project window

Results can be filtered by check type without running another scan.

**Note:** On large projects, the missing-script check may take longer because it inspects prefabs
and scenes under `Assets/`. You can cancel a scan at any time.

## What it detects

| Check | Reports |
| --- | --- |
| Missing Scripts | Missing MonoBehaviour components in prefabs and scenes |
| Oversized Textures | Source width or height above the configured pixel threshold |
| Large Files | Assets at or above the configured MiB threshold |
| Invalid Build Scenes | Empty or missing scene entries in Build Settings |
| Empty Folders | Leaf folders containing no assets or subfolders |

Scans cover `Assets/` only. Package dependencies and generated folders are excluded.

## Current limitations

This is an early MVP focused on core scanning. Not included yet:

- Automatic fixes
- Persistent settings (thresholds reset when the window closes)
- Issue suppressions or severity levels
- JSON or batch reports
- Background or pre-build scanning
- Scanning `Packages/` or generated folders

See [ROADMAP.md](ROADMAP.md) for planned work.

## What's next?

Upcoming priorities include persistent project settings, exportable reports, and safe quick fixes.
Progress is tracked openly in [ROADMAP.md](ROADMAP.md).

## Feedback and contributing

This project is in early release and **community feedback is welcome**. If you try it:

- [Open an issue](https://github.com/ludioo/Unity-Project-Health-Check/issues) for bugs, feedback, or ideas

To contribute code, see [CONTRIBUTING.md](CONTRIBUTING.md). Pull requests should stay focused
and dependency-free.

## Development

To work on the package itself, clone this repository and open its root as a Unity 2022.3 project:

```bash
git clone https://github.com/ludioo/Unity-Project-Health-Check.git
```

Production package code lives in `Packages/com.ludioo.project-health-check`. Development fixtures
live in `Assets/Development`.

EditMode tests run locally via the Unity Test Runner or in batchmode — see [CONTRIBUTING.md](CONTRIBUTING.md).
GitHub Actions runs the same EditMode suite on pushes and pull requests to `main` and `develop`.

## License

MIT — see [LICENSE](LICENSE).
