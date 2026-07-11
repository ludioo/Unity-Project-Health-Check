using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProjectHealthCheck.Tests
{
    internal sealed class ProjectHealthCheckWindowTests
    {
        private ProjectHealthCheckWindow window;

        [SetUp]
        public void SetUp()
        {
            window = ScriptableObject.CreateInstance<ProjectHealthCheckWindow>();
            window.CreateGUI();
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(window);

        [Test]
        public void UiAssetsAndRequiredControls_Load()
        {
            Assert.That(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ProjectHealthCheckWindow.UxmlPath), Is.Not.Null);
            Assert.That(AssetDatabase.LoadAssetAtPath<StyleSheet>(ProjectHealthCheckWindow.UssPath), Is.Not.Null);
            Assert.That(window.rootVisualElement.Q<Button>("scan-button"), Is.Not.Null);
            Assert.That(window.rootVisualElement.Q<DropdownField>("category-filter"), Is.Not.Null);
            Assert.That(window.rootVisualElement.Q("results"), Is.Not.Null);
        }

        [Test]
        public void Settings_StartCollapsedWithExpectedDefaults()
        {
            Assert.That(window.rootVisualElement.Q<Foldout>("settings-foldout").value, Is.False);
            Assert.That(window.rootVisualElement.Q<IntegerField>("texture-threshold").value, Is.EqualTo(4096));
            Assert.That(window.rootVisualElement.Q<FloatField>("file-threshold").value, Is.EqualTo(50f));
        }

        [TestCase("missing-script", "Missing Scripts")]
        [TestCase("oversized-texture", "Oversized Textures")]
        [TestCase("large-file", "Large Files")]
        [TestCase("invalid-build-scene", "Invalid Build Scenes")]
        [TestCase("empty-folder", "Empty Folders")]
        public void FriendlyNames_CoverEveryCheck(string id, string expected)
        {
            Assert.That(ProjectHealthCheckWindow.FriendlyName(id), Is.EqualTo(expected));
        }

        [Test]
        public void Filtering_DoesNotChangeSourceResults()
        {
            var source = new List<HealthIssue>
            {
                new HealthIssue("large-file", "Large", "Assets/Large.bin"),
                new HealthIssue("empty-folder", "Empty", "Assets/Empty")
            };

            var filtered = ProjectHealthCheckWindow.FilterIssues(source, "Large Files");

            Assert.That(filtered.Single().CheckId, Is.EqualTo("large-file"));
            Assert.That(source, Has.Count.EqualTo(2));
        }

        [Test]
        public void Rendering_UpdatesFullSummaryAndHandlesEmptyState()
        {
            window.RenderForTests(new List<HealthIssue>
            {
                new HealthIssue("large-file", "Large", "Assets/Missing.bin"),
                new HealthIssue("empty-folder", "Empty", "Assets/Missing")
            });

            Assert.That(window.rootVisualElement.Q<Label>("total-label").text, Is.EqualTo("2"));
            Assert.That(window.rootVisualElement.Q<Label>("summary-count-large-file").text, Is.EqualTo("1"));
            Assert.That(ProjectHealthCheckWindow.CanPing(new HealthIssue("large-file", "Missing", "Assets/DoesNotExist.bin")), Is.False);

            window.RenderForTests(new List<HealthIssue>(), "No issues.");
            Assert.That(window.rootVisualElement.Q("empty-state").style.display.value, Is.EqualTo(DisplayStyle.Flex));
        }
    }
}
