using Etheron.Core.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Etheron.Core.XMachine
{
    public class XMachineState
    {
        public XMachineState(Enum id)
        {
            this.id = id;
        }

        public Enum id { get; }

        internal virtual bool Guard()
        {
            return true;
        }
        internal virtual void Entry() { }

        internal virtual void Invoke() { }

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

        public XMachine Start(Enum initialStateId)
        {
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
        private readonly Dictionary<Type, XComponent.ICompData> _components = new Dictionary<Type, XComponent.ICompData>();
        private XComponent.XCompSystem[] _cachedCompSystems;
        public XMachine xMachine { get; private set; }

        public void SetComponent<T>(T component) where T : struct, XComponent.ICompData
        {
            _components[key: typeof(T)] = component;
        }

        public bool TryGetComponentData<T>(out T component) where T : struct, XComponent.ICompData
        {
            if (_components.TryGetValue(key: typeof(T), value: out XComponent.ICompData comp))
            {
                component = (T)comp;
                return true;
            }

            component = default(T);
            return false;
        }

        public bool HasComponent<T>() where T : struct, XComponent.ICompData
        {
            return _components.ContainsKey(key: typeof(T));
        }

        public bool RemoveComponent<T>() where T : struct, XComponent.ICompData
        {
            return _components.Remove(key: typeof(T));
        }

        public IEnumerable<XComponent.ICompData> GetAllComponents()
        {
            return _components.Values;
        }

        protected abstract XComponent.XCompSystem[] GetCompSystems();
        protected abstract XMachineState[] GetMachineStates();

        #region RendererCycle

        private void Awake()
        {
            _cachedCompSystems = GetCompSystems();
            xMachine = new XMachine().RegisterMachineStates(machineStates: GetMachineStates());
        }

        protected virtual void Start()
        {
            foreach (XComponent.XCompSystem t in _cachedCompSystems)
            {
                t.Start();
            }
        }

        private void Update()
        {
            foreach (XComponent.XCompSystem t in _cachedCompSystems)
            {
                t.Update();
            }
        }

        private void OnDestroy()
        {
            foreach (XComponent.XCompSystem t in _cachedCompSystems)
            {
                t.Stop();
            }
        }

        #endregion

    }
}
