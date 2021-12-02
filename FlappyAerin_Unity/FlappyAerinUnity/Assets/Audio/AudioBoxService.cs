using Engine;
using JetBrains.Annotations;
using Tools;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Exposes the Create Audio methods to the inheriting objects.
    /// Interfaces CAN be generic.  // Could create a generic data class that contains the default args. IReusable<IProjectile>
    /// </summary>
    public interface IAudioBoxService : IService
    {
        [UsedImplicitly]public void PlaySound(ref AudioBoxParams p, out AudioBox ret);
    }

    /// <summary>
    /// Registers self with Service Factory
    /// Provides access to Audio Sources via a Reusable Bag. Objects will say "Give me audio clip", and then will return them if not using it.
    /// </summary>
    public class AudioBoxService : MonoBehaviour, IAudioBoxService
    {
        IReusableBag<AudioBox> _bag;
        [SerializeField] Transform sceneRoot;
        [SerializeField] Object prefab;

        void Awake()
        {
            ServiceFactory.AddService(this as IAudioBoxService);
        }

        void Start()
        {
            if (sceneRoot == null) sceneRoot = transform;
            _bag = new ReusableBag<AudioBox>(ref prefab, sceneRoot);
        }

        public void PlaySound(ref AudioBoxParams p, out AudioBox ret)
        {
            AudioBox newBox = _bag.Get();
            newBox.LoadParams(ref p);
            ret = newBox;
            newBox.Play();
        }
    }
}
