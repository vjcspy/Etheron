using Etheron.Core.XMachine;
using UnityEngine;
namespace Etheron.Core.XComponent
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
        public abstract void Enable();
        public abstract void Update();
        public abstract void Disable();
    }

    [RequireComponent(typeof(XMachineEntity))]
    public abstract class XCompAuthoring : MonoBehaviour
    {
        // private XMachineEntity xMachineEntity;
        private void Awake()
        {
            XMachineEntity xMachineEntity = GetComponent<XMachineEntity>();
            if (xMachineEntity == null)
            {
                Debug.LogError(message: "XCompAuthoring must be attached to a GameObject with an XMachineEntity component.");
                return;
            }

            // Add the component to the XMachineEntity
            Authoring(xMachineEntity: xMachineEntity);
        }

        protected abstract void Authoring(XMachineEntity xMachineEntity);
    }
}
