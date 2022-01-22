using UnityEngine;

namespace Map
{
    public class Movable
    {
        protected Transform transform;
        protected MapMovementParams mapParams;
        public Vector3 Position => transform.position;
        public void SetPosition(Vector3 pos)
        {
            transform.localPosition = pos;
        }
        public void MoveUnsafe(Vector3 delta)
        {
            transform.position += mapParams.speed * delta;
        }
    }
}