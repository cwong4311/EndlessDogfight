using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestRotation : MonoBehaviour
{
    public Transform Target;

    public void FixedUpdate()
    {
        var distance = Target.position - transform.position;
        transform.rotation = Quaternion.LookRotation(distance, Vector3.up);
    }

    public void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward * 100, Color.red);
    }
}
