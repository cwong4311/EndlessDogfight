using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boids/StayAboveGround")]
public class StayAboveGround : BoidsBehaviour
{
    public float groundLevel;

    public override Vector3 GetMove(BoidsAgent agent, List<Transform> neighbours, BoidsMasterController controller)
    {
        var distanceFromGround = agent.transform.position.y - groundLevel;

        if (distanceFromGround < 0)
        {
            return new Vector3(0f, Mathf.Abs(distanceFromGround * distanceFromGround), 0f);
        }

        return Vector3.zero;
    }
}
