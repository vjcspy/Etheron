using Colyseus;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
namespace Etheron.Colyseus
{
    public class ColyseusManager : MonoBehaviour
    {

        [SerializeField] private string serverEndpoint = "ws://localhost:2567";

        public ColyseusClient client { get; private set; }

        public static ColyseusManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(obj: gameObject);
                return;
            }
            Instance = this;
            // DontDestroyOnLoad(target: gameObject);

            client = new ColyseusClient(endpoint: serverEndpoint);
        }

        public async UniTask SignInAsync(string email, string password)
        {
            WhenLoading(isLoading: true);

            try
            {
                await client.Auth
                    .SignInWithEmailAndPassword(email: email, password: password);

                Debug.Log(message: "[Auth] Signed in successfully");
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning(message: "[Auth] SignIn was canceled.");
            }
            catch (Exception ex)
            {
                Debug.LogError(message: $"[Auth] SignIn failed: {ex}");
            }
            finally
            {
                WhenLoading(isLoading: false);
            }
        }

        private void WhenLoading(bool isLoading)
        {
            Debug.Log(message: $"Loading Colyseus: {isLoading}");
        }
    }
}
