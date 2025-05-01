using Etheron.Core.XComponent;
namespace Etheron.Gameplay.Character.Player.Common.Components.InputComp
{
    public class InputCompAuthoring : XCompAuthoring
    {
        protected override void Authoring()
        {
            AddComponentData(
                component: new InputCompData());
            AddSystem(system: new InputCompSystem(xMachineEntity: xMachineEntity));
        }
    }
}
