using System.Collections.Generic;
using UnityEngine;
namespace Etheron.Core.Manager
{
    public class ManagersInitializer : MonoBehaviour
    {
        private static bool hasInitialized;

        [SerializeField] private List<GameObject> managerPrefabs;

        private void Awake()
        {
            if (hasInitialized)
            {
                Destroy(obj: gameObject);
                return;
            }

            hasInitialized = true;

            foreach (GameObject prefab in managerPrefabs)
            {
                GameObject instance = Instantiate(original: prefab);
                DontDestroyOnLoad(target: instance);
            }

            DontDestroyOnLoad(target: gameObject);
        }
    }
}
