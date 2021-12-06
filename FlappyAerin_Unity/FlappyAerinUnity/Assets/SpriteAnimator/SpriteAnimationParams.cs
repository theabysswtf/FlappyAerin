using System.Collections.Generic;
using UnityEngine;

namespace SpriteAnimator
{
    [CreateAssetMenu(menuName = "Create SpriteAnimationParams", fileName = "SpriteAnimationParams", order = 0)]
    public class SpriteAnimationParams : ScriptableObject
    {
        //A sprite animation contains a set of sprites representing the main loop 
        // Should have options for variant sprites. i.e. Left vs Right facing
        public List<Sprite> frames;
        public float fps;
        public bool timeScaled;
        public bool looping;
        public float SecondsPerFrame => 1.0f / fps;

        public Sprite Get(int index)
        {
            // AnimComplete Event raised from here!
            var currentFrame = (looping) ? (index) % frames.Count : Mathf.Min(index, frames.Count - 1);
            return frames[currentFrame];
        }
    
        // Should have AnimationEvent delegate declared, then make OnStart, OnEnd, OnTick alternates.
    }
}