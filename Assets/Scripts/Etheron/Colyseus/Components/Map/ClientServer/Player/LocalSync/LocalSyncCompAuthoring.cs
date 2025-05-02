using Etheron.Core.XComponent;
using Etheron.Gameplay.Character.Player.Common;
using UnityEngine;
namespace Etheron.Colyseus.Components.Map.ClientServer.Player.LocalSync
{
    /// <summary>
    ///     Dùng ở player local game object để sync local player data to server
    /// </summary>
    [RequireComponent(requiredComponent: typeof(PlayerEntity))]
    public class LocalSyncCompAuthoring : XCompAuthoring
    {
        [SerializeField] private int syncIntervalMs = 100;
        protected override void Authoring()
        {
            AddComponentData(component: new LocalSyncCompData
            {
                syncIntervalMs = syncIntervalMs
            });
            AddSystem(system: new LocalSyncCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
