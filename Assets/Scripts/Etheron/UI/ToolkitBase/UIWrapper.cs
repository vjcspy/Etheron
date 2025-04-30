using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
namespace Etheron.UI.ToolkitBase
{
    [RequireComponent(requiredComponent: typeof(UIDocument))]
    public class UIWrapper : MonoBehaviour
    {
        [FormerlySerializedAs("controllerAsset")] [SerializeField] private UIControllerAssetBase controller;

        private void Awake()
        {
            if (controller == null)
            {
                Debug.LogError(message: "[UIToolkitWrapper] Controller Asset is not assigned.");
                return;
            }

            VisualElement root = GetComponent<UIDocument>()?.rootVisualElement;

            if (root == null)
            {
                Debug.LogError(message: "[UIToolkitWrapper] No rootVisualElement found!");
                return;
            }

            controller.Setup(root: root);
        }

        private void OnDestroy()
        {
            controller?.Cleanup();
        }
    }
}
