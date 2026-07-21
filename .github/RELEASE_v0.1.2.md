# Release v0.1.2 — CI and stability

**Early MVP · Read-only · Safe to try**

First public release recommended for feedback and evaluation.

## Install

```text
https://github.com/ludioo/Unity-Project-Health-Check.git?path=/Packages/com.ludioo.project-health-check#v0.1.2
```

Requires Unity 2022.3 LTS+ and Git.

## Highlights

- Manual project health scan from **Tools → Project Health Check**
- Five read-only checks: missing scripts, oversized textures, large files, invalid build scenes, empty folders
- UI Toolkit window with summary cards, grouped results, and category filtering
- Select/Ping to locate affected assets
- Cancelable scan with progress bar
- EditMode test coverage and GitHub Actions CI

## Safety

This release does not modify project assets, import settings, or Build Settings.

## Known limitations

- Thresholds reset when the window closes
- No auto-fix, suppressions, or export reports
- Scans `Assets/` only; large projects may take longer during missing-script checks

## Feedback

Try it and [open an issue](https://github.com/ludioo/Unity-Project-Health-Check/issues) with bugs or ideas.

Full changelog: [CHANGELOG.md](Packages/com.ludioo.project-health-check/CHANGELOG.md)
