using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed = 6f;

    private Vector3 movement;

    private Rigidbody rb;

    private void Awake()
    {
        rb = CommonUtils.GetComponentOrPanic<Rigidbody>(this.gameObject);
    }

    private void FixedUpdate()
    {
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");
        
        Move(h, v);
        Rotate();

        //Animating(h, v);
    }

    private void Move(float horizontal, float vertical)
    {
        movement.Set(horizontal, 0f, vertical);
        CommonUtils.Move(rb, movement, speed);
    }

    private void Rotate()
    {
        DiscreteMovement.RotateXZDiscrete45(rb, movement);
    }
}
