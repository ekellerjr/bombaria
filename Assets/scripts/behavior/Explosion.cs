using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    public float range = 5;
    public float speed = 5;

    public float rangeDampingFactor = 2;
    public float speedDampingFactor = 2;

    private Vector3 targetScale;
    private bool exploding;
    
    private float curRange;
    private float dampingRange;

    private void Init()
    {
        targetScale = new Vector3(range, range, range);
        curRange = 0;
        dampingRange = range / rangeDampingFactor;
        exploding = false;
        
    }

    private void Update()
    {
        if (!exploding)
            return;

        // transform.localScale = Vector3.LerpUnclamped(transform.localScale, targetScale, speed * Time.deltaTime);
        float curSpeed = speed * Time.deltaTime;
        float dampingSpeed = curSpeed / speedDampingFactor;

        transform.localScale = new Vector3(
            transform.localScale.x + curSpeed,
            transform.localScale.y + curSpeed,
            transform.localScale.z + curSpeed);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, (curRange >= dampingRange) ? dampingRange : curRange += dampingSpeed);

        foreach (Collider c in hitColliders)
        {
            if (c.gameObject.CompareTag(CommonTags.DESTRUCTIBLE_TAG))
            {
                Destroy(c.gameObject);
            }
        }

        if (   transform.localScale.x >= targetScale.x 
            && transform.localScale.y >= targetScale.y
            && transform.localScale.z >= targetScale.z)
        {
            Destroy(this.gameObject);
        }
    }

    /*
    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Trigger: " + other.gameObject.tag);

        if (other.CompareTag(CommonTags.DESTRUCTIBLE_TAG))
        {
            Destroy(other.gameObject);
        }

    }
    */

    /*
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision: " + collision.gameObject.tag);

        if (collision.gameObject.CompareTag(CommonTags.DESTRUCTIBLE_TAG))
        {
            Destroy(collision.gameObject);
        }
    }
    */

    public void Explode()
    {
        Init();

        this.exploding = true;
    }

}
