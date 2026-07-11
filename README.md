# Project Health Check for Unity

A lightweight, Editor-only tool for finding common project health issues in Unity projects.

> The project is in its initial setup phase. No health checks are implemented yet.

## Requirements

- Unity 2022.3 LTS or newer
- No third-party runtime dependencies

## Installation

In Unity, open **Window > Package Manager**, choose **Add package from git URL**, and enter:

```text
https://github.com/ludioo/Unity-Project-Health-Check.git?path=/Packages/com.ludioo.project-health-check
```

For production projects, install a tagged release by appending a version such as `#v0.1.0`.

## Development

Open the repository root as a Unity project. Production package files live in
`Packages/com.ludioo.project-health-check`; project-only fixtures live in `Assets/Development`.

See [CONTRIBUTING.md](CONTRIBUTING.md) before contributing.
