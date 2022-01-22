using UnityEngine;

namespace Map
{
    public class FloorChunk : Movable
    {
        
        public Vector3 EndPosition => transform.localPosition + _params.endPosition;
        public Vector3 StartPosition => transform.localPosition;
        public Bounds Bounds => _collider.bounds;

        readonly SpriteRenderer _renderer;
        readonly Collider2D _collider;
        FloorChunkParams _params;

        public FloorChunk(GameObject obj, ref MapMovementParams p)
        {
            transform = obj.GetComponent<Transform>();
            mapParams = p;
            
            _renderer = obj.GetComponent<SpriteRenderer>();
            _collider = obj.GetComponent<Collider2D>();
        }
        public void LoadParams(FloorChunkParams p)
        {
            _params = p;
            _renderer.sprite = _params.sprite;
        }
    }
}
