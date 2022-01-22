using UnityEngine;

namespace Map
{
    [CreateAssetMenu(menuName = "Create MapMovementParams", fileName = "MapMovementParams", order = 0)]
    public class MapMovementParams : ScriptableObject
    {
        public float speed = 1.0f;
        public float minSpeed = 1.0f;
        public float maxSpeed = 4.5f;
        public float speedStep = .05f;
        public int numChunks = 9;
        public int numPillars = 3;
        public int chunksToNextPillar = 2;
        public float pillarSeparation = 4;
        public float minPillarSeparation = 1.25f;
        public float maxPillarSeparation = 4.0f;

        public float minPillarHeightDelta = -1.5f;
        public float maxPillarHeightDelta = 2.5f;
        public float maxPillarHeightStep = 0.5f;

        public void Reset()
        {
            speed = minSpeed;
            pillarSeparation = maxPillarSeparation;
        }

        public void Step()
        {
            speed = Mathf.Min(speed + speedStep, maxSpeed);
            pillarSeparation = Mathf.Max(pillarSeparation - .05f, minPillarSeparation);
        }

        public float GetNextHeightStep(float lastHeightStep)
        {
            float step = Random.Range(-maxPillarHeightStep, maxPillarHeightStep);
            return Mathf.Clamp(lastHeightStep + step, minPillarSeparation, maxPillarSeparation);
        }
    }
}