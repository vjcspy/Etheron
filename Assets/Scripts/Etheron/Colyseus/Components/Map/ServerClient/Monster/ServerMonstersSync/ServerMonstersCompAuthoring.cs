using Etheron.Core.XComponent;
using Etheron.Gameplay.Character.Monster;
using UnityEngine;
namespace Etheron.Colyseus.Components.Map.ServerClient.Monster.ServerMonstersSync
{
    public class ServerMonstersCompAuthoring : XCompAuthoring
    {
        [SerializeField] private int pollingIntervalMs = 100;
        [SerializeField] private MonsterDatabase monsterDatabase;
        protected override void Authoring()
        {
            AddComponentData(component: new ServerMonstersCompData
            {
                monsterDatabase = monsterDatabase,
                pollingIntervalMs = pollingIntervalMs
            });
            AddSystem(system: new ServerMonstersCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
