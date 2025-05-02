using Etheron.Core.XComponent;
using UnityEngine;
namespace Etheron.Colyseus.Components.Map.ClientServer.Player.LocalSync
{
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
