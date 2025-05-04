using Colyseus.Schema;
using Cysharp.Threading.Tasks;
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
        public ServerMonstersCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity)
        {
        }
        public override void OnCreate()
        {
            _colyseusManager = ColyseusManager.Instance;
            _serverMonsterStorage = GetStorage<ServerMonstersCompData>();
            _config = _serverMonsterStorage.Get();

            if (!_serverMonsterStorage.IsEnable()) return;

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

                await UniTask.Delay(millisecondsDelay: 1000);
            }
        }
        private void RegisterCallbacks(StateCallbackStrategy<MapV1State> callback)
        {
            callback.OnAdd(
                propertyExpression: state => state.monsters,
                handler: (sessionId, monster) =>
                {
                    ELogger.Log(message: $"[ServerMonstersCompSystem] Monster added: {sessionId}");

                    if (_monsters.ContainsKey(key: sessionId)) return;
                    ELogger.Log(message: "[ServerMonstersCompSystem] Creating new monster GameObject with sessionId " + sessionId);
                    GameObject monsterPrefab = _config.monsterDatabase.GetMonsterPrefab(id: monster.id);
                    if (monsterPrefab == null)
                    {
                        ELogger.Log(message: "[ServerMonstersCompSystem] Monster prefab not found");
                        return;
                    }
                    GameObject monsterGO = Object.Instantiate(original: monsterPrefab);
                    XEntity xEntity = monsterGO.GetComponent<XEntity>();
                    if (xEntity != null)
                    {
                        // xEntity.AddComponentData(component: new ServerPlayerVisualizationCompData
                        // {
                        //     sessionId = sessionId
                        // });
                    }
                    else
                    {
                        ELogger.Log(message: "[ServerMonstersCompSystem] XEntity not found in player prefab");
                    }

                    monsterGO.name = $"ServerMonster_{sessionId}";
                    _monsters[key: sessionId] = monsterGO;
                }
            );

            callback.OnRemove(
                propertyExpression: state => state.monsters,
                handler: (sessionId, monster) =>
                {
                    ELogger.Log(message: $"[ServerMonstersCompSystem] Monster removed: {sessionId}");

                    if (_monsters.TryGetValue(key: sessionId, value: out GameObject go))
                    {
                        Object.Destroy(obj: go);
                        _monsters.Remove(key: sessionId);
                    }
                }
            );
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
