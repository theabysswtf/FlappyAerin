using System.Collections;
using UnityEngine;

namespace Tools
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ColourShifter : MonoBehaviour
    {
        SpriteRenderer _rend;
        Coroutine _activeShift;

        void Awake()
        {
            _rend = GetComponent<SpriteRenderer>();
        }

        IEnumerator DoShift(Color startColor, Color endColor, float duration)
        {
            for (float t = 0; t <= duration; t = Mathf.Min(t + Time.deltaTime, duration))
            {
                float T = t / duration;
                _rend.color = startColor * (1 - T) + endColor * T;
                yield return null;   
            }
        }

        public void ShiftColor(string hexCode)
        {
            if (!ColorUtility.TryParseHtmlString(hexCode, out Color endColor)) return;
            if (_activeShift is not null)
                StopCoroutine(_activeShift);
            _activeShift = StartCoroutine(DoShift(_rend.color, endColor, 0.75f));
        }
    

        public void SetColor(string hexCode)
        {
            if (!ColorUtility.TryParseHtmlString(hexCode, out Color endColor)) return;
            if (_activeShift is not null)
                StopCoroutine(_activeShift);
            _rend.color = endColor;
        }
    }
}