using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Engine
{
    [RequireComponent(typeof(PlayerInput))]
    public class Engine : MonoBehaviour
    {
        IInputService _inputService;
        InputActionMap _uiMap;
        InputActionMap _playerMap;

        void Start()
        {
            _inputService = ServiceFactory.GetService<IInputService>();
            
            _uiMap = _inputService.GetUIMap();
            _playerMap = _inputService.GetPlayerMap();

            // Connect to OnPause + OnResume
            _uiMap.FindAction("Resume").started += OnResume;
            _uiMap.FindAction("Escape").started += OnResume;
            _playerMap.FindAction("Pause").started += OnPause;
            _playerMap.FindAction("Escape").started += OnPause;
            Time.timeScale = 0;
        }

        void OnPause(InputAction.CallbackContext context)
        {
            Time.timeScale = 0;
            _inputService.PopMap();
        }
        void OnResume(InputAction.CallbackContext context)
        {
            Time.timeScale = 1;
            _inputService.PushMap(_playerMap);
        }
        
        // HAVE OVERALL GAME STATES:
        // MENU vs GAME vs PAUSE or something
        // Escape in Menu leads to Close game
        // Escape in Game leads to Pause
        // Escape in Pause leads to Game.. Can encode these things in a tree of some kind.
    }
}