using System;
using System.Collections.Generic;
using Engine;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Map
{
    /// <summary>
    /// Alrighty, so basically this is what will be controlling the map behaviour.
    /// Need to provide it with components needed to form the ground + the towers.
    /// Therefore need 45, 30 ish, 0, -30, -45 degree angle components in both directions
    /// Set leftmost corner to 0, rightmost corner to 1.
    /// </summary>
    
    public class MapEngine : MonoBehaviour
    {
        [SerializeField] Transform sceneRoot;
        [SerializeField] Object pillarPrefab;
        [SerializeField] Object chunkPrefab;
        [SerializeField] MapMovementParams mapParams;
        [SerializeField] List<FloorChunkParams> possibleChunks;
        [SerializeField] int numChunks;
        [SerializeField] int chunksToNextPillar;
        
        ReusableBag<PillarPair> _pillarBag; // Different Use Case.

        List<FloorChunk> _chunkStack;
        FloorChunk _lastPiece;
        int _chunksSinceLastPillar;

        void Awake()
        {
            if (sceneRoot == null) sceneRoot = transform;
            _chunkStack = new List<FloorChunk>();
            _pillarBag = new ReusableBag<PillarPair>(ref pillarPrefab, sceneRoot);
        }

        void Start()
        {
            for (var i = 0; i < numChunks; i++)
            {
                var obj = Object.Instantiate(chunkPrefab, sceneRoot, true) as GameObject;
                var c = new FloorChunk(obj, ref mapParams);
                c.SetPosition(new Vector3(2.0f * i, 0.0f, 0.0f));
                c.LoadParams(possibleChunks[2]);
                _chunkStack.Add(c);
                _lastPiece = c;
            }

            _chunksSinceLastPillar = chunksToNextPillar - 1;
        }

        void Update()
        {
            foreach (var chunk in _chunkStack)
            {
                chunk.MoveUnsafe(Vector3.left * Time.deltaTime);
                if (!(chunk.EndPosition.x < 0)) continue;
                chunk.LoadParams(GetNextChunkParams());
                chunk.SetPosition(_lastPiece.EndPosition);
                _lastPiece = chunk;

                _chunksSinceLastPillar ++;
                if (_chunksSinceLastPillar < chunksToNextPillar) continue;
                var b = _pillarBag.Get();
                b.transform.position = _lastPiece.EndPosition;
            }
        }

        FloorChunkParams GetNextChunkParams()
        {
            // based on currently desired direction, which one will move the current position closest to that direction?
            // for now, just give me straight left
            // Also need to handle the game start case. Should go straight left initially.
            return possibleChunks[2];
        }
    }
}
