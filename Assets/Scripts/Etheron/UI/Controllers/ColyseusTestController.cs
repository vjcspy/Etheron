using Colyseus.Schema;
using Cysharp.Threading.Tasks;
using Etheron.Colyseus;
using Etheron.Colyseus.Schemas;
using Etheron.UI.ToolkitBase;
using UnityEngine;
using UnityEngine.UIElements;
namespace Etheron.UI.Controllers
{
    [CreateAssetMenu(menuName = "UI Toolkit/Controllers/ColyseusTestController")]
    public class ColyseusTestControllerAsset : UIControllerAssetBase
    {
        private StateCallbackStrategy<MapState> callbacks;
        private Button joinRoomButton;
        private Button loginButton;

        public override void Setup(VisualElement root)
        {
            VisualElement container = root.Q<VisualElement>(name: "Container");
            loginButton = container?.Q<Button>(name: "Login");
            joinRoomButton = container?.Q<Button>(name: "JoinRoom");

            if (loginButton != null) loginButton.clicked += OnLoginClicked;
            if (joinRoomButton != null) joinRoomButton.clicked += OnJoinRoomClicked;
        }
        private async void OnJoinRoomClicked()
        {
            Debug.Log(message: "OnJoinRoomClicked");
            ColyseusManager.Instance.EnterMapV1(mapId: "sandbox").Forget();
        }

        public override void Cleanup()
        {
            if (loginButton != null)
            {
                loginButton.clicked -= OnLoginClicked;
                loginButton = null;
            }
            if (joinRoomButton != null)
            {
                joinRoomButton.clicked -= OnJoinRoomClicked;
                joinRoomButton = null;
            }
        }
        private void OnLoginClicked()
        {
            ColyseusManager.Instance.SignInAsync(email: "test2@gmail.com", password: "test123456").Forget();
        }
    }
}
