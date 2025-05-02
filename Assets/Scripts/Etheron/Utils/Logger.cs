using System.Diagnostics;
using Debug = UnityEngine.Debug;
namespace Etheron.Utils
{
    public static class ELogger
    {
        [Conditional(conditionString: "UNITY_EDITOR")]
        public static void Log(string message)
        {
            Debug.Log(message: message);
        }

        [Conditional(conditionString: "UNITY_EDITOR")]
        public static void LogWarning(string message)
        {
            Debug.LogWarning(message: message);
        }

        [Conditional(conditionString: "UNITY_EDITOR")]
        public static void LogError(string message)
        {
            Debug.LogError(message: message);
        }
    }
}
