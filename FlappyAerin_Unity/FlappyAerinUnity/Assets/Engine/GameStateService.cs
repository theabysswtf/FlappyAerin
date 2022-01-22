using System;
using Map;
using UnityEngine;

namespace Engine
{
    public interface IGameStateService : IService
    {
        public void PassPillar();
        public void KillPlayer();
    }
    
    public class GameStateService : MonoBehaviour, IGameStateService
    {
        IMapService _mapService;
        
        void Awake()
        {
            ServiceFactory.AddService(this as IGameStateService);
        }

        void Start()
        {
            _mapService = ServiceFactory.GetService<IMapService>();
        }

        public void PassPillar()
        {
            _mapService.MapParams.Step();
            // UI modifications
        }

        public void KillPlayer()
        {
            Debug.Log("Player Died!");
            // Needs control to load the Game scene.
        }
    }
}
