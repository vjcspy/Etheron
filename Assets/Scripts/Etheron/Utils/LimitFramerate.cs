using UnityEngine;
namespace Etheron.Utils
{
    public class LimitFramerate : MonoBehaviour
    {
        [Tooltip(tooltip: "Target frame rate. Set to 120 by default.")]
        public int targetFrameRate = 120;

        private void Awake()
        {
            // Tắt VSync để không override targetFrameRate
            QualitySettings.vSyncCount = 0;

            // Thiết lập giới hạn FPS
            Application.targetFrameRate = targetFrameRate;

            Debug.Log(message: $"[LimitFramerate] Set target frame rate to {targetFrameRate} FPS");
        }

        private void OnValidate()
        {
            // Giữ giá trị hợp lệ
            targetFrameRate = Mathf.Clamp(value: targetFrameRate, min: 15, max: 1000);
        }
    }
}
