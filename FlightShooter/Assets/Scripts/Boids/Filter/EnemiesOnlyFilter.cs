using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boids/EnemiesOnlyFilter")]
public class EnemiesOnlyFilter : BoidsGroupFilter
{
    public override List<Transform> FilterBoidsGroup(BoidsAgent agent, List<Transform> allNeighbours)
    {
        List<Transform> filtered = new List<Transform>();
        foreach (var neighbour in allNeighbours)
        {
            var neighbourBoid = neighbour.GetComponent<BoidsAgent>();
            if (neighbourBoid != null && neighbour.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                filtered.Add(neighbour);
            }
        }

        return filtered;
    }
}
