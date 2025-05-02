using Colyseus;
using Cysharp.Threading.Tasks;
using Etheron.Colyseus.Schemas;
using System;
using UnityEngine;
namespace Etheron.Colyseus
{
    public class ColyseusManager : MonoBehaviour
    {

        [SerializeField] private string serverEndpoint = "ws://localhost:2567";
        private bool _isLoggedIn;
        public ColyseusRoom<MapV1State> currentMapRoom { get; private set; }

        public ColyseusClient client { get; private set; }
        public static ColyseusManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(obj: gameObject);
                return;
            }
            Instance = this;
            // DontDestroyOnLoad(target: gameObject);

            client = new ColyseusClient(endpoint: serverEndpoint);
        }

        public async UniTask SignInAsync(string email, string password)
        {
            if (_isLoggedIn) return;
            WhenLoading(isLoading: true);

            try
            {
                await client.Auth
                    .SignInWithEmailAndPassword(email: email, password: password);
                _isLoggedIn = true;
                Debug.Log(message: "[Auth] Signed in successfully");
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning(message: "[Auth] SignIn was canceled.");
            }
            catch (Exception ex)
            {
                Debug.LogError(message: $"[Auth] SignIn failed: {ex}");
            }
            finally
            {
                WhenLoading(isLoading: false);
            }
        }

        public async UniTask EnterMapV1(string mapId)
        {
            if (currentMapRoom != null)
            {
                Debug.Log(message: "[ColyseusManager.EnterMap] Leaving current map before entering a new one");
                await currentMapRoom.Leave();
            }

            try
            {
                ColyseusMatchMakeResponse reservation = await client.Http.Get<ColyseusMatchMakeResponse>(uriPath: "map-maker/enter?mapId=" + mapId);
                Debug.Log(message: "[ColyseusManager.EnterMap] Got reservation");
                currentMapRoom = await client.ConsumeSeatReservation<MapV1State>(response: reservation);
                Debug.Log(message: "[ColyseusManager.EnterMap] joined map successfully");

            }
            catch (Exception ex)
            {
                Debug.Log(message: "join error");
                Debug.Log(message: ex.Message);
            }
        }

        private void WhenLoading(bool isLoading)
        {
            Debug.Log(message: $"Loading Colyseus: {isLoading}");
        }
    }
}
