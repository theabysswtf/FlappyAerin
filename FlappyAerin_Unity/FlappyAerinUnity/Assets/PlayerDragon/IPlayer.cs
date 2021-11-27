using UnityEngine.InputSystem;

public interface IPlayer : IService
{
    InputActionMap ControlMap { get; }
}