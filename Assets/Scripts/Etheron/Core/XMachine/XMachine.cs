using Etheron.Core.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Etheron.Core.XMachine
{
    public class XMachineState
    {
        protected readonly XMachineEntity _xMachineEntity;
        public XMachineState(Enum id, XMachineEntity xMachineEntity)
        {
            _xMachineEntity = xMachineEntity;
            this.id = id;
        }
        public Enum id { get; }

        internal virtual bool Guard()
        {
            return true;
        }
        internal virtual void Entry() { }

        internal virtual void Exit() { }
    }

    public class XMachine
    {
        private Enum _currentStateId;
        private Dictionary<Enum, XMachineState> _states;
        public XMachine RegisterMachineStates(XMachineState[] machineStates)
        {
            _states = machineStates.ToDictionary(keySelector: state => state.id);

            return this;
        }

        public XMachine Start(Enum initialStateId = null)
        {
            initialStateId ??= _states.First().Value.id;

            _currentStateId = initialStateId;
            GetCurrentState().Entry();

            return this;
        }

        public XMachine Transition(Enum toStateId)
        {
            if (Equals(objA: toStateId, objB: _currentStateId))
            {
                // TODO: Currently, we don't allow self transition, so only need to check if the state is different
                return this;
            }
            if (!_states[key: toStateId].Guard())
            {
                return this;
            }

            GetCurrentState().Exit();
            _states[key: toStateId].Entry();

            return this;
        }

        public XMachineState GetCurrentState()
        {
            return _states[key: _currentStateId];
        }
    }
    public abstract class XMachineEntity : MonoBehaviour
    {
        private readonly Dictionary<Type, object> _components = new Dictionary<Type, object>();
        private XCompSystem[] _cachedCompSystems;
        public XMachine xMachine { get; private set; }

        // Add component lần đầu tiên
        public void AddXComponent<T>(T component) where T : struct
        {
            Type type = typeof(T);
            if (!_components.TryGetValue(key: type, value: out object storageObj))
            {
                var newStorage = new XCompStorage<T>(value: component);
                newStorage.Enable();
                _components[key: type] = newStorage;
            }
            else
            {
                ((XCompStorage<T>)storageObj).Enable();
            }
        }

        // Set/update component. Should use from cached storage
        // public void SetXComponent<T>(T component) where T : struct
        // {
        //     Type type = typeof(T);
        //     if (_components.TryGetValue(key: type, value: out object storageObj))
        //     {
        //         ((XCompStorage<T>)storageObj).Set(value: component);
        //     }
        //     else
        //     {
        //         _components[key: type] = new XCompStorage<T>(value: component);
        //     }
        // }

        // Try get component data. Should use from cached storage
        // public bool TryGetXComponent<T>(out T component) where T : struct
        // {
        //     Type type = typeof(T);
        //     if (_components.TryGetValue(key: type, value: out object storageObj))
        //     {
        //         var storage = (XCompStorage<T>)storageObj;
        //         if (storage.IsEnable())
        //         {
        //             component = storage.Get();
        //             return true;
        //         }
        //     }
        //
        //     component = default(T);
        //     return false;
        // }

        // Get ref trực tiếp tới Storage
        public XCompStorage<T> GetOrCreateXStorage<T>() where T : struct
        {
            Type type = typeof(T);
            if (!_components.TryGetValue(key: type, value: out object storageObj))
            {
                var newStorage = new XCompStorage<T>();
                _components[key: type] = newStorage;
                return newStorage;
            }

            return (XCompStorage<T>)storageObj;
        }

        // Check xem có component và nó còn hợp lệ không
        public bool HasXComponent<T>() where T : struct
        {
            Type type = typeof(T);
            if (_components.TryGetValue(key: type, value: out object storageObj))
            {
                return ((XCompStorage<T>)storageObj).IsEnable();
            }
            return false;
        }

        // Invalidate component thay vì Remove
        public void DisableXComponent<T>() where T : struct
        {
            Type type = typeof(T);
            if (_components.TryGetValue(key: type, value: out object storageObj))
            {
                ((XCompStorage<T>)storageObj).Disable();
            }
        }

        // Get toàn bộ component storage
        public IEnumerable<object> GetAllXComponents()
        {
            return _components.Values;
        }

        protected abstract XCompSystem[] GetXCompSystems();
        protected abstract XMachineState[] GetXMachineStates();

        #region RendererCycle

        protected virtual void Awake()
        {
            _cachedCompSystems = GetXCompSystems();
            xMachine = new XMachine().RegisterMachineStates(machineStates: GetXMachineStates());
            Authoring();
        }

        protected abstract void Authoring();

        protected virtual void Start()
        {
            xMachine.Start(initialStateId: null);
            foreach (XCompSystem t in _cachedCompSystems)
            {
                t.Start();
            }
        }

        private void Update()
        {
            foreach (XCompSystem t in _cachedCompSystems)
            {
                t.Update();
            }
        }

        private void OnDestroy()
        {
            foreach (XCompSystem t in _cachedCompSystems)
            {
                t.Stop();
            }
        }

        #endregion

    }
}
