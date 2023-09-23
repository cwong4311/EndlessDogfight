using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysFaceUp : MonoBehaviour
{
    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
