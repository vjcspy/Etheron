using Colyseus.Schema;
using Cysharp.Threading.Tasks;
using Etheron.Colyseus.Components.Map.ServerClient.Player.VisualizationComp;
using Etheron.Colyseus.Schemas;
using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using Etheron.Utils;
using System.Collections.Generic;
using UnityEngine;
namespace Etheron.Colyseus.Components.Map.ServerClient.Player.ServerSync
{
    internal class ServerPlayersCompSystem : XCompSystem
    {
        private readonly Dictionary<string, GameObject> _players = new Dictionary<string, GameObject>();
        private ColyseusManager _colyseusManager;
        private ServerPlayersCompData _config;
        private bool _isRunning;
        private XCompStorage<ServerPlayersCompData> _serverPlayersCompStorage;

        public ServerPlayersCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity) { }

        public override void OnCreate()
        {
            _colyseusManager = ColyseusManager.Instance;
            _serverPlayersCompStorage = GetStorage<ServerPlayersCompData>();
            _config = _serverPlayersCompStorage.Get();

            if (!_serverPlayersCompStorage.IsEnable()) return;

            _isRunning = true;
            WaitForMapRoomConnected().Forget();
        }

        private async UniTaskVoid WaitForMapRoomConnected()
        {
            while (_isRunning)
            {
                if (_colyseusManager.currentMapRoomCallback != null)
                {
                    RegisterCallbacks(callback: _colyseusManager.currentMapRoomCallback);
                    break;
                }

                await UniTask.Delay(millisecondsDelay: _config.pollingIntervalMs); // Kiểm tra mỗi 100ms
            }
        }

        private void RegisterCallbacks(StateCallbackStrategy<MapV1State> callback)
        {
            callback.OnAdd(
                propertyExpression: state => state.players,
                handler: (sessionId, player) =>
                {
                    if (sessionId == _colyseusManager.currentMapRoom.SessionId)
                    {
                        ELogger.Log(message: "[ServerPlayersCompSystem] Skip adding self player");
                        return;
                    }

                    ELogger.Log(message: $"[ServerPlayersCompSystem] Player added: {sessionId}");

                    if (_players.ContainsKey(key: sessionId)) return;
                    ELogger.Log(message: "[ServerPlayersCompSystem] Creating new player GameObject with sessionId " + sessionId);
                    GameObject playerGO = Object.Instantiate(original: _config.playerPrefab);
                    XEntity xEntity = playerGO.GetComponent<XEntity>();
                    if (xEntity != null)
                    {
                        xEntity.AddComponentData(component: new ServerPlayerVisualizationCompData
                        {
                            sessionId = sessionId
                        });
                    }
                    else
                    {
                        ELogger.Log(message: "[ServerPlayersCompSystem] XEntity not found in player prefab");
                    }

                    playerGO.name = $"RemotePlayer_{sessionId}";
                    _players[key: sessionId] = playerGO;
                }
            );

            callback.OnRemove(
                propertyExpression: state => state.players,
                handler: (sessionId, player) =>
                {
                    ELogger.Log(message: $"[ServerPlayersCompSystem] Player removed: {sessionId}");

                    if (_players.TryGetValue(key: sessionId, value: out GameObject go))
                    {
                        Object.Destroy(obj: go);
                        _players.Remove(key: sessionId);
                    }
                }
            );
        }

        public override void Update() { }

        public override void OnDestroy()
        {
            _isRunning = false;

            foreach (GameObject go in _players.Values)
            {
                if (go != null)
                    Object.Destroy(obj: go);
            }

            _players.Clear();
        }
    }
}
