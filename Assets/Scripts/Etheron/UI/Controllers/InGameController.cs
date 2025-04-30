using Etheron.UI.ToolkitBase;
using UnityEngine;
using UnityEngine.UIElements;
namespace Etheron.UI.Controllers
{
    [CreateAssetMenu(menuName = "UI Toolkit/Controllers/InGameController")]
    public class InGameControllerAsset : UIControllerAssetBase
    {
        [SerializeField] private UIComponentReference mainMenuComponentRef;

        private TemplateContainer mainMenuContainer;
        private Button menuBtn;

        public override void Setup(VisualElement root)
        {
            menuBtn = root.Q<Button>(name: "MenuBtn");

            if (menuBtn != null)
            {
                menuBtn.clicked += OnMenuBtnClicked;
            }

            VisualElement container = root.Q<VisualElement>(name: "Container");
            VisualElement mainContainer = container.Q<VisualElement>(name: "Main");
            mainMenuContainer = mainMenuComponentRef.visualTreeAsset.Instantiate();

            if (mainMenuContainer != null)
            {
                mainContainer.Add(child: mainMenuContainer);
                mainMenuContainer.style.display = DisplayStyle.None;
                mainMenuComponentRef.controllerAsset.Setup(root: mainMenuContainer);
            }
        }

        public override void Cleanup()
        {
            mainMenuComponentRef.controllerAsset?.Cleanup();

            if (menuBtn != null)
            {
                menuBtn.clicked -= OnMenuBtnClicked;
            }
        }

        private void OnMenuBtnClicked()
        {
            mainMenuContainer.style.display =
                mainMenuContainer.style.display == DisplayStyle.None
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
        }
    }
}
