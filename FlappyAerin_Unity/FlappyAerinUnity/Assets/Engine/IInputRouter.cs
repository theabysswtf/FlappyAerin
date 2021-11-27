using System;
using UnityEngine.InputSystem;

namespace Engine
{
    public interface IInputRouter : IService
    {
        public void PushMap(InputActionMap map);
        public InputActionMap PopMap();
        public void SetDefaultMap(InputActionMap map);
        public InputActionMap GetPlayerMap();
        public InputActionMap GetUIMap();
    }
}