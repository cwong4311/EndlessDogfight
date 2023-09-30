using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class BoxBoundaryManager : MonoBehaviour
{
    [Serializable]
    public struct BoundariesWall
    {
        public Collider collider;
        public Vector3 restrictedDirection;
    }

    public BoundariesWall[] Walls;
    private bool[] isColliding;

    private Transform playerTarget;
    private PlayerController playerController;
    public void Start()
    {
        playerController = GameObject.FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerTarget = playerController.transform;
        }

        if (Walls.Length != 6)
        {
            throw new InvalidOperationException("BoxBoundaryManager needs to have exactly 6 colliders");
        }

        isColliding = new bool[Walls.Length];
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < Walls.Length; i++)
        {
            var wall = Walls[i];
            var wallCollider = wall.collider;
            var restrictedDirection = wall.restrictedDirection;

            var closestPoint = wallCollider.ClosestPoint(playerTarget.position);
            if ((playerTarget.position - closestPoint).sqrMagnitude < 15f * 15f)
            {
                if (isColliding[i] == false)
                {
                    playerController.RestrictVelocity = (playerController.RestrictVelocity.HasValue) ?
                        playerController.RestrictVelocity.Value + restrictedDirection :
                        restrictedDirection;

                    playerTarget.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }

                isColliding[i] = true;
            }
            else
            {
                isColliding[i] = false;
            }
        }

        if (isColliding.All(x => !x) && playerController.RestrictVelocity.HasValue)
        {
            playerController.RestrictVelocity = null;
        }
    }
}
