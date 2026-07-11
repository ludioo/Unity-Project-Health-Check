using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ProjectHealthCheck
{
    internal sealed class ProjectHealthCheckWindow : EditorWindow
    {
        private int textureSize = ProjectHealthScanner.DefaultTextureSize;
        private float largeFileMiB = 50f;
        private List<HealthIssue> issues;
        private Vector2 scroll;
        private string status = "Ready to scan.";

        [MenuItem("Tools/Project Health Check")]
        private static void Open() => GetWindow<ProjectHealthCheckWindow>("Project Health Check");

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Project Health Check", EditorStyles.boldLabel);
            textureSize = Mathf.Max(1, EditorGUILayout.IntField("Texture threshold (px)", textureSize));
            largeFileMiB = Mathf.Max(.01f, EditorGUILayout.FloatField("Large file threshold (MiB)", largeFileMiB));

            if (GUILayout.Button("Scan Project")) Scan();
            EditorGUILayout.HelpBox(status, MessageType.Info);
            if (issues == null) return;
            if (issues.Count == 0) EditorGUILayout.LabelField("No issues found.");

            scroll = EditorGUILayout.BeginScrollView(scroll);
            foreach (var issue in issues)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(issue.CheckId, EditorStyles.boldLabel);
                EditorGUILayout.LabelField(issue.Message, EditorStyles.wordWrappedLabel);
                EditorGUILayout.SelectableLabel(string.IsNullOrEmpty(issue.AssetPath) ? "(no path)" : issue.AssetPath, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                if (!string.IsNullOrEmpty(issue.AssetPath) && GUILayout.Button("Select / Ping")) Ping(issue);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }

        private void Scan()
        {
            try
            {
                status = "Scanning...";
                issues = ProjectHealthScanner.Scan(textureSize, (long)(largeFileMiB * 1024 * 1024),
                    (label, progress) => EditorUtility.DisplayCancelableProgressBar("Project Health Check", label, progress));
                status = $"Scan complete: {issues.Count} issue(s).";
            }
            catch (OperationCanceledException)
            {
                status = "Scan cancelled.";
            }
            catch (Exception exception)
            {
                status = "Scan failed. See Console for details.";
                Debug.LogException(exception);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                Repaint();
            }
        }

        private static void Ping(HealthIssue issue)
        {
            var target = issue.Context != null ? issue.Context : AssetDatabase.LoadMainAssetAtPath(issue.AssetPath);
            if (target == null) return;
            Selection.activeObject = target;
            EditorGUIUtility.PingObject(target);
        }
    }
}
