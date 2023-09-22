using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boids/Composite")]
public class CompositeBehaviour : BoidsBehaviour
{
    [System.Serializable]
    public struct CompositeBehaviourData
    {
        public BoidsBehaviour behaviour;
        public float weight;
    }

    public CompositeBehaviourData[] weightedBehaviours;

    public override Vector3 GetMove(BoidsAgent agent, List<Transform> neighbours, BoidsMasterController controller)
    {
        Vector3 totalMove = Vector3.zero;
        foreach (var weightedBehaviour in weightedBehaviours)
        {
            Vector3 partialMove = weightedBehaviour.behaviour.GetMove(agent, neighbours, controller) * weightedBehaviour.weight;

            if (partialMove != Vector3.zero && partialMove.sqrMagnitude > weightedBehaviour.weight * weightedBehaviour.weight)
            {
                partialMove.Normalize();
                partialMove *= weightedBehaviour.weight;
            }

            totalMove += partialMove;
        }

        return totalMove;
    }
}
