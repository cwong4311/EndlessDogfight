using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boids/Cohesion")]
public class Cohesion : BoidsBehaviour
{
    public float SmoothTime = 0.5f;
    private Vector3 _currentVelocity;
    
    public override Vector3 GetMove(BoidsAgent agent, List<Transform> neighbours, EnemyBoidsController controller)
    {
        if (neighbours.Count == 0) return Vector3.zero;

        Vector3 finalMovement = Vector3.zero;
        foreach (Transform neighbour in neighbours)
        {
            finalMovement += neighbour.position;
        }

        finalMovement /= neighbours.Count;
        finalMovement -= agent.transform.position;
        return Vector3.SmoothDamp(agent.transform.forward, finalMovement, ref _currentVelocity, SmoothTime);
    }
}
