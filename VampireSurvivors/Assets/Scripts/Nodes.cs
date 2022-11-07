using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nodes
{
    [System.Serializable]
    public class NAudioClip
    {
        public float scale;
        public AudioClip audioClip;
    }

    [System.Serializable]
    public class NAudioSource
    {
        public NAudioClip soundClip;
        public AudioSource audioSource;
    }
}
