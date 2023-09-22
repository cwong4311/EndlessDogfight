using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoidsBehaviour : ScriptableObject
{
    public abstract Vector3 GetMove(BoidsAgent agent, List<Transform> neighbours, BoidsMasterController controller);
}
