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

    Vector3 movement;
    Rigidbody rigidbody;
    Transform transform;

    MoveDirection currentMoveDirection;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();

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
        else if (h < 0)
        {
            newMoveDirection = MoveDirection.left;
        }
        else if (v < 0)
        {
            newMoveDirection = MoveDirection.backward;
        }
        else if (v > 0)
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
        
        Quaternion newRotation = Quaternion.Euler(lookDirection);

        rigidbody.MoveRotation(newRotation);
    }

    private void Move(float h, float v)
    {
        movement.Set(h, 0f, v);

        movement = movement.normalized * speed * Time.deltaTime;

        rigidbody.MovePosition(transform.position + movement);
    }

}
