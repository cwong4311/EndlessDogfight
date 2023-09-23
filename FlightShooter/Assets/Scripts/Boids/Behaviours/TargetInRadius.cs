using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Boids/TargetInRadius")]
public class TargetInRadius : BoidsBehaviour
{
    public float trackingRadius = 300f;
    public float stopTrackingRadius = 50f;
    public float targetTrackingDamp = 50f;

    private Vector3 _currentVelocity;
    private SortedDictionary<float, Transform> targetDistances;

    public override Vector3 GetMove(BoidsAgent agent, List<Transform> neighbours, BoidsMasterController controller)
    {
        if (neighbours.Count == 0) return Vector3.zero;

        var sqrRadius = trackingRadius * trackingRadius;
        var sqrAvoid = stopTrackingRadius * stopTrackingRadius;
        targetDistances = new SortedDictionary<float, Transform>();

        Vector3 finalMovement = Vector3.zero;
        foreach (Transform neighbour in neighbours)
        {
            var sqrDistanceToTarget = (neighbour.position - agent.transform.position).sqrMagnitude;
            if (sqrDistanceToTarget < sqrRadius && sqrDistanceToTarget > sqrAvoid)
            {
                if (!targetDistances.ContainsKey(sqrDistanceToTarget))
                    targetDistances.Add(sqrDistanceToTarget, neighbour);
            }
        }

        if (targetDistances.Count > 0)
        {
            var closestTarget = targetDistances.First().Value;
            var distToTarget = (closestTarget.position - agent.transform.position);

            var smoothDamp = (distToTarget.magnitude / trackingRadius) * targetTrackingDamp + 0.1f; // add epsilon
            finalMovement = Vector3.SmoothDamp(agent.transform.forward, distToTarget, ref _currentVelocity, smoothDamp);
        }

        return finalMovement;
    }
}
