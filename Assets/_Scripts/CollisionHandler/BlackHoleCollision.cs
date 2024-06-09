using System.Collections;
using UnityEngine;

namespace CollisionHandler
{
    public class BlackHoleCollision : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.CompareTag("Planet") 
               || other.gameObject.CompareTag("BrownPlanet"))
                StartCoroutine(WaitTillEventHorizon(other));
        }

        private IEnumerator WaitTillEventHorizon(Collision other)
        {
            yield return new WaitForSeconds(.5f);
            other.gameObject.SetActive(false);
        }
    }
}
