# Project Health Check

**Early MVP · Read-only · Safe to try**

Open **Tools → Project Health Check** and select **Scan Project**. The tool scans `Assets/` only
and does not modify project content.

## Install

Requires Unity 2022.3 LTS or newer and Git (for Package Manager Git URLs).

```text
https://github.com/ludioo/Unity-Project-Health-Check.git?path=/Packages/com.ludioo.project-health-check#v0.1.2
```

## Use

1. Open **Tools → Project Health Check**
2. Optionally expand **Scan Settings** to change thresholds or restore defaults
3. Click **Scan Project**
4. Filter grouped results or **Ping** an asset to locate it

## Checks

- **Invalid build scenes:** Build Settings entries with an empty or missing scene path.
- **Large files:** Assets at or above the file threshold (default 50 MiB).
- **Oversized textures:** Source width or height above the texture threshold (default 4096 px).
- **Empty folders:** Leaf folders containing no asset or subfolder.
- **Missing scripts:** Missing MonoBehaviour components in prefabs and scenes.

## Limitations (v0.1)

Threshold changes last for the current window only. Automatic fixes, suppressions, background
scans, and package scanning are not included yet.

Feedback and roadmap: [GitHub repository](https://github.com/ludioo/Unity-Project-Health-Check).
