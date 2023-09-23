using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoidsGroupFilter : ScriptableObject
{
    public abstract List<Transform> FilterBoidsGroup(BoidsAgent agent, List<Transform> allNeighbours);
}
