using System;
using Engine;
using Tools;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Object that is provided to other classes, provides the ability to use AudioClipParam objects
    /// </summary>
    public class AudioBox : MonoBehaviour, IReusable
    {
        // contains reference to: AudioSource obj, transform.
        // has constructor which just takes in those params
        // has method to play a sound. It'll parse out info contained in AudioClipParam file
        public void LoadParams(ref ReusableParams p)
        {
            throw new NotImplementedException();
        }

        public ReturnDelegate ReturnToBag { get; set; }

        public void Init(ref ReusableParams reusableParams)
        {
            throw new NotImplementedException();
        }
    }
}