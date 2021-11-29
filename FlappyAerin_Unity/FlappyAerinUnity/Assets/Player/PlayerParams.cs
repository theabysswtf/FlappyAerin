using UnityEngine;

namespace Player
{
    [CreateAssetMenu(menuName = "Create PlayerParams", fileName = "PlayerParams", order = 0)]
    public class PlayerParams : ScriptableObject
    {
        [Header("Projectile")]
        public Object projectileBase;
        public Vector2 shotDirection;
        [Header("Motion")]
        public float jumpForce;
        public float maxJumpDuration;
        public float gravity;
        public float glideGravity;
        public float maxGlideSpeed;
        public float maxFallSpeed;
    }
}