using Engine;
using UnityEngine;
using UnityEngine.InputSystem;

  
public class Player : MonoBehaviour, IPlayer
{
    public InputActionMap ControlMap { get; private set; }

    void Awake()
    {
        ServiceFactory.AddService(this);
    }

    void Start()
    {
        var r = ServiceFactory.GetService<IInputRouter>();
        ControlMap = r.GetPlayerMap();

        var a = ControlMap.FindAction("Jump");
        a.started += OnJump;    // Start Jump
        a.canceled += OnJump;   // End Jump
    }

    void OnJump(InputAction.CallbackContext obj)
    {
        Debug.Log("Jumping!");
    }
}