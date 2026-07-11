# Project Health Check

Open **Tools > Project Health Check** and select **Scan Project**. Expand **Scan Settings** to change
thresholds or restore their defaults. The tool scans `Assets` only and does not modify project
content. Summary cards and grouped results can be narrowed with the category filter without
running another scan.

## Checks

- **Invalid build scenes:** Build Settings entries with an empty or missing scene path.
- **Large files:** Assets at or above the file threshold (default 50 MiB).
- **Oversized textures:** Source width or height above the texture threshold (default 4096 px).
- **Empty folders:** Leaf folders containing no asset or subfolder.
- **Missing scripts:** Missing MonoBehaviour components in prefabs and scenes.

Threshold changes last for the current window only. Results support Select/Ping; automatic fixes,
suppressions, background scans, and package scanning are not included in version 0.1.
