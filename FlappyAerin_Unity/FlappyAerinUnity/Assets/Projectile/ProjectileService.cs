using Engine;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Projectile
{
    /// <summary>
    /// Interface
    /// </summary>
    public interface IProjectileService : IService
    {
        public void CreateProjectile(ref ProjectileParams p, Vector2 position, Vector2 dir, out Projectile projectile);
    }

    /// <summary>
    /// Service Behaviour
    /// </summary>
    public class ProjectileService : MonoBehaviour, IProjectileService
    {
        // Master set of ALL projectiles in it's bag.
        IReusableBag<Projectile> _bag;
        [SerializeField] Transform sceneRoot;
        [SerializeField] Object projectileBase;

        void Awake()
        {
            ServiceFactory.AddService(this as IProjectileService);
        }

        void Start()
        {
            if (sceneRoot == null) sceneRoot = transform;
            _bag = new ReusableBag<Projectile>(ref projectileBase, sceneRoot);
        }

        public void CreateProjectile(ref ProjectileParams p, Vector2 position, Vector2 dir, out Projectile projectile)
        {
            Projectile newShot = _bag.Get();
            newShot.Init(ref p, position, dir);
            projectile = newShot;
        }
    }
}