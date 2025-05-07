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
        [SerializeField] private int mainThreadSyncIntervalMs = 50;
        [SerializeField] private int serverSyncIntervalMs = 25;
        protected override void Authoring()
        {
            AddComponentData(component: new LocalSyncCompData
            {
                mainThreadSyncIntervalMs = mainThreadSyncIntervalMs,
                serverSyncIntervalMs = serverSyncIntervalMs
            });
            AddSystem(system: new LocalSyncCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
