using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boids/PhysicsLayerFilter")]
public class PhysicsLayerFilter : BoidsGroupFilter
{
    public LayerMask avoidLayerMask;

    public override List<Transform> FilterBoidsGroup(BoidsAgent agent, List<Transform> allNeighbours)
    {
        List<Transform> filtered = new List<Transform>();
        foreach (var neighbour in allNeighbours)
        {
            if (((1 << neighbour.gameObject.layer) & avoidLayerMask.value) != 0)
            {
                filtered.Add(neighbour);
            }
        }

        return filtered;
    }
}
