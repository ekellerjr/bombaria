using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    public float range = 5;
    public float speed = 5;
    
    private Vector3 targetScale;
    private bool exploding;
    
    private void Init()
    {
        targetScale = new Vector3(range, range, range);
        exploding = false;
    }

    private void Update()
    {
        if (!exploding)
            return;

        // transform.localScale = Vector3.LerpUnclamped(transform.localScale, targetScale, speed * Time.deltaTime);
        float curSpeed = speed * Time.deltaTime;

        transform.localScale = new Vector3(
            transform.localScale.x + curSpeed,
            transform.localScale.y + curSpeed,
            transform.localScale.z + curSpeed);

        if (   transform.localScale.x >= targetScale.x 
            && transform.localScale.y >= targetScale.y
            && transform.localScale.z >= targetScale.z)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Trigger: " + other.gameObject.tag);

        if (other.CompareTag("Destructible"))
        {
            Destroy(other.gameObject);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision: " + collision.gameObject.tag);

        if (collision.gameObject.CompareTag("Destructible"))
        {
            Destroy(collision.gameObject);
        }
    }

    public void Explode()
    {
        Init();

        this.exploding = true;
    }

}
