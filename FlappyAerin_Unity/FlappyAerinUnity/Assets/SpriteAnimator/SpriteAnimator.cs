using UnityEngine;

namespace SpriteAnimator
{
    public class SpriteAnimator 
    {
        readonly SpriteRenderer _renderer;
        int _currentFrame;

        float TimeOfLastFrame { get; set; }
        float AdjustedTime => Anim.timeScaled ? Time.time : Time.unscaledTime;
        bool OnNextFrame => AdjustedTime > TimeOfLastFrame + Anim.SecondsPerFrame;
        
        SpriteAnimationParams Anim { get; set; }

        public bool Playing { get; private set; }

        public SpriteAnimator(SpriteRenderer rend)
        {
            _renderer = rend;
            Playing = false;
            _currentFrame = 0;
        }
        
        public void SetAnim(SpriteAnimationParams anim, bool autoplay=true)
        {
            Anim = anim;
            _renderer.sprite = Anim.Get(_currentFrame);
            Playing = autoplay;
        }

        public void Play()
        {
            Playing = true;
            TimeOfLastFrame = AdjustedTime;
        } 
        public void Play(int i)
        {
            _currentFrame = i;
            Playing = true;
            TimeOfLastFrame = AdjustedTime;
        } 
        public void Stop() => Playing = false;

        public void TryStep()
        {
            if (!Playing || !OnNextFrame) return;
            _renderer.sprite = Anim.Get(++_currentFrame);
            TimeOfLastFrame = AdjustedTime;
        }

        public void ToggleVisible(bool visible)
        {
            _renderer.enabled = visible;
        }
    }
}