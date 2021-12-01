using Engine;
using Tools;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Exposes the Create Audio methods to the inheriting objects.
    /// Interfaces CAN be generic.  // Could create a generic data class that contains the default args. IReusable<IProjectile>
    /// </summary>
    public interface IAudioBoxService : IReusableService<AudioBox> { }

    /// <summary>
    /// Registers self with Service Factory
    /// Provides access to Audio Sources via a Reusable Bag. Objects will say "Give me audio clip", and then will return them if not using it.
    /// </summary>
    public class AudioBoxService : MonoBehaviour, IAudioBoxService
    {
        // Master set of ALL projectiles in it's bag.
        IReusableBag<AudioBox> _bag;
        [SerializeField] Transform sceneRoot;
        [SerializeField] Object projectileBase;

        void Awake()
        {
            ServiceFactory.AddService(this as IAudioBoxService);
        }

        void Start()
        {
            if (sceneRoot == null) sceneRoot = transform;
            _bag = new ReusableBag<AudioBox>(ref projectileBase, sceneRoot);
        }
        
        public void Instance(ref ReusableParams p, out AudioBox ret)
        {
            AudioBox newBox = _bag.Get();
            newBox.Init(ref p);
            ret = newBox;
        }
    }
}
