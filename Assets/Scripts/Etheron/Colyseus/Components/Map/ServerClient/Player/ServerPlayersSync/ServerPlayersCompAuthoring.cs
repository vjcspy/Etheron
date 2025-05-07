using Etheron.Core.XComponent;
using UnityEngine;
namespace Etheron.Colyseus.Components.Map.ServerClient.Player.ServerPlayersSync
{
    [RequireComponent(requiredComponent: typeof(ColyseusRoomEntity))]
    internal class ServerPlayersAuthoring : XCompAuthoring
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private bool debugLocalPlayer = true;
        protected override void Authoring()
        {
            AddComponentData(component: new ServerPlayersCompData
            {
                playerPrefab = playerPrefab,
                debugLocalPlayer = debugLocalPlayer
            });
            AddSystem(system: new ServerPlayersCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
