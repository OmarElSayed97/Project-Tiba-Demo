using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float explosionForce = 5f;
    [SerializeField] private float explosionRadius = 1;
    [SerializeField] private float upwardModifier = 0;
    [SerializeField] private LayerMask explosionMask = 1 << 1;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        Explode();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, explosionMask);

        foreach (Collider collider in colliders)
        {
            Rigidbody rb;
            if (collider.gameObject.TryGetComponent<Rigidbody>(out rb))
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardModifier, ForceMode.Impulse);
            }
        }

    }
}
