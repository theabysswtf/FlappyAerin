using Tools;
using UnityEngine;

namespace Map
{
    public class FloorChunk
    {
        
        public Vector2 EndPosition => (Vector2)(_transform.localPosition) + _params.endPosition;

        readonly Transform _transform;
        readonly SpriteRenderer _renderer;
        readonly MapMovementParams _mapParams;
        FloorChunkParams _params;

        public FloorChunk(GameObject obj, ref MapMovementParams p)
        {
            _transform = obj.GetComponent<Transform>();
            _renderer = obj.GetComponent<SpriteRenderer>();
            _mapParams = p;
        }
        public void SetPosition(Vector3 pos)
        {
            _transform.localPosition = pos;
        }
        public void MoveUnsafe(Vector3 delta)
        {
            _transform.position += _mapParams.speed * delta;
        }
        public void LoadParams(FloorChunkParams p)
        {
            _params = p;
            _renderer.sprite = _params.sprite;
        }
    }
}
