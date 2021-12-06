using SpriteAnimator;
using Tools;
using UnityEngine;

namespace Projectile
{
    [CreateAssetMenu(menuName = "Create ProjectileParams", fileName = "ProjectileParams", order = 0)]
    public class ProjectileParams : ScriptableObject
    {
        public SpriteAnimationParams spriteAnim;
        public float speed;
        public float lifetime;
    }
}