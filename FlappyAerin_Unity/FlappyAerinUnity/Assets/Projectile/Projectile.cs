using System.Collections;
using Tools;
using UnityEngine;
using UnityEngine.Events;

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

        SpriteAnimator.SpriteAnimator _anim;
        Coroutine _timerRoutine;

        public ReturnDelegate ReturnToBag { get; set; }
        Vector2 Direction { get; set; }
        float LifeStartTime { get; set; }
        float EndTime => LifeStartTime + _params.lifetime;

        public UnityEvent<string> tempEvent;
        public UnityEvent<string> onHit;    // This should take in a list of hittable objects + a ProjectileParams object

        
        void Awake()
        {
            _anim = new SpriteAnimator.SpriteAnimator(GetComponent<SpriteRenderer>());
        }
        
        /// <summary>
        /// Moves to desired position, sets direction + all params
        /// </summary>
        public void Init(ref ProjectileParams p, Vector2 position, Vector2 dir)
        {
            _params = p;
            _anim.SetAnim(_params.spriteAnim);
            Direction = dir;
            transform.position = position;
            _timerRoutine = StartCoroutine(Countdown());
            _anim.Play(0);
            
            tempEvent.Invoke("Hello");
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
            // if other has a Hittable component, start the Return coroutine.
            // Ideally this would literally just say "hey world, I'm dead", then whatever
            // components are actively listening to this projectile would say "Okay~ let's"
            // Handle this thing's demise. Play a sound, start a
            // particle effect, etc. at it's last location"
            // This means the generic onDie method for a projectile should take in a "ProjectileParams" object, 
            // as well as a transform. That should be good.
            // onHit should take in a set of objects to be collided with
            // Maybe have a global statusTicker which says "okay, everything's status will update according to this timer.
            // and will just pay attention to, and invoke methods as time goes on.

            Return();
        }
        
        // Return the projectile to it's bag.
        void Return()
        {
            tempEvent.Invoke("World!");
            StopCoroutine(_timerRoutine);
            _anim.Stop();
            Direction = Vector2.zero;
            ReturnToBag(this);
        }

        public void PrintString(string s)
        {
            Debug.Log(s);
        }
    }
}