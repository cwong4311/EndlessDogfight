using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform player;

    private void Start()
    {
        player = GameObject.FindFirstObjectByType<PlayerController>()?.transform;
    }

    void LateUpdate()
    {
        Vector3 minimapPosition = player.position;
        minimapPosition.y = transform.position.y;
        transform.position = minimapPosition;

        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}
