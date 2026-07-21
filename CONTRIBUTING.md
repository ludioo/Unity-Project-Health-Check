# Contributing

Thank you for looking at Project Health Check. This is an early MVP — focused contributions and
clear feedback are both valuable.

## Development setup

1. Clone the repository:
   ```bash
   git clone https://github.com/ludioo/Unity-Project-Health-Check.git
   ```
2. Open its root with Unity 2022.3 LTS.
3. Make package changes under `Packages/com.ludioo.project-health-check`.
4. Keep development-only assets under `Assets/Development`.
5. Run EditMode tests before opening a pull request.

From PowerShell, EditMode tests can also run headlessly when `UNITY_EDITOR` points to Unity.exe:

```powershell
& $env:UNITY_EDITOR -batchmode -quit -projectPath $PWD -runTests -testPlatform EditMode -testResults TestResults.xml
```

GitHub Actions runs the same EditMode suite for pushes and pull requests targeting `main` and
`develop`.

Do not commit generated Unity directories such as `Library`, `Temp`, `Logs`, or `UserSettings`.

## Guidelines

- Keep changes focused and dependency-free. New dependencies require prior discussion.
- Do not add product features in drive-by pull requests unless discussed in an issue first.
- Checks remain internal until a real external consumer requires a public extension API.
- Match existing code style and test coverage for behavioral changes.

## Reporting issues

Use [GitHub Issues](https://github.com/ludioo/Unity-Project-Health-Check/issues). Include your
Unity version, how you installed the package, and steps to reproduce when reporting bugs.
