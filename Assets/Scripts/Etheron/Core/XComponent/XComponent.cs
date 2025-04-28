using Etheron.Core.XMachine;
namespace Etheron.Core.Component
{
    public class XCompStorage<T> where T : struct
    {
        private bool _hasValue;
        private T _value;

        public XCompStorage()
        {
            _hasValue = false;
        }

        public XCompStorage(T value)
        {
            _value = value;
            _hasValue = true;
        }

        public void Set(T value)
        {
            _value = value;
            _hasValue = true;
        }

        public void Disable()
        {
            _hasValue = false;
        }

        public void Enable()
        {
            _hasValue = true;
        }

        public T Get()
        {
            return _value;
        }

        public bool IsEnable()
        {
            return _hasValue;
        }


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
