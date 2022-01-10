using System.Collections;
using Audio;
using Engine;
using Tools;
using UnityEngine;

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
        Vector2 Direction { get; set; }
        float LifeStartTime { get; set; }
        float EndTime => LifeStartTime + _params.lifetime;

        /// <summary>
        /// Variables
        /// </summary>
        bool _projectileActive;

        void Awake()
        {
            _anim = new SpriteAnimator.SpriteAnimator(GetComponent<SpriteRenderer>());
            _audioService = ServiceFactory.GetService<IAudioBoxService>();
        }
        
        /// <summary>
        /// Moves to desired position, sets direction + all params
        /// </summary>
        public void Init(ref ProjectileParams p, Vector2 position, Vector2 dir)
        {
            _params = p;
            _projectileActive = true;
            Direction = dir;
            transform.position = position;
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
        
        void Update()
        {
            transform.Translate(Direction * (_params.speed * Time.deltaTime));
            _anim.TryStep();
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            // GetHittable component, call Hit(params).
            if (_projectileActive) _audioService.PlaySound(ref _params.impactSound, out _);
            _projectileActive = false;
            Direction = Vector2.zero;
        }
        void OnCollisionEnter2D(Collision2D other)
        {
            // GetHittable component, call Hit(params);
            if (_projectileActive) _audioService.PlaySound(ref _params.wallImpactSound, out _);
            _projectileActive = false;
            Direction = Vector2.zero;
        }
        
        // Return the projectile to it's bag.
        void Return()
        {
            _projectileActive = false;
            StopCoroutine(_timerRoutine);
            _anim.Stop();
            Direction = Vector2.zero;
            ReturnToBag(this);
        }
    }
}