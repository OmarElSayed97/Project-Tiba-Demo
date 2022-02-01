
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ConstantAngularVel : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]
    Vector3 angularVelVector;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.angularVelocity = angularVelVector;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
