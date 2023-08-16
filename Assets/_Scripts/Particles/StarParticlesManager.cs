using UnityEngine;

namespace Particles
{
    // Inspired by: Star Rendering With No Skybox. https://www.youtube.com/watch?v=Ipl7EVDsExk
    // From Mobius Digital's great Space Game "Outer Wilds". 
    public class StarParticlesManager : MonoBehaviour
    {
        private void Start()
        {
            var particleRenderer = GetComponent<ParticleSystemRenderer>();
            particleRenderer.material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;
        }

        private void Update()
        {
            transform.rotation = Quaternion.identity;
        }
    }
}