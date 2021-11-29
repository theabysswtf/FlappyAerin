using System.Collections;
using UnityEngine;

namespace Projectile
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Projectile : MonoBehaviour, IProjectile
    {

        /// <summary>
        /// ACTUAL BEHAVIOUR
        /// </summary>
        ProjectileParams @params;
        
        Rigidbody2D _rb;
        SpriteRenderer _sprite;
        Coroutine _timerRoutine;

        public Vector2 Direction { get; set; }
        float LifeStartTime { get; set; }
        float EndTime => LifeStartTime + @params.lifetime;
        
        public ReturnDelegate ReturnToBag { get; set; }

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _sprite = GetComponent<SpriteRenderer>();
        }

        public void Init(ref ProjectileParams p, Vector2 position, Vector2 dir)
        {
            @params = p;
            Direction = dir;
            transform.position = position;
            _sprite.sprite = p.image;
            _timerRoutine = StartCoroutine(Countdown());
            
            // OnFire events would fire off here
        }
        void Update()
        {
            transform.Translate(Direction * (@params.speed * Time.deltaTime));
            // OnTick events would fire off here. Maybe 1 per n ticks
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            // OnCollision events would fire off here. Maybe 1 per n ticks
            Die();
        }

        void Die()
        {
            // OnDie events fire off here
            //Start the burst coroutine
            StopCoroutine(_timerRoutine);
            Direction = Vector2.zero;
            ReturnToBag(this);
        }

        IEnumerator Countdown()
        {
            LifeStartTime = Time.time;
            yield return new WaitUntil(() => Time.time > EndTime);
            Die();
        }
    }

    public interface IProjectile
    {
        public void Init(ref ProjectileParams p, Vector2 position, Vector2 dir);
        ReturnDelegate ReturnToBag { get; set; }
    }
    public delegate void ReturnDelegate(Projectile p);
}