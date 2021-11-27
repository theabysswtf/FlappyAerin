using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Engine
{
    [RequireComponent(typeof(PlayerInput))]
    public class Engine : MonoBehaviour
    {
        IInputRouter _inputRouter;
        Player _player;
        InputActionMap _uiMap;
        InputActionMap _playerMap;

        void Start()
        {   
            _inputRouter = ServiceFactory.GetService<IInputRouter>();
            _player = ServiceFactory.GetService<Player>();
            
            _uiMap = _inputRouter.GetUIMap();
            _playerMap = _inputRouter.GetPlayerMap();

            // Connect to OnPause + OnResume
            _uiMap.FindAction("Resume").started += OnResume;
            _uiMap.FindAction("Escape").started += OnResume;
            _playerMap.FindAction("Pause").started += OnPause;
            _playerMap.FindAction("Escape").started += OnPause;
        }

        void OnPause(InputAction.CallbackContext context)
        {
            Debug.Log("Pausing!");
            _inputRouter.PushMap(_uiMap);
        }
        void OnResume(InputAction.CallbackContext context)
        {   
            Debug.Log("Playing!");
            _inputRouter.PopMap();
        }
    }
}