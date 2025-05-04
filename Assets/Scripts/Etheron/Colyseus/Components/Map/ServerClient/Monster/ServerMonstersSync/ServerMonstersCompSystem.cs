using Colyseus.Schema;
using Cysharp.Threading.Tasks;
using Etheron.Colyseus.Components.Map.ServerClient.Player.ServerPlayersSync;
using Etheron.Colyseus.Components.Map.ServerClient.Player.ServerPlayerVisualizationComp;
using Etheron.Colyseus.Schemas;
using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using Etheron.Utils;
using System.Collections.Generic;
using UnityEngine;
namespace Etheron.Colyseus.Components.Map.ServerClient.Monster.ServerMonstersSync
{
    public class ServerMonstersCompSystem : XCompSystem
    {
        private readonly Dictionary<string, GameObject> _monsters = new Dictionary<string, GameObject>();
        private ColyseusManager _colyseusManager;
        private ServerMonstersCompData _config;
        private bool _isRunning;
        private XCompStorage<ServerMonstersCompData> _serverMonsterStorage;
        private XCompStorage<ServerPlayersCompData> _serverPlayersCompStorage;
        public ServerMonstersCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity)
        {
        }
        public override void OnCreate()
        {
            _colyseusManager = ColyseusManager.Instance;
            _serverMonsterStorage = GetStorage<ServerMonstersCompData>();
            _serverPlayersCompStorage = GetStorage<ServerPlayersCompData>();
            _config = _serverMonsterStorage.Get();

            if (!_serverPlayersCompStorage.IsEnable()) return;
            _isRunning = true;
            WaitForMapRoomConnected().Forget();
        }
        public override void Update()
        {
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
            // callback.OnAdd(
            //     propertyExpression: state => state.players,
            //     handler: (sessionId, player) =>
            //     {
            //         ELogger.Log(message: $"[ServerPlayersCompSystem] Player added: {sessionId}");
            //
            //         if (_monsters.ContainsKey(key: sessionId)) return;
            //         ELogger.Log(message: "[ServerPlayersCompSystem] Creating new player GameObject with sessionId " + sessionId);
            //         GameObject playerGO = Object.Instantiate(original: _config.playerPrefab);
            //         XEntity xEntity = playerGO.GetComponent<XEntity>();
            //         if (xEntity != null)
            //         {
            //             xEntity.AddComponentData(component: new ServerPlayerVisualizationCompData
            //             {
            //                 sessionId = sessionId
            //             });
            //         }
            //         else
            //         {
            //             ELogger.Log(message: "[ServerPlayersCompSystem] XEntity not found in player prefab");
            //         }
            //
            //         playerGO.name = $"RemotePlayer_{sessionId}";
            //         _players[key: sessionId] = playerGO;
            //     }
            // );
            //
            // callback.OnRemove(
            //     propertyExpression: state => state.players,
            //     handler: (sessionId, player) =>
            //     {
            //         ELogger.Log(message: $"[ServerPlayersCompSystem] Player removed: {sessionId}");
            //
            //         if (_players.TryGetValue(key: sessionId, value: out GameObject go))
            //         {
            //             Object.Destroy(obj: go);
            //             _players.Remove(key: sessionId);
            //         }
            //     }
            // );
        }
        public override void OnDestroy()
        {
            _isRunning = false;

            foreach (GameObject go in _monsters.Values)
            {
                if (go != null)
                    Object.Destroy(obj: go);
            }

            _monsters.Clear();
        }
    }
}
