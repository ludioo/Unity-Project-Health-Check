using UnityEngine;

namespace ProjectHealthCheck
{
    internal sealed class HealthIssue
    {
        internal HealthIssue(string checkId, string message, string assetPath, Object context = null)
        {
            CheckId = checkId;
            Message = message;
            AssetPath = assetPath;
            Context = context;
        }

        internal string CheckId { get; }
        internal string Message { get; }
        internal string AssetPath { get; }
        internal Object Context { get; }
    }
}
