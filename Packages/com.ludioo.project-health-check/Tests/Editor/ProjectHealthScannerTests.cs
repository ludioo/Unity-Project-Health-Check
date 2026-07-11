using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProjectHealthCheck.Tests
{
    internal sealed class ProjectHealthScannerTests
    {
        private const string TempRoot = "Assets/ProjectHealthCheckTests";
        private EditorBuildSettingsScene[] buildScenes;

        [SetUp]
        public void SetUp()
        {
            buildScenes = EditorBuildSettings.scenes;
            AssetDatabase.DeleteAsset(TempRoot);
            AssetDatabase.CreateFolder("Assets", "ProjectHealthCheckTests");
        }

        [TearDown]
        public void TearDown()
        {
            EditorBuildSettings.scenes = buildScenes;
            AssetDatabase.DeleteAsset(TempRoot);
            AssetDatabase.Refresh();
        }

        [Test]
        public void InvalidBuildScene_IsReportedWithoutChangingSettings()
        {
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Missing.unity", false) };
            var issues = new List<HealthIssue>();

            ProjectHealthScanner.CheckBuildScenes(issues);

            Assert.That(issues.Single().CheckId, Is.EqualTo("invalid-build-scene"));
            Assert.That(EditorBuildSettings.scenes.Single().path, Is.EqualTo("Assets/Missing.unity"));
        }

        [Test]
        public void LargeFile_UsesInclusiveByteThreshold()
        {
            var path = TempRoot + "/file.bytes";
            File.WriteAllBytes(path, new byte[16]);
            AssetDatabase.ImportAsset(path);

            var atThreshold = new List<HealthIssue>();
            ProjectHealthScanner.CheckLargeFiles(atThreshold, 16);
            var aboveThreshold = new List<HealthIssue>();
            ProjectHealthScanner.CheckLargeFiles(aboveThreshold, 17);

            Assert.That(atThreshold.Any(issue => issue.AssetPath == path), Is.True);
            Assert.That(aboveThreshold.Any(issue => issue.AssetPath == path), Is.False);
        }

        [Test]
        public void Texture_OnlyReportsDimensionsAboveThreshold()
        {
            var path = TempRoot + "/texture.png";
            var texture = new Texture2D(2, 1);
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path);

            var equal = new List<HealthIssue>();
            ProjectHealthScanner.CheckTextures(equal, 2);
            var below = new List<HealthIssue>();
            ProjectHealthScanner.CheckTextures(below, 1);

            Assert.That(equal.Any(issue => issue.AssetPath == path), Is.False);
            Assert.That(below.Any(issue => issue.AssetPath == path), Is.True);
        }

        [Test]
        public void EmptyFolder_ReportsLeafButNotParent()
        {
            AssetDatabase.CreateFolder(TempRoot, "Parent");
            AssetDatabase.CreateFolder(TempRoot + "/Parent", "Leaf");
            var issues = new List<HealthIssue>();

            ProjectHealthScanner.CheckEmptyFolders(issues);

            Assert.That(issues.Any(issue => issue.AssetPath == TempRoot + "/Parent/Leaf"), Is.True);
            Assert.That(issues.Any(issue => issue.AssetPath == TempRoot + "/Parent"), Is.False);
        }

        [Test]
        public void MissingScripts_AreReportedForPrefabAndSceneWhileSetupIsPreserved()
        {
            var scenePath = TempRoot + "/MissingScript.unity";
            var fixture = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Development/Fixtures/MissingScript.prefab");
            EditorSceneManager.OpenScene("Assets/Development/Scenes/SampleScene.unity", OpenSceneMode.Single);
            var fixtureScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            PrefabUtility.InstantiatePrefab(fixture, fixtureScene);
            EditorSceneManager.SaveScene(fixtureScene, scenePath);
            EditorSceneManager.CloseScene(fixtureScene, true);
            var before = EditorSceneManager.GetSceneManagerSetup();
            var issues = new List<HealthIssue>();

            ProjectHealthScanner.CheckMissingScripts(issues);

            var after = EditorSceneManager.GetSceneManagerSetup();
            Assert.That(issues.Any(issue => issue.AssetPath.EndsWith("MissingScript.prefab")), Is.True);
            Assert.That(issues.Any(issue => issue.AssetPath == scenePath), Is.True);
            Assert.That(after.Select(item => item.path), Is.EqualTo(before.Select(item => item.path)));
            Assert.That(after.Select(item => item.isLoaded), Is.EqualTo(before.Select(item => item.isLoaded)));
        }

        [Test]
        public void FullScan_ReturnsDeterministicallySortedResults()
        {
            var issues = ProjectHealthScanner.Scan(int.MaxValue, long.MaxValue);
            var sorted = issues.OrderBy(issue => issue.CheckId).ThenBy(issue => issue.AssetPath).ToArray();

            Assert.That(issues, Is.EqualTo(sorted));
        }
    }
}
