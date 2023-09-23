using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boids/Composite")]
public class CompositeBehaviour : BoidsBehaviour
{
    [System.Serializable]
    public struct CompositeBehaviourData
    {
        public BoidsBehaviour Behaviour;
        public BoidsGroupFilter GroupFilter;
        public float Weight;
    }

    public CompositeBehaviourData[] weightedBehaviours;

    public override Vector3 GetMove(BoidsAgent agent, List<Transform> allNeighbours, BoidsMasterController controller)
    {
        Vector3 totalMove = Vector3.zero;
        foreach (var weightedBehaviour in weightedBehaviours)
        {
            List<Transform> neighbours = (weightedBehaviour.GroupFilter != null) ? 
                weightedBehaviour.GroupFilter.FilterBoidsGroup(agent, allNeighbours) : 
                allNeighbours;

            Vector3 partialMove = weightedBehaviour.Behaviour.GetMove(agent, neighbours, controller) * weightedBehaviour.Weight;

            if (partialMove != Vector3.zero && partialMove.sqrMagnitude > weightedBehaviour.Weight * weightedBehaviour.Weight)
            {
                partialMove.Normalize();
                partialMove *= weightedBehaviour.Weight;
            }

            totalMove += partialMove;
        }

        return totalMove;
    }
}
