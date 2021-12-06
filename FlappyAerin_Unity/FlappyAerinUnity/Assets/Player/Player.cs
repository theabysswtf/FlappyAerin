using Audio;
using Engine;
using Projectile;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /// <summary>
    /// INTERFACE
    /// </summary>
    public interface IPlayer : IService
    {
        InputActionMap ControlMap { get; }
    }
    
    /// <summary>
    /// CLASS DEFINITION
    /// </summary>0xA46638dEcb698520a569ADC5e882b17A83b54648
    public class Player : MonoBehaviour, IPlayer
    {
        [SerializeField] PlayerParams @params;
        [SerializeField] AudioBoxParams audioParams;
        [SerializeField] ProjectileParams projectileParams;
        [SerializeField] Transform projectileOrigin;
        public InputActionMap ControlMap { get; private set; }

        ICameraService _cam;
        IProjectileService _proj;
        IAudioBoxService _audio;
        
        Vector2 _mousePosition;
        Vector2 _velocity;
        float _gravity;
        bool _jumping;
        float _timeOfJumpStart;

        void Awake()
        {
            ServiceFactory.AddService(this);
            if (projectileOrigin == null) projectileOrigin = transform;
        }

        void Start()
        {
            _proj = ServiceFactory.GetService<IProjectileService>();
            _cam = ServiceFactory.GetService<ICameraService>();
            _audio = ServiceFactory.GetService<IAudioBoxService>();
            var r = ServiceFactory.GetService<IInputService>();
            ControlMap = r.GetPlayerMap();

            InputAction a = ControlMap.FindAction("Jump");
            a.started += OnJump;    // Start Jump
            a.canceled += OnJump;   // End Jump
        
            a = ControlMap.FindAction("Point");
            a.performed += OnPoint;    // Performed Motion
        
            a = ControlMap.FindAction("Fire");
            a.started += OnFire;    // Started Fire

            _gravity = @params.gravity;
        }

        void Update()
        {
            if (_jumping && Time.time < _timeOfJumpStart + @params.maxJumpDuration)
            {
                _velocity.y = @params.jumpForce;
            }
            _velocity.y = Mathf.Max(
                            _velocity.y - _gravity * Time.deltaTime, 
                            (_jumping) ? @params.maxGlideSpeed : @params.maxFallSpeed);
            transform.Translate(_velocity * Time.deltaTime);
        }

        void OnJump(InputAction.CallbackContext obj)
        {
            // should have slight cooldown on jump, + use jump buffering
            if (obj.started)
            {
                _jumping = true;
                _timeOfJumpStart = Time.time;
                _gravity = @params.glideGravity;
                
                _audio.PlaySound(ref audioParams, out AudioBox _);
                
            }
            else if (obj.canceled)
            {
                _jumping = false;
                _gravity = @params.gravity;
            }
        }
        void OnFire(InputAction.CallbackContext obj)
        {
            _proj.CreateProjectile(
                        ref projectileParams,
                        projectileOrigin.position, 
                        (@params.shotDirection).normalized,
                        out _);
        }
        void OnPoint(InputAction.CallbackContext obj)
        {
            _mousePosition = _cam.ScreenToWorld(obj.ReadValue<Vector2>()); 
        }
    }
}