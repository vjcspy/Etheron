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
        [SerializeField] private string mapId = "sandbox";
        [SerializeField] private string email = "test1@gmail.com";

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
        private void OnJoinRoomClicked()
        {
            Debug.Log(message: "OnJoinRoomClicked");
            ColyseusManager.Instance.EnterMapV1(mapId: mapId).Forget();
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
            ColyseusManager.Instance.SignInAsync(email: email, password: "test123456").Forget();
        }
    }
}
