using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectHealthCheck
{
    internal static class ProjectHealthScanner
    {
        internal const int DefaultTextureSize = 4096;
        internal const long DefaultLargeFileBytes = 50L * 1024 * 1024;

        internal static List<HealthIssue> Scan(int textureSize, long largeFileBytes, Func<string, float, bool> cancel = null)
        {
            if (textureSize < 1) throw new ArgumentOutOfRangeException(nameof(textureSize));
            if (largeFileBytes < 1) throw new ArgumentOutOfRangeException(nameof(largeFileBytes));

            var issues = new List<HealthIssue>();
            Run("Invalid build scenes", 0f, cancel, () => CheckBuildScenes(issues));
            Run("Large files", .2f, cancel, () => CheckLargeFiles(issues, largeFileBytes));
            Run("Oversized textures", .4f, cancel, () => CheckTextures(issues, textureSize));
            Run("Empty folders", .6f, cancel, () => CheckEmptyFolders(issues));
            Run("Missing scripts", .8f, cancel, () => CheckMissingScripts(issues));
            return issues.OrderBy(issue => issue.CheckId, StringComparer.Ordinal)
                .ThenBy(issue => issue.AssetPath, StringComparer.Ordinal).ToList();
        }

        private static void Run(string label, float progress, Func<string, float, bool> cancel, Action check)
        {
            if (cancel != null && cancel(label, progress)) throw new OperationCanceledException();
            check();
        }

        internal static void CheckBuildScenes(List<HealthIssue> issues)
        {
            foreach (var scene in EditorBuildSettings.scenes)
                if (string.IsNullOrEmpty(scene.path) || AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path) == null)
                    issues.Add(new HealthIssue("invalid-build-scene", "Build Settings references a missing scene.", scene.path));
        }

        internal static void CheckLargeFiles(List<HealthIssue> issues, long thresholdBytes)
        {
            foreach (var path in AssetDatabase.GetAllAssetPaths())
            {
                if (!path.StartsWith("Assets/", StringComparison.Ordinal) || path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase)) continue;
                var file = new FileInfo(Path.GetFullPath(path));
                if (file.Exists && file.Length >= thresholdBytes)
                    issues.Add(new HealthIssue("large-file", $"File is {file.Length / 1048576d:0.##} MiB.", path, AssetDatabase.LoadMainAssetAtPath(path)));
            }
        }

        internal static void CheckTextures(List<HealthIssue> issues, int threshold)
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!(AssetImporter.GetAtPath(path) is TextureImporter importer)) continue;
                importer.GetSourceTextureWidthAndHeight(out var width, out var height);
                if (width > threshold || height > threshold)
                    issues.Add(new HealthIssue("oversized-texture", $"Source texture is {width}x{height}px.", path, AssetDatabase.LoadMainAssetAtPath(path)));
            }
        }

        internal static void CheckEmptyFolders(List<HealthIssue> issues)
        {
            foreach (var directory in Directory.GetDirectories(Application.dataPath, "*", SearchOption.AllDirectories))
            {
                if (Directory.EnumerateFileSystemEntries(directory).Any(entry => !entry.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))) continue;
                var path = "Assets" + directory.Substring(Application.dataPath.Length).Replace('\\', '/');
                issues.Add(new HealthIssue("empty-folder", "Folder contains no assets or subfolders.", path, AssetDatabase.LoadMainAssetAtPath(path)));
            }
        }

        internal static void CheckMissingScripts(List<HealthIssue> issues)
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var root = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                CheckHierarchy(root, path, issues);
            }

            var opened = new List<Scene>();
            try
            {
                foreach (var guid in AssetDatabase.FindAssets("t:Scene", new[] { "Assets" }))
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var scene = SceneManager.GetSceneByPath(path);
                    if (!scene.IsValid() || !scene.isLoaded)
                    {
                        scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                        opened.Add(scene);
                    }
                    foreach (var root in scene.GetRootGameObjects()) CheckHierarchy(root, path, issues);
                }
            }
            finally
            {
                foreach (var scene in opened.Where(scene => scene.IsValid()).Reverse<Scene>())
                    EditorSceneManager.CloseScene(scene, true);
            }
        }

        private static void CheckHierarchy(GameObject root, string path, List<HealthIssue> issues)
        {
            if (root == null) return;
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
            {
                var count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject);
                if (count > 0)
                    issues.Add(new HealthIssue("missing-script", $"'{transform.name}' has {count} missing script component(s).", path, transform.gameObject));
            }
        }
    }
}
