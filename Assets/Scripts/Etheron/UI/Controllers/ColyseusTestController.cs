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
        private StateCallbackStrategy<SandboxRoomState> callbacks;
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
            var room = await ColyseusManager.Instance.client.JoinOrCreate<SandboxRoomState>(roomName: "sandbox");

            // get state callbacks handler
            if (callbacks == null)
            {
                room.OnMessage<string>(type: "player_left", handler: message =>
                {
                    Debug.Log(message: message);
                });

                callbacks = Callbacks.Get(room: room);

                callbacks.OnAdd(propertyExpression: state => state.players, handler: (sessionId, player) =>
                {
                    // ...
                    Debug.Log(message: "entity added id " + player.id + " sessionId " + sessionId);

                    callbacks.Listen(instance: player, propertyExpression: entity => entity.position, handler: (currentPos, _) =>
                    {
                        Debug.Log(message: "player " + sessionId + "changed position to " + currentPos.x);
                    });
                });
            }

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
