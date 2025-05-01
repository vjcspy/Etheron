using Etheron.Core.XComponent;
using Etheron.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public virtual void OnCreate() { }

        internal virtual bool Guard()
        {
            return true;
        }
        internal virtual void Entry() { }

        internal virtual void Exit() { }
    }

    public class XMachine
    {
        private Dictionary<int, XMachineState> _states;
        public int currentStateId;
        public XMachine RegisterMachineStates(XMachineState[] machineStates)
        {
            _states = machineStates.ToDictionary(keySelector: state =>
            {
                state.OnCreate();

                return state.id;
            });

            return this;
        }

        public XMachine Start(Enum initialStateId = null)
        {
            if (initialStateId == null && (_states == null || _states.Count == 0))
            {
                return this;
            }

            int id = initialStateId == null ? _states.First().Value.id : EnumUtility.ToIntFast(enumValue: initialStateId);

            currentStateId = id;
            GetCurrentState().Entry();

            return this;
        }

        public XMachine Transition(int toStateId)
        {
            if (currentStateId == toStateId)
            {
                // TODO: Currently, we don't allow self transition, so only need to check if the state is different
                return this;
            }
            if (!_states[key: toStateId].Guard())
            {
                return this;
            }

            GetCurrentState().Exit();
            currentStateId = toStateId;
            _states[key: currentStateId].Entry();
            return this;
        }

        public XMachineState GetCurrentState()
        {
            return _states[key: currentStateId];
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

    public abstract class XMachineEntity : XEntity
    {
        public XMachine xMachine { get; } = new XMachine();

        protected virtual void Awake()
        {
            var states = GetXMachineStates();
            if (states.Length > 0)
            {
                xMachine.RegisterMachineStates(machineStates: GetXMachineStates());
            }
        }

        protected virtual void Start()
        {
            xMachine?.Start(initialStateId: null);
        }
        protected abstract XMachineState[] GetXMachineStates();
    }
}
