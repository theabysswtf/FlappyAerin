using Engine;

namespace Map
{
    public interface IMapService : IService
    {
        public MapMovementParams MapParams { get; }
    }
}