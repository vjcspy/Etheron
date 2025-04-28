using Etheron.Core.XMachine;
namespace Etheron.Core.Component
{
    public class XComponent
    {
        public interface ICompData
        {
        }

        public abstract class XCompSystem
        {
            protected readonly XMachineEntity _xMachineEntity;
            protected XCompSystem(XMachineEntity xMachineEntity)
            {
                _xMachineEntity = xMachineEntity;
            }
            public abstract void Start();
            public abstract void Update();
            public abstract void Stop();
        }
    }
}
