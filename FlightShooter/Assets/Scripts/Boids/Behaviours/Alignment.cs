using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boids/Alignment")]
public class Alignment : BoidsBehaviour
{
    public override Vector3 GetMove(BoidsAgent agent, List<Transform> neighbours, EnemyBoidsController controller)
    {
        if (neighbours.Count == 0) return agent.transform.forward;

        Vector3 finalMovement = Vector3.zero;
        foreach (Transform neighbour in neighbours)
        {
            finalMovement += neighbour.transform.forward;
        }

        finalMovement /= neighbours.Count;
        return finalMovement;
    }
}
