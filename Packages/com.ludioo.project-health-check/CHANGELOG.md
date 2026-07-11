# Changelog

All notable changes to this package will be documented in this file.

## [0.1.2] - 2026-07-12

### Added

- Automated Unity EditMode tests with GitHub Actions.
- CI runs for pushes and pull requests targeting `main` and `develop`.
- Test artifacts and Unity logs for CI failure diagnosis.
- CI validation using Unity 2022.3.23f1.

## [0.1.1] - 2026-07-12

### Changed

- Rebuilt the Editor window with UI Toolkit.
- Added responsive summary cards, friendly grouped results, and category filtering.
- Moved thresholds into a collapsed settings panel with reset-to-default behavior.
- Improved ready, success, empty, cancelled, and failed states.

## [0.1.0] - 2026-07-11

### Added

- Manual Project Health Check Editor window.
- Checks for missing scripts, oversized textures, large files, invalid build scenes, and empty folders.
- Editable per-window thresholds and Select/Ping result actions.
- EditMode test coverage for all checks.
