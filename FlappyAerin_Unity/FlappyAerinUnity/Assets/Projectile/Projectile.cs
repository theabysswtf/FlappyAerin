using System.Collections;
using Audio;
using Engine;
using Map;
using Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Projectile
{

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Projectile : MonoBehaviour, IReusable
    {

        /// <summary>
        /// Parameters
        /// </summary>
        ProjectileParams _params;
        MapMovementParams _mapParams;
        SpriteAnimator.SpriteAnimator _anim;
        Coroutine _timerRoutine;
        
        /// <summary>
        /// Services
        /// </summary>
        IAudioBoxService _audioService;

        /// <summary>
        /// Properties
        /// </summary>
        public ReturnDelegate ReturnToBag { get; set; }
        Vector3 Direction { get; set; }
        float LifeStartTime { get; set; }
        float EndTime => LifeStartTime + _params.lifetime;

        /// <summary>
        /// Variables
        /// </summary>
        public UnityEvent impactEvent;
        Rigidbody2D _rb;
        bool _projectileActive;
        float _speed;

        void Awake()
        {
            _mapParams = ServiceFactory.GetService<IMapService>().MapParams;
            _anim = new SpriteAnimator.SpriteAnimator(GetComponent<SpriteRenderer>());
            _audioService = ServiceFactory.GetService<IAudioBoxService>();
            _rb = GetComponent<Rigidbody2D>();
        }
        
        /// <summary>
        /// Moves to desired position, sets direction + all params
        /// </summary>
        public void Init(ref ProjectileParams p, Vector2 position, Vector2 dir, Transform origin)
        {
            _params = p;
            _projectileActive = true;
            Direction = dir;
            transform.position = position;
            transform.SetParent(origin);
            _anim.SetAnim(_params.spriteAnim);
            _timerRoutine = StartCoroutine(Countdown());
            _anim.Play(0);
            _audioService.PlaySound(ref _params.fireSound, out _);
        }
        
        /// <summary>
        /// Lifetime Enforcer.
        /// </summary>
        IEnumerator Countdown()
        {
            LifeStartTime = Time.time;
            yield return new WaitUntil(() => Time.time > EndTime);
            Return();
        }
        
        void FixedUpdate()
        {
            _anim.TryStep();
            _speed = _projectileActive ? _params.speed : _mapParams.speed;
            _rb.MovePosition(transform.position + Direction * (_speed * Time.deltaTime));
        }

        void Expend()
        {
            impactEvent.Invoke();
            _projectileActive = false;
            _anim.Stop();
            Direction = Vector2.left * _mapParams.speed;
        }
        
        void OnCollisionEnter2D(Collision2D other) // This handles any practical object impacts
        {
            // GetHittable component, call Hit(params);
            if (_projectileActive) _audioService.PlaySound(ref _params.wallImpactSound, out _);
            Expend();
        }
        
        // Return the projectile to it's bag.
        void Return()
        {
            StopCoroutine(_timerRoutine);
            ReturnToBag(this);
        }
    }
}