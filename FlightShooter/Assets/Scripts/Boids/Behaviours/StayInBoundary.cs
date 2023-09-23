using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boids/StayInBoundary")]
public class StayInBoundary : BoidsBehaviour
{
    public Vector3 Center;
    public float Radius = 100f;
    public float SoftThreshold;
    
    public override Vector3 GetMove(BoidsAgent agent, List<Transform> neighbours, EnemyBoidsController controller)
    {
        Vector3 centerOffset = Center - agent.transform.position;
        var distanceFromCenter = centerOffset.magnitude / Radius;

        if (distanceFromCenter < SoftThreshold) return Vector3.zero;

        return centerOffset * distanceFromCenter * distanceFromCenter;
    }
}
