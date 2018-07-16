using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform target;
    public float smoothing = 5f;
    
    public Vector3 offset;

    private void FixedUpdate()
    {
        if (target == null)
            return;

        Vector3 targetCamPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        
        transform.LookAt(target);
    }

    public void Follow(Transform target)
    {
        this.target = target;
    }

}
