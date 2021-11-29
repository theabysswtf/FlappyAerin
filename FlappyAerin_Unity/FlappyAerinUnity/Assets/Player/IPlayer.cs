using Engine;
using UnityEngine.InputSystem;

namespace Player
{
    public interface IPlayer : IService
    {
        InputActionMap ControlMap { get; }
    }
}