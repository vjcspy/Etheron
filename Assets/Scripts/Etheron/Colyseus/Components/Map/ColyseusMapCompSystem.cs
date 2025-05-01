using Colyseus;
using Colyseus.Schema;
using Cysharp.Threading.Tasks;
using Etheron.Colyseus.Schemas;
using Etheron.Core.XComponent;
using Etheron.Core.XMachine;
using UnityEngine;
namespace Etheron.Colyseus.Components.Map
{
    public class ColyseusMapCompSystem : XCompSystem
    {
        private StateCallbackStrategy<MapState> _callbacks;
        private ColyseusClient _client;
        private XCompStorage<ColyseusMapCompData> _colyseusMapCompStorage;
        private ColyseusRoom<MapState> _room;

        public ColyseusMapCompSystem(XMachineEntity xMachineEntity) : base(xMachineEntity: xMachineEntity)
        {
        }
        public override void OnCreate()
        {
            _colyseusMapCompStorage = GetStorage<ColyseusMapCompData>();
            _client = ColyseusManager.Instance.client;
            // JoinRoom(colyseusMapCompData: _colyseusMapCompStorage.Get()).Forget();
        }

        private async UniTaskVoid JoinRoom(ColyseusMapCompData colyseusMapCompData)
        {
            await ColyseusManager.Instance.SignInAsync(email: "test2@gmail.com", password: "test123456");
            _room = await _client.JoinOrCreate<MapState>(roomName: colyseusMapCompData.mapName);
            _callbacks = Callbacks.Get(room: _room);

            _callbacks.OnAdd(propertyExpression: state => state.players, handler: (sessionId, player) =>
            {
                Debug.Log(message: "entity added id " + player.id + " sessionId " + sessionId);

                _callbacks.Listen(instance: player, propertyExpression: entity => entity.position, handler: (currentPos, _) =>
                {
                    Debug.Log(message: "player " + sessionId + "changed position to " + currentPos.x);
                });
            });
        }

        public override void Update()
        {
            if (!_colyseusMapCompStorage.IsEnable()) return;
        }
        public override void OnDestroy()
        {
        }
    }
}
