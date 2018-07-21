using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private enum MoveDirection
    {
        forward, backward, right, left
    }

    public float speed = 6f;

    private Vector3 movement;

    private Rigidbody rb;
    //Transform transform;

    MoveDirection currentMoveDirection;

    private void Awake()
    {
        rb = CommonUtils.GetComponentOrPanic<Rigidbody>(this.gameObject);
        // transform = GetComponent<Transform>();

        currentMoveDirection = MoveDirection.forward;
    }

    private void FixedUpdate()
    {
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");
        
        Move(h, v);

        Turning(h, v);

        //Animating(h, v);
    }

    private void Turning(float h, float v)
    {
        if (h == 0 && v == 0)
            return;

        MoveDirection newMoveDirection = currentMoveDirection;
        
        if (h > 0)
        {
            newMoveDirection = MoveDirection.right;
        }

        if (h < 0)
        {
            newMoveDirection = MoveDirection.left;
        }

        if (v < 0)
        {
            newMoveDirection = MoveDirection.backward;
        }

        if (v > 0)
        {
            newMoveDirection = MoveDirection.forward;
        }

        if (currentMoveDirection == newMoveDirection)
            return;
        
        currentMoveDirection = newMoveDirection;

        Vector3 lookDirection = Vector3.zero;

        switch (newMoveDirection)
        {
            case MoveDirection.forward:
                lookDirection = new Vector3(0, 0, 0);
                break;
            case MoveDirection.backward:
                lookDirection = new Vector3(0, 180, 0);
                break;
            case MoveDirection.right:
                lookDirection = new Vector3(0, 90, 0);
                break;
            case MoveDirection.left:
                lookDirection = new Vector3(0, 270, 0);
                break;
            default:
                break;
        }

        rb.MoveRotation(Quaternion.Euler(lookDirection));
    }

    private void Move(float h, float v)
    {
        movement.Set(h, 0f, v);

        CommonUtils.Move(rb, movement, speed);
    }


}
