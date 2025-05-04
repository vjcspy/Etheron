using System;
using System.Collections.Generic;
using UnityEngine;
namespace Etheron.Gameplay.Character.Monster
{
    [CreateAssetMenu(fileName = "MonsterDatabase", menuName = "Game/Monster Database")]
    public class MonsterDatabase : ScriptableObject
    {

        public List<MonsterEntry> monsters;

        private Dictionary<string, GameObject> _lookup;

        public GameObject GetMonsterPrefab(string id)
        {
            if (_lookup == null)
            {
                _lookup = new Dictionary<string, GameObject>();
                foreach (MonsterEntry entry in monsters)
                {
                    _lookup[key: entry.id] = entry.prefab;
                }
            }

            _lookup.TryGetValue(key: id, value: out GameObject prefab);
            return prefab;
        }
        [Serializable]
        public class MonsterEntry
        {
            public string id;
            public GameObject prefab;
        }
    }
}
