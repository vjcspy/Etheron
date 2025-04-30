using UnityEngine;
using UnityEngine.UIElements;
namespace Etheron.UI.ToolkitBase
{
    public abstract class UIControllerAssetBase : ScriptableObject
    {
        public abstract void Setup(VisualElement root);
        public abstract void Cleanup();
    }
}
