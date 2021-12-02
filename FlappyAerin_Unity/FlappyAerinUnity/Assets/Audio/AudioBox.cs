using System.Collections;
using Tools;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Object that is provided to other classes, provides the ability to use AudioClipParam objects
    /// </summary>
    /// 
    [RequireComponent(typeof(AudioSource))]
    public class AudioBox : MonoBehaviour, IReusable
    {
        AudioBoxParams _params;
        AudioSource _source;
        Coroutine _timerRoutine;

        public void Play() => _timerRoutine = StartCoroutine(PlayAndReturn());
        void Stop()
        {
            // OnDie events fire off here
            //Start the burst coroutine
            StopCoroutine(_timerRoutine);
            _source.Stop();
            ReturnToBag(this);
        }

        void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        public void LoadParams(ref AudioBoxParams p)
        {
            _params = p;
            _source.clip = _params.clip;
            _source.pitch = _params.pitch;
            _source.volume = _params.volume;
        }

        public ReturnDelegate ReturnToBag { get; set; }
        

        IEnumerator PlayAndReturn()
        {
            _source.Play();
            yield return new WaitUntil(() => !_source.isPlaying);
            Stop();
        }
    }
}