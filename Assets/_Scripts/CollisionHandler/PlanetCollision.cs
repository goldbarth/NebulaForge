using System.Collections;
using UnityEngine;

namespace CollisionHandler
{
    public class PlanetCollision : MonoBehaviour
    {
        [SerializeField] private GameObject _explosion;
        [SerializeField] private GameObject _childObject;

        private void Start()
        {
            _explosion.SetActive(false);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("BrownPlanet"))
                StartCoroutine(DestroyPlanet());
        }

        private IEnumerator DestroyPlanet()
        {
            _explosion.SetActive(true);
            yield return new WaitForSeconds(2f);
            _childObject.SetActive(false);
            yield return new WaitForSeconds(10f);
            _explosion.SetActive(false);
        }
    }
}