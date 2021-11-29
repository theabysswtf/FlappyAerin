using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Engine
{
    public interface IInputService : IService
    {
        public void PushMap(InputActionMap map);
        public InputActionMap PopMap();
        public void SetDefaultMap(InputActionMap map);
        public InputActionMap GetPlayerMap();
        public InputActionMap GetUIMap();
    }
    
    // Input Switcher used to switch between different input maps
    public class InputService : MonoBehaviour, IInputService
    {
        PlayerInput _playerInput;
        Stack<InputActionMap> _fifoMapStack;
        InputActionMap _defaultMap;

        void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _defaultMap = _playerInput.currentActionMap;
            _fifoMapStack = new Stack<InputActionMap>();
            ServiceFactory.AddService(this as IInputService);
        }
        
        // Swap Out Maps
        public void PushMap(InputActionMap map)
        {
            _fifoMapStack.Push(_playerInput.currentActionMap);
            _playerInput.currentActionMap = map;
            Debug.Log("Changed to " + _playerInput.currentActionMap.name);
        }
        public InputActionMap PopMap()
        {
            InputActionMap current = _defaultMap;
            if (_fifoMapStack.Count > 0)
            {
                current = _fifoMapStack.Pop();
            }
            if (_playerInput.currentActionMap != current)
                _playerInput.currentActionMap = current;
            Debug.Log("Changed to " + _playerInput.currentActionMap.name);
            return current;   
        }
        public void SetDefaultMap(InputActionMap map)
        {
            _defaultMap = map;
        }

        // Map Retrieval
        InputActionMap GetMapWithName(string target)
        {
            return _playerInput.actions.FindActionMap(target);
        }

        public InputActionMap GetPlayerMap()
        {
            return GetMapWithName("Player");
        }

        public InputActionMap GetUIMap()
        {
            return GetMapWithName("UI");
        }       
    }
}