using UnityEngine;

namespace Map
{
    [CreateAssetMenu(menuName = "Create FloorChunkParams", fileName = "FloorChunkParams", order = 0)]
    public class FloorChunkParams : ScriptableObject
    {
        public Sprite sprite;
        public Vector3 endPosition;
    }
}
