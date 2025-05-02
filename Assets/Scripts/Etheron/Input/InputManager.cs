using Etheron.Core.Manager;
namespace Etheron.Input
{
    public class InputManager : ManagerBase
    {
        public static InputManager Instance { get; private set; }
        public InputSystem_Actions InputActions { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(obj: gameObject);
                return;
            }

            Instance = this;
            InputActions = new InputSystem_Actions();
        }
    }
}
