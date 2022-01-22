using System.Collections.Generic;
using System.Linq;
using Engine;
using Player;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Object = UnityEngine.Object;

namespace Map
{
    /// <summary>
    /// Alrighty, so basically this is what will be controlling the map behaviour.
    /// Need to provide it with components needed to form the ground + the towers.
    /// Therefore need 45, 30 ish, 0, -30, -45 degree angle components in both directions
    /// Set leftmost corner to 0, rightmost corner to 1.
    /// </summary>
    
    /// Have reference to PlayerEngine thing, Check if player in bounds.
    public class MapEngine : MonoBehaviour, IMapService
    {
        public MapMovementParams MapParams => mapParams;
        
        [SerializeField] Transform sceneRoot;
        [SerializeField] Object pillarPrefab;
        [SerializeField] Object chunkPrefab;
        [SerializeField] MapMovementParams mapParams;
        [SerializeField] List<FloorChunkParams> possibleChunks;
        
        IPlayer _player;

        List<FloorChunk> _chunkList;
        List<PillarData> _pillarList;
        FloorChunk _lastUpdatedPiece;
        float _nextHeightStep;
        int _chunksSinceLastPillar;
        int _nextPillarIndex;
        bool _active;

        void Awake()
        {
            ServiceFactory.AddService(this as IMapService);
            mapParams.Reset();
            if (sceneRoot == null) sceneRoot = transform;
            _chunkList = new List<FloorChunk>();
            _pillarList = new List<PillarData>();
            _active = true;
            _chunksSinceLastPillar = 0;
            _nextPillarIndex = 0;
            _nextHeightStep = 0;
        }

        void Start()
        {
            for (int i = 0; i < mapParams.numChunks; i++)
            {
                GameObject obj = Instantiate(chunkPrefab, sceneRoot, true) as GameObject;
                FloorChunk c = new(obj, ref mapParams);
                c.SetPosition(new Vector3(-mapParams.numChunks + 2.0f * i, 0.0f, 0.0f));
                c.LoadParams(possibleChunks[2]);
                _chunkList.Add(c);
                _lastUpdatedPiece = c;
            }
            for (int i = 0; i < mapParams.numPillars; i++)
            {
                PillarPair obj = (Instantiate(pillarPrefab, sceneRoot, true) as GameObject)?.GetComponent<PillarPair>();
                Debug.Assert(obj != null, nameof(obj) + " != null");
                PillarData p = new(obj, ref mapParams);
                p.SetPosition(new Vector3(-mapParams.numChunks, -sceneRoot.transform.position.y, 0.0f));
                p.SetSeparation(mapParams.pillarSeparation);
                _pillarList.Add(p);
            }
        }

        void Update()
        {
            if (!_active) return;
            foreach (FloorChunk chunk in _chunkList)
            {
                // Translate chunks
                chunk.MoveUnsafe(Vector3.left * (mapParams.speed * Time.deltaTime));
                
                // Chunk Wrapping
                if (!(chunk.EndPosition.x < -mapParams.numChunks)) continue;
                chunk.LoadParams(GetNextChunkParams());
                chunk.SetPosition(_lastUpdatedPiece.EndPosition);
                _lastUpdatedPiece = chunk;
                _chunksSinceLastPillar++;
                
                // Pillar Wrapping
                if (_chunksSinceLastPillar <= mapParams.chunksToNextPillar) continue;
                _chunksSinceLastPillar = 0;
                PillarData p = _pillarList[_nextPillarIndex];
                _nextHeightStep = mapParams.GetNextHeightStep(_nextHeightStep);
                p.SetPosition(_lastUpdatedPiece.EndPosition + Vector3.down * sceneRoot.transform.position.y + _nextHeightStep * Vector3.up);
                p.SetSeparation(mapParams.pillarSeparation);
                _nextPillarIndex = (_nextPillarIndex + 1) % mapParams.numPillars;
            }
            
            // Translate Pillars
            foreach (PillarData pillar in _pillarList.Where(pillar => pillar.Position.x > -mapParams.numChunks))
            {
                pillar.MoveUnsafe(Vector3.left * (mapParams.speed * Time.deltaTime));
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
