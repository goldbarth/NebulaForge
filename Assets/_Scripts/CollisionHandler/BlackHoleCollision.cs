using UnityEngine;

namespace CollisionHandler
{
    public class BlackHoleCollision : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.CompareTag("Planet") 
               || other.gameObject.CompareTag("BrownPlanet"))
                other.gameObject.SetActive(false);
        }
    }
}
