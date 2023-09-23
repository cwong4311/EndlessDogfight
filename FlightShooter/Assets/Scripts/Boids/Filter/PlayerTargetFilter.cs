using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boids/PlayerTargetFilter")]
public class PlayerTargetFilter : BoidsGroupFilter
{
    public override List<Transform> FilterBoidsGroup(BoidsAgent agent, List<Transform> allNeighbours)
    {
        List<Transform> filtered = new List<Transform>();
        foreach (var neighbour in allNeighbours)
        {
            var playerObject = neighbour.GetComponent<PlayerController>();
            if (playerObject != null && neighbour.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                filtered.Add(neighbour);
            }
        }

        return filtered;
    }
}
