using Etheron.Core.XComponent;
using UnityEngine;
namespace Etheron.Colyseus.Components.Map.ServerClient.Player.ServerSync
{
    [RequireComponent(requiredComponent: typeof(ColyseusRoomEntity))]
    internal class ServerPlayersAuthoring : XCompAuthoring
    {
        [SerializeField] private int pollingIntervalMs = 100;
        [SerializeField] private GameObject playerPrefab;
        protected override void Authoring()
        {
            AddComponentData(component: new ServerPlayersCompData
            {
                pollingIntervalMs = pollingIntervalMs,
                playerPrefab = playerPrefab
            });
            AddSystem(system: new ServerPlayersCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
