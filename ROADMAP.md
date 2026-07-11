# Roadmap

## Foundation

- [x] Embedded UPM package structure
- [x] Unity 2022.3 LTS project and Git hygiene
- [x] MIT license and package metadata
- [x] Editor and EditMode test assemblies

## Scanner framework

- [x] Internal issue model and scanner
- [x] Manual Editor window with editable thresholds
- [x] Deterministic results and Select/Ping action

## Health checks

- [x] Invalid build scenes
- [x] Large files
- [x] Oversized textures
- [x] Empty folders
- [x] Missing scripts in prefabs and scenes

## Testing

- [x] EditMode tests and temporary fixtures
- [x] Scene and Build Settings restoration tests
- [x] Local batchmode test command

## Documentation

- [x] User guide for installation and checks
- [x] Contributor guide for structure and tests

## Release v0.1.0

- [x] Clean Unity compilation and EditMode test run
- [x] Install package from tagged Git URL in a clean project
- [x] Final changelog and `v0.1.0` tag

## Release v0.1.1 — UI/UX

- [x] Migrate the Editor window to UI Toolkit
- [x] Add responsive summary, grouping, and category filtering
- [x] Add UI state and interaction tests
- [x] Update the README screenshot and validate the release package
- [x] Tag and publish `v0.1.1`

## Release v0.1.2 — CI

- [x] Run EditMode tests on pushes and pull requests targeting `main` and `develop`
- [x] Preserve test artifacts and logs when CI fails
- [x] Validate CI with Unity 2022.3.23f1

## Post-v0.1 backlog

- [ ] Persistent Project Settings
- [ ] Filtering, severity, and suppressions
- [ ] JSON and batch scan reports
- [x] GitHub Actions EditMode CI
- [ ] Safe quick fixes and additional checks
- [ ] Public custom-rule API when an external consumer needs it
