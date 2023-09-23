using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boids/SameGroupFilter")]
public class SameGroupFilter : BoidsGroupFilter
{
    public override List<Transform> FilterBoidsGroup(BoidsAgent agent, List<Transform> allNeighbours)
    {
        List<Transform> filtered = new List<Transform>();
        foreach (var neighbour in allNeighbours)
        {
            var neighbourBoid = neighbour.GetComponent<BoidsAgent>();
            if (neighbourBoid != null && agent.BoidsGroup == neighbourBoid.BoidsGroup)
            {
                filtered.Add(neighbour);
            }
        }

        return filtered;
    }
}
