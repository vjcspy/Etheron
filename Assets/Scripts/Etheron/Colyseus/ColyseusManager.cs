using Colyseus;
using Colyseus.Schema;
using Cysharp.Threading.Tasks;
using Etheron.Colyseus.Schemas;
using Etheron.Core.Manager;
using Etheron.Utils;
using System;
using UnityEngine;
namespace Etheron.Colyseus
{
    public class ColyseusManager : ManagerBase
    {

        [SerializeField] private string serverEndpoint = "ws://localhost:2567";
        private bool _isLoggedIn;
        private string _lastMapId;
        private bool _needsReconnect;
        public StateCallbackStrategy<MapV1State> currentMapRoomCallback { get; private set; }
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
                ELogger.Log(message: "[Auth] Signed in successfully");
            }
            catch (OperationCanceledException)
            {
                ELogger.LogWarning(message: "[Auth] SignIn was canceled.");
            }
            catch (Exception ex)
            {
                ELogger.LogError(message: $"[Auth] SignIn failed: {ex}");
            }
            finally
            {
                WhenLoading(isLoading: false);
            }
        }

        public async UniTask EnterMapV1(string mapId)
        {
            _lastMapId = mapId;
            if (currentMapRoom != null)
            {
                ELogger.Log(message: "[ColyseusManager.EnterMap] Leaving current map before entering a new one");
                await currentMapRoom.Leave();
                CleanupCurrentMapRoom();
            }

            try
            {
                ColyseusMatchMakeResponse reservation = await client.Http.Get<ColyseusMatchMakeResponse>(uriPath: "map-maker/enter?mapId=" + mapId);
                ELogger.Log(message: "[ColyseusManager.EnterMap] Got reservation");
                currentMapRoom = await client.ConsumeSeatReservation<MapV1State>(response: reservation);
                currentMapRoomCallback = Callbacks.Get(room: currentMapRoom);
                ELogger.Log(message: "[ColyseusManager.EnterMap] joined map successfully");
                RegisterRoomDisconnectEvents(room: currentMapRoom);
            }
            catch (Exception ex)
            {
                ELogger.Log(message: "join error");
                ELogger.Log(message: ex.Message);
            }
        }

        private void WhenLoading(bool isLoading)
        {
            ELogger.Log(message: $"Loading Colyseus: {isLoading}");
        }

        private void RegisterRoomDisconnectEvents(ColyseusRoom<MapV1State> room)
        {
            room.OnLeave += HandleRoomLeave;
            room.OnError += HandleRoomError;
        }
        private void HandleRoomError(int code, string message)
        {
            ELogger.LogWarning(message: $"[ColyseusManager] Room error (code={code}): {message}");
            // CleanupCurrentMapRoom();
            // _needsReconnect = true;
            // TryReconnectAsync().Forget();
        }

        private void HandleRoomLeave(int code)
        {
            ELogger.LogWarning(message: $"[ColyseusManager] Room left (code={code}). Resetting state.");
            CleanupCurrentMapRoom();
        }

        private void CleanupCurrentMapRoom()
        {
            currentMapRoom = null;
            currentMapRoomCallback = null;
        }

        private async UniTaskVoid TryReconnectAsync()
        {
            int retryCount = 0;
            while (_needsReconnect && retryCount < 5)
            {
                await UniTask.Delay(millisecondsDelay: 3000); // chờ 3 giây

                ELogger.Log(message: $"[Reconnect] Attempt #{retryCount + 1}");
                try
                {
                    await EnterMapV1(mapId: _lastMapId); // bạn cần lưu lại mapId gần nhất
                    _needsReconnect = false;
                    ELogger.Log(message: "[Reconnect] Success");
                    break;
                }
                catch
                {
                    retryCount++;
                    ELogger.LogWarning(message: "[Reconnect] Failed, will retry...");
                }
            }

            if (_needsReconnect)
            {
                ELogger.LogError(message: "[Reconnect] All retries failed.");
                // TODO: Show UI cho người dùng chọn reconnect thủ công
            }
        }
    }
}
