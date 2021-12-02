using Tools;
using UnityEngine;

namespace Projectile
{
    [CreateAssetMenu(menuName = "Create ProjectileParams", fileName = "ProjectileParams", order = 0)]
    public class ProjectileParams : ScriptableObject
    {
        public float speed;
        public Sprite image;
        public float lifetime;
    }
}