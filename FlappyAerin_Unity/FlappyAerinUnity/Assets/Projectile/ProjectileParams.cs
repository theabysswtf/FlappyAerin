using Audio;
using SpriteAnimator;
using UnityEngine;

namespace Projectile
{
    [CreateAssetMenu(menuName = "Create ProjectileParams", fileName = "ProjectileParams", order = 0)]
    public class ProjectileParams : ScriptableObject
    {
        public AudioBoxParams fireSound;
        public AudioBoxParams impactSound;
        public AudioBoxParams wallImpactSound;
        public SpriteAnimationParams spriteAnim;
        public float speed;
        public float lifetime;
    }
}