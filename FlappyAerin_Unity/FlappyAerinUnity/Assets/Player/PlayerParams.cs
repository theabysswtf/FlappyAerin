using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Player
{
    [CreateAssetMenu(menuName = "Create PlayerParams", fileName = "PlayerParams", order = 0)]
    public class PlayerParams : ScriptableObject
    {
        [Header("Stats")]
        int _hitsTaken;
        public int maxHitsTaken;
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

        public void Reset()
        {
            _hitsTaken = 0;
        }

        public bool TakeHit()
        {
            return ++_hitsTaken > maxHitsTaken;
        }
    }
}