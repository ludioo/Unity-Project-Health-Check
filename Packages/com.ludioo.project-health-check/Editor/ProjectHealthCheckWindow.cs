using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProjectHealthCheck
{
    internal sealed class ProjectHealthCheckWindow : EditorWindow
    {
        internal const string UxmlPath = "Packages/com.ludioo.project-health-check/Editor/UI/ProjectHealthCheckWindow.uxml";
        internal const string UssPath = "Packages/com.ludioo.project-health-check/Editor/UI/ProjectHealthCheckWindow.uss";

        private static readonly Category[] Categories =
        {
            new Category("missing-script", "Missing Scripts", "cs Script Icon"),
            new Category("oversized-texture", "Oversized Textures", "Texture Icon"),
            new Category("large-file", "Large Files", "DefaultAsset Icon"),
            new Category("invalid-build-scene", "Invalid Build Scenes", "SceneAsset Icon"),
            new Category("empty-folder", "Empty Folders", "Folder Icon")
        };

        private int textureSize = ProjectHealthScanner.DefaultTextureSize;
        private float largeFileMiB = 50f;
        private List<HealthIssue> issues;
        private IntegerField textureField;
        private FloatField fileField;
        private DropdownField filterField;
        private Label statusLabel;
        private Label totalLabel;
        private VisualElement summary;
        private VisualElement results;
        private VisualElement emptyState;

        [MenuItem("Tools/Project Health Check")]
        private static void Open()
        {
            var window = GetWindow<ProjectHealthCheckWindow>("Project Health Check");
            window.minSize = new Vector2(360, 420);
        }

        public void CreateGUI()
        {
            rootVisualElement.Clear();
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            var style = AssetDatabase.LoadAssetAtPath<StyleSheet>(UssPath);
            if (tree == null || style == null)
            {
                rootVisualElement.Add(new HelpBox("Project Health Check UI assets could not be loaded.", HelpBoxMessageType.Error));
                return;
            }

            tree.CloneTree(rootVisualElement);
            rootVisualElement.styleSheets.Add(style);
            rootVisualElement.Q<Image>("header-icon").image = EditorGUIUtility.IconContent("console.infoicon").image;

            textureField = rootVisualElement.Q<IntegerField>("texture-threshold");
            fileField = rootVisualElement.Q<FloatField>("file-threshold");
            filterField = rootVisualElement.Q<DropdownField>("category-filter");
            statusLabel = rootVisualElement.Q<Label>("status-label");
            totalLabel = rootVisualElement.Q<Label>("total-label");
            summary = rootVisualElement.Q("summary");
            results = rootVisualElement.Q("results");
            emptyState = rootVisualElement.Q("empty-state");

            textureField.value = textureSize;
            fileField.value = largeFileMiB;
            textureField.RegisterValueChangedCallback(evt =>
            {
                textureSize = Mathf.Max(1, evt.newValue);
                if (textureSize != evt.newValue) textureField.SetValueWithoutNotify(textureSize);
            });
            fileField.RegisterValueChangedCallback(evt =>
            {
                largeFileMiB = Mathf.Max(.01f, evt.newValue);
                if (!Mathf.Approximately(largeFileMiB, evt.newValue)) fileField.SetValueWithoutNotify(largeFileMiB);
            });
            rootVisualElement.Q<Button>("reset-settings").clicked += ResetSettings;
            rootVisualElement.Q<Button>("scan-button").clicked += Scan;

            filterField.choices = new[] { "All checks" }.Concat(Categories.Select(category => category.Name)).ToList();
            filterField.value = "All checks";
            filterField.RegisterValueChangedCallback(_ => RenderResults());
            ShowReady();
        }

        internal static string FriendlyName(string checkId)
        {
            var category = Categories.FirstOrDefault(item => item.Id == checkId);
            return category.Name ?? checkId;
        }

        internal static List<HealthIssue> FilterIssues(IEnumerable<HealthIssue> source, string filter)
        {
            if (filter == "All checks") return source.ToList();
            var id = Categories.FirstOrDefault(category => category.Name == filter).Id;
            return source.Where(issue => issue.CheckId == id).ToList();
        }

        internal void RenderForTests(List<HealthIssue> testIssues, string message = "Scan complete.")
        {
            issues = testIssues;
            statusLabel.text = message;
            RenderSummary();
            RenderResults();
        }

        private void ResetSettings()
        {
            textureSize = ProjectHealthScanner.DefaultTextureSize;
            largeFileMiB = 50f;
            textureField.SetValueWithoutNotify(textureSize);
            fileField.SetValueWithoutNotify(largeFileMiB);
        }

        private void Scan()
        {
            var previous = issues;
            filterField.SetValueWithoutNotify("All checks");
            statusLabel.text = "Scanning project...";
            try
            {
                var scanned = ProjectHealthScanner.Scan(textureSize, (long)(largeFileMiB * 1024 * 1024),
                    (label, progress) => EditorUtility.DisplayCancelableProgressBar("Project Health Check", label, progress));
                issues = scanned;
                statusLabel.text = $"Scan complete — {issues.Count} issue{(issues.Count == 1 ? "" : "s")} found.";
                RenderSummary();
                RenderResults();
            }
            catch (OperationCanceledException)
            {
                issues = previous;
                statusLabel.text = "Scan cancelled. Previous results are unchanged.";
                if (issues != null) { RenderSummary(); RenderResults(); }
            }
            catch (Exception exception)
            {
                issues = previous;
                statusLabel.text = "Scan failed. See Console for details.";
                if (issues != null) { RenderSummary(); RenderResults(); }
                Debug.LogException(exception);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void ShowReady()
        {
            statusLabel.text = "Ready to scan. Your project will not be modified.";
            summary.style.display = DisplayStyle.None;
            results.style.display = DisplayStyle.None;
            emptyState.style.display = DisplayStyle.Flex;
            emptyState.Q<Label>().text = "Run a scan to review project health.";
        }

        private void RenderSummary()
        {
            summary.Clear();
            summary.style.display = DisplayStyle.Flex;
            totalLabel.text = issues.Count.ToString();
            foreach (var category in Categories)
            {
                var count = issues.Count(issue => issue.CheckId == category.Id);
                var card = new VisualElement();
                card.AddToClassList("summary-card");
                if (count == 0) card.AddToClassList("summary-card--muted");
                var icon = new Image { image = EditorGUIUtility.IconContent(category.Icon).image };
                icon.AddToClassList("summary-icon");
                card.Add(icon);
                card.Add(new Label(count.ToString()) { name = "summary-count-" + category.Id });
                card.Add(new Label(category.Name));
                summary.Add(card);
            }
        }

        private void RenderResults()
        {
            if (issues == null) return;
            results.Clear();
            var visible = FilterIssues(issues, filterField.value);
            results.style.display = visible.Count == 0 ? DisplayStyle.None : DisplayStyle.Flex;
            emptyState.style.display = visible.Count == 0 ? DisplayStyle.Flex : DisplayStyle.None;
            emptyState.Q<Label>().text = issues.Count == 0 ? "No issues found. Your project looks healthy." : "No issues match this filter.";

            foreach (var category in Categories)
            {
                var groupIssues = visible.Where(issue => issue.CheckId == category.Id).ToList();
                if (groupIssues.Count == 0) continue;
                var foldout = new Foldout { text = $"{category.Name}  ({groupIssues.Count})", value = true };
                foldout.AddToClassList("result-group");
                foreach (var issue in groupIssues) foldout.Add(CreateResultRow(issue, category));
                results.Add(foldout);
            }
        }

        private static VisualElement CreateResultRow(HealthIssue issue, Category category)
        {
            var row = new VisualElement();
            row.AddToClassList("result-row");
            row.Add(new Image { image = EditorGUIUtility.IconContent(category.Icon).image });
            var text = new VisualElement();
            text.AddToClassList("result-copy");
            text.Add(new Label(issue.Message) { name = "issue-message" });
            var path = new Label(string.IsNullOrEmpty(issue.AssetPath) ? "No asset path" : issue.AssetPath);
            path.AddToClassList("asset-path");
            text.Add(path);
            row.Add(text);

            var ping = new Button(() => Ping(issue)) { text = "Ping" };
            ping.AddToClassList("ping-button");
            ping.SetEnabled(CanPing(issue));
            row.Add(ping);
            row.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.target != ping) Ping(issue);
            });
            return row;
        }

        internal static bool CanPing(HealthIssue issue) => ResolveTarget(issue) != null;

        private static UnityEngine.Object ResolveTarget(HealthIssue issue)
        {
            if (issue.Context != null) return issue.Context;
            return string.IsNullOrEmpty(issue.AssetPath) ? null : AssetDatabase.LoadMainAssetAtPath(issue.AssetPath);
        }

        private static void Ping(HealthIssue issue)
        {
            var target = ResolveTarget(issue);
            if (target == null) return;
            Selection.activeObject = target;
            EditorGUIUtility.PingObject(target);
        }

        private sealed class Category
        {
            internal Category(string id, string name, string icon) { Id = id; Name = name; Icon = icon; }
            internal string Id { get; }
            internal string Name { get; }
            internal string Icon { get; }
        }
    }
}
