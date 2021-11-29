using System.Collections.Concurrent;
using Engine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Projectile
{
    /// <summary>
    /// Interface
    /// </summary>
    public interface IProjectileService : IService
    {
        public void SpawnProjectile(ref ProjectileParams p, Vector2 position, Vector2 dir);
    }
    
    /// <summary>
    /// Data Structure
    /// </summary>
    public class ProjectileBag : IReusableBag<IProjectile>
    {
        static ConcurrentBag<IProjectile> Bag { get; } = new ConcurrentBag<IProjectile>();
        static Object _prefab;
        static Transform _sceneAnchor;

        public ProjectileBag(ref Object baseObject, Transform transform)
        {
            if (_prefab == null)
            {
                _prefab = baseObject;   
            }

            _sceneAnchor = transform;
        }

        IProjectile Generate()
        {
            var obj = Object.Instantiate(_prefab, _sceneAnchor, true) as GameObject;
            var pb = obj.GetComponent<Projectile>() as IProjectile;
            pb.ReturnToBag = ((IReusableBag<IProjectile>) this).Return;

            return pb;
        }

        IProjectile IReusableBag<IProjectile>.Get()
        {
            return (Bag.TryTake(out var t)) ? t : Generate();
        }

        void IReusableBag<IProjectile>.Return(IProjectile t)
        {
            Bag.Add(t);
        }
    }
    
    /// <summary>
    /// Service Behaviour
    /// </summary>
    public class ProjectileService : MonoBehaviour, IProjectileService
    {
        // Master set of ALL projectiles in it's bag.
        IReusableBag<IProjectile> _bag;
        [SerializeField] Transform sceneRoot;
        [SerializeField] Object projectileBase;

        void Awake()
        {
            ServiceFactory.AddService(this as IProjectileService);
        }

        void Start()
        {
            if (sceneRoot == null) sceneRoot = transform;
            _bag = new ProjectileBag(ref projectileBase, sceneRoot);
        }

        public void SpawnProjectile(ref ProjectileParams p, Vector2 position, Vector2 dir)
        {
            var newShot = _bag.Get();
            newShot.Init(ref p, position, dir);
        }
    }
}