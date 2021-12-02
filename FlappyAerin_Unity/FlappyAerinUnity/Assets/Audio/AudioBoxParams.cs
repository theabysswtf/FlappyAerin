using Tools;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Scriptable Object which contains clip, levels, etc. Can have multiple AudioClipParams with the same audio clip, & different pitch/volume/etc.
    /// </summary>
    [CreateAssetMenu(menuName = "Create AudioBoxParams", fileName = "AudioBoxParams", order = 0)]
    public class AudioBoxParams : ScriptableObject
    {
        public AudioClip clip;
        [Range(0, 1)]
        public float volume;
        [Range(-3, 3)]
        public float pitch;
        public bool loop;
    }
}