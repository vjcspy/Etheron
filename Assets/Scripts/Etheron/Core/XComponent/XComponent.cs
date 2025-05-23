﻿using Etheron.Core.XMachine;
using Etheron.Utils;
using System;
using System.Collections.Generic;
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
        public abstract void OnCreate();
        public abstract void Update();
        public abstract void OnDestroy();

        protected XCompStorage<T> GetStorage<T>() where T : struct
        {
            return _xMachineEntity.GetOrCreateStorage<T>();
        }

        protected T GetComponent<T>()
        {
            return _xMachineEntity.GetComponent<T>();
        }
    }

    [RequireComponent(requiredComponent: typeof(XMachineEntity))]
    public abstract class XCompAuthoring : MonoBehaviour
    {
        private XCompSystem _system;
        protected XMachineEntity xMachineEntity;
        private void Awake()
        {
            xMachineEntity = GetComponent<XMachineEntity>();
            if (xMachineEntity == null)
            {
                Debug.LogError(message: "XCompAuthoring must be attached to a GameObject with an XMachineEntity component.");
            }
            Authoring();
        }

        protected void AddComponentData<T>(T component) where T : struct
        {
            xMachineEntity.AddComponentData(component: component);
        }

        protected void AddSystem(XCompSystem system)
        {
            xMachineEntity.AddSystem(system: system);
        }

        protected abstract void Authoring();
    }

    public abstract class XEntity : MonoBehaviour
    {
        private readonly Dictionary<Type, object> _storages = new Dictionary<Type, object>();
        private readonly XCompSystemArray _xCompSystems = new XCompSystemArray();

        #region Component Storage

        public void AddComponentData<T>(T component) where T : struct
        {
            Type type = typeof(T);
            if (!_storages.TryGetValue(key: type, value: out object storageObj))
            {
                var newStorage = new XCompStorage<T>(value: component);
                newStorage.Enable();
                _storages[key: type] = newStorage;
            }
            else
            {
                var storage = (XCompStorage<T>)storageObj;
                T merged = StructMerger.Merge(primary: storage.Get(), secondary: component);
                storage.Set(value: merged);
                storage.Enable();
            }
        }

        public XCompStorage<T> GetOrCreateStorage<T>() where T : struct
        {
            Type type = typeof(T);
            if (!_storages.TryGetValue(key: type, value: out object storageObj))
            {
                var newStorage = new XCompStorage<T>();
                _storages[key: type] = newStorage;
                return newStorage;
            }

            return (XCompStorage<T>)storageObj;
        }

        public XCompStorage<T> GetStorage<T>() where T : struct
        {
            return GetOrCreateStorage<T>();
        }

        public bool HasComponent<T>() where T : struct
        {
            Type type = typeof(T);
            if (_storages.TryGetValue(key: type, value: out object storageObj))
            {
                return ((XCompStorage<T>)storageObj).IsEnable();
            }
            return false;
        }

        public void DisableComponent<T>() where T : struct
        {
            Type type = typeof(T);
            if (_storages.TryGetValue(key: type, value: out object storageObj))
            {
                ((XCompStorage<T>)storageObj).Disable();
            }
        }
        public void EnableComponent<T>() where T : struct
        {
            Type type = typeof(T);
            if (_storages.TryGetValue(key: type, value: out object storageObj))
            {
                ((XCompStorage<T>)storageObj).Enable();
            }
        }

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
        //
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

        public IEnumerable<object> GetAllComponents()
        {
            return _storages.Values;
        }
        public void AddSystem(XCompSystem system)
        {
            if (system == null) return;
            _xCompSystems.Add(system: system);
            system.OnCreate();
        }

        #endregion

        #region Lifecycle

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
                _xCompSystems[index: i].OnDestroy();
            }
        }

        #endregion

    }
}
