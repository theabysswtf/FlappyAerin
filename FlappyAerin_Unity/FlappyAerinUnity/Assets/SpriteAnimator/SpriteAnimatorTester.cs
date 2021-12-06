using Engine;
using UnityEngine;
using UnityEngine.InputSystem;


// This is the animator base class. All it does is Tick through sprites
// Ideally, we'd have some mechanism to sync up multiple animations/tracks.

/*
 * OH! it'd be super cool to provide the spriteanimator with an index method, like,
 * based on this random input parameter, decide what frame to retrieve
 */

namespace SpriteAnimator
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimatorTester : MonoBehaviour
    {
        SpriteAnimator _anim;
        InputActionMap _controlMap;
        public SpriteAnimationParams @params;

        void Awake()
        {
            _anim = new SpriteAnimator(GetComponent<SpriteRenderer>());
            _anim.SetAnim(@params, true);
        }

        void Start()
        {
            _controlMap = ServiceFactory.GetService<IInputService>().GetPlayerMap();
            var pauseAction = _controlMap.FindAction("Test");
            pauseAction.started += Toggle;
        }

        void Toggle(InputAction.CallbackContext obj)
        {
            Debug.Log("Called");
            if (_anim.Playing) _anim.Stop();
            else _anim.Play();
        }

        void Update()
        {
            _anim.TryStep();
        }
    }
}
