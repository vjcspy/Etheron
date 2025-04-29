using Etheron.Core.Component;
using Etheron.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Etheron.Core.XMachine
{
    public class XMachineState
    {
        protected readonly XMachineEntity _xMachineEntity;
        protected XMachineState(int id, XMachineEntity xMachineEntity)
        {
            _xMachineEntity = xMachineEntity;
            this.id = id;
        }
        public int id { get; }

        internal virtual bool Guard()
        {
            return true;
        }
        internal virtual void Entry() { }

        internal virtual void Exit() { }
    }

    public class XMachine
    {
        private int _currentStateId;
        private Dictionary<int, XMachineState> _states;
        public XMachine RegisterMachineStates(XMachineState[] machineStates)
        {
            _states = machineStates.ToDictionary(keySelector: state => state.id);

            return this;
        }

        public XMachine Start(Enum initialStateId = null)
        {
            int id = initialStateId == null ? _states.First().Value.id : EnumUtility.ToIntFast(enumValue: initialStateId);

            _currentStateId = id;
            GetCurrentState().Entry();

            return this;
        }

        public XMachine Transition(int toStateId)
        {
            if (_currentStateId == toStateId)
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

    public class XCompSystemArray
    {
        private XCompSystem[] _systems;

        public XCompSystemArray(int initialCapacity = 8)
        {
            _systems = new XCompSystem[initialCapacity];
            Count = 0;
        }

        public int Count { get; private set; }

        public XCompSystem this[int index]
        {
            get
            {
                // if (index >= _count) throw new IndexOutOfRangeException();
                return _systems[index];
            }
        }

        public void Add(XCompSystem system)
        {
            if (Count == _systems.Length)
            {
                Resize(newSize: _systems.Length + 8);
            }

            _systems[Count++] = system;
        }

        public void Remove(XCompSystem system)
        {
            for (int i = 0; i < Count; i++)
            {
                if (_systems[i] == system)
                {
                    _systems[i] = _systems[Count - 1]; // swap last vào
                    _systems[Count - 1] = null;
                    Count--;
                    return;
                }
            }
        }

        private void Resize(int newSize)
        {
            var newArray = new XCompSystem[newSize];
            Array.Copy(sourceArray: _systems, destinationArray: newArray, length: Count);
            _systems = newArray;
        }

        public void ForEach(Action<XCompSystem> action)
        {
            for (int i = 0; i < Count; i++)
            {
                action(obj: _systems[i]);
            }
        }
    }

    public abstract class XMachineEntity : MonoBehaviour
    {
        private readonly Dictionary<Type, object> _components = new Dictionary<Type, object>();
        private readonly XCompSystemArray _xCompSystems = new XCompSystemArray();
        public XMachine xMachine { get; private set; }

        #region Component Storage

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

        public bool HasXComponent<T>() where T : struct
        {
            Type type = typeof(T);
            if (_components.TryGetValue(key: type, value: out object storageObj))
            {
                return ((XCompStorage<T>)storageObj).IsEnable();
            }
            return false;
        }

        public void DisableXComponent<T>() where T : struct
        {
            Type type = typeof(T);
            if (_components.TryGetValue(key: type, value: out object storageObj))
            {
                ((XCompStorage<T>)storageObj).Disable();
            }
        }

        // Try get component data. Should use from cached storage
        public bool TryGetXComponent<T>(out T component) where T : struct
        {
            Type type = typeof(T);
            if (_components.TryGetValue(key: type, value: out object storageObj))
            {
                var storage = (XCompStorage<T>)storageObj;
                if (storage.IsEnable())
                {
                    component = storage.Get();
                    return true;
                }
            }

            component = default(T);
            return false;
        }

        // Set/update component. Should use from cached storage
        public void SetXComponent<T>(T component) where T : struct
        {
            Type type = typeof(T);
            if (_components.TryGetValue(key: type, value: out object storageObj))
            {
                ((XCompStorage<T>)storageObj).Set(value: component);
            }
            else
            {
                _components[key: type] = new XCompStorage<T>(value: component);
            }
        }

        public IEnumerable<object> GetAllXComponents()
        {
            return _components.Values;
        }

        #endregion

        #region System Management

        // protected abstract XCompSystem[] GetXCompSystems();
        protected abstract XMachineState[] GetXMachineStates();

        // Cho phép đăng ký thêm XCompSystem trong runtime
        public void RegisterXCompSystem(XCompSystem system)
        {
            if (system == null) return;
            _xCompSystems.Add(system: system);
            system.Start();
        }

        protected virtual void Awake()
        {
            xMachine = new XMachine().RegisterMachineStates(machineStates: GetXMachineStates());
        }

        // protected abstract void Authoring();

        protected virtual void Start()
        {
            xMachine.Start(initialStateId: null);
        }

        private void Update()
        {
            for (int i = 0; i < _xCompSystems.Count; i++)
            {
                _xCompSystems[index: i].Update();
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _xCompSystems.Count; i++)
            {
                _xCompSystems[index: i].Stop();
            }
        }

        #endregion

    }
}
