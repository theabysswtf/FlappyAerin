using System.Collections;
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
        /// ACTUAL BEHAVIOUR
        /// </summary>
        ProjectileParams _params;

        SpriteRenderer _sprite;
        Coroutine _timerRoutine;

        public ReturnDelegate ReturnToBag { get; set; }
        Vector2 Direction { get; set; }
        float LifeStartTime { get; set; }
        float EndTime => LifeStartTime + _params.lifetime;


        void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        public void Init(ref ReusableParams p, Vector2 position, Vector2 dir)
        {
            LoadParams(ref p);
            Direction = dir;
            transform.position = position;
            _timerRoutine = StartCoroutine(Countdown());
            
            // OnFire events would fire off here
        }
        
        public void LoadParams(ref ReusableParams p)
        {
            _params = (ProjectileParams) p;
            _sprite.sprite = _params.image;
        }
        void Update()
        {
            transform.Translate(Direction * (_params.speed * Time.deltaTime));
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
}