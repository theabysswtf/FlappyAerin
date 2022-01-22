using System;
using Audio;
using Engine;
using Projectile;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    /// <summary>
    /// INTERFACE
    /// </summary>
    public interface IPlayer : IService
    {
        InputActionMap ControlMap { get; }
        Vector2 Position { get; }
        Bounds Bounds { get; }
    }
    
    /// <summary>
    /// CLASS DEFINITION
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Player : MonoBehaviour, IPlayer
    {
        [SerializeField] PlayerParams @params;
        [FormerlySerializedAs("audioParams"),SerializeField] AudioBoxParams jumpAudioParams;
        [SerializeField] AudioBoxParams bonkAudioParams;
        [SerializeField] AudioBoxParams passAudioParams;
        [SerializeField] ProjectileParams projectileParams;
        [SerializeField] Transform projectileOrigin;
        public InputActionMap ControlMap { get; private set; }
        public Vector2 Position => transform.position;
        public Bounds Bounds => _collider.bounds;

        IGameStateService _game;
        ICameraService _cam;
        IProjectileService _proj;
        IAudioBoxService _audio;
        
        Collider2D _collider;
        Rigidbody2D _rb;
        
        Vector2 _mousePosition;
        Vector3 _velocity;
        float _gravity;
        bool _jumping;
        float _timeOfJumpStart;

        void Awake()
        {
            ServiceFactory.AddService(this as IPlayer);
            if (projectileOrigin == null) projectileOrigin = transform;
            @params.Reset();
        }

        void Start()
        {
            _proj = ServiceFactory.GetService<IProjectileService>();
            _cam = ServiceFactory.GetService<ICameraService>();
            _audio = ServiceFactory.GetService<IAudioBoxService>();
            _game = ServiceFactory.GetService<IGameStateService>();
            
            IInputService r = ServiceFactory.GetService<IInputService>();
            ControlMap = r.GetPlayerMap();

            InputAction a = ControlMap.FindAction("Jump");
            a.started += OnJump;    // Start Jump
            a.canceled += OnJump;   // End Jump
        
            a = ControlMap.FindAction("Point");
            a.performed += OnPoint;    // Performed Motion
        
            a = ControlMap.FindAction("Fire");
            a.started += OnFire;    // Started Fire

            _collider = GetComponent<Collider2D>();
            _rb = GetComponent<Rigidbody2D>();
            
            _gravity = @params.gravity;
        }

        void FixedUpdate()
        {
            if (_jumping && Time.time < _timeOfJumpStart + @params.maxJumpDuration)
            {
                _velocity.y = @params.jumpForce;
            }
            _velocity.y = Mathf.Max(
                            _velocity.y - _gravity * Time.deltaTime, 
                            (_jumping) ? @params.maxGlideSpeed : @params.maxFallSpeed);
            _rb.MovePosition(transform.position + _velocity * Time.deltaTime);
        }

        void OnJump(InputAction.CallbackContext obj)
        {
            // should have slight cooldown on jump, + use jump buffering
            if (obj.started)
            {
                _jumping = true;
                _timeOfJumpStart = Time.time;
                _gravity = @params.glideGravity;
                
                _audio.PlaySound(ref jumpAudioParams, out AudioBox _);
                
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

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag($"GoalPost"))
            {
                Debug.Log("Increase Point! Tell Game Engine what's happening");
                _game.PassPillar();
                _audio.PlaySound(ref passAudioParams, out AudioBox _);
            }
        }
        void OnCollisionEnter2D(Collision2D other)
        {
            // Debug.Log("COLLIDED WITH: " + other.collider.name);
            // Need to reference the State transition machine here to go from PLAYING to DIEING, then DIEING to RESTARTING or menu or whatever
            _audio.PlaySound(ref bonkAudioParams, out AudioBox _);
            if (@params.TakeHit())
            {
                _game.KillPlayer();
            }
        }
    }
}