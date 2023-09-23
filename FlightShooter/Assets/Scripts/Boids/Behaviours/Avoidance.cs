using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boids/Avoidance")]
public class Avoidance : BoidsBehaviour
{
    public override Vector3 GetMove(BoidsAgent agent, List<Transform> neighbours, BoidsMasterController controller)
    {
        if (neighbours.Count == 0) return Vector3.zero;

        Vector3 finalMovement = Vector3.zero;
        int avoidCount = 0;

        foreach (Transform neighbour in neighbours)
        {
            if (Vector3.SqrMagnitude(neighbour.position - agent.transform.position) < agent.SqrAvoidRadius)
            {
                avoidCount++;
                finalMovement += (agent.transform.position - neighbour.position);
            }
        }

        if (avoidCount > 0) finalMovement /= avoidCount;
        return finalMovement;
    }
}