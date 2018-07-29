using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private static readonly Vector3 screen_center = new Vector3(0.5F, 0.5F, 0);

    public GameObject target;

    [Header("Smoothing")]
    public float smoothTime = 0.1f;
    public float maxSmoothSpeed = 10f;
    public float flankSmoothing = 1f;

    [Header("Casting")]
    public float sphereRadius = 2f;

    [Header("Positioning")]
    public Vector3 offset;
    public bool freezeRotationX;
    public bool freezeRotationY;
    public bool freezeRotationZ;

    private Vector3 smoothingVelocity = Vector3.zero;

    private Vector3 curOffset;

    private Vector3 startRotation;

    private Camera cam;

    void Awake()
    {
        cam = CommonUtils.GetComponentOrPanic<Camera>(this.gameObject);

        startRotation = cam.transform.rotation.eulerAngles;

        curOffset = offset;
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        RaycastHit hit;

        if (Physics.SphereCast(cam.ViewportPointToRay(screen_center), sphereRadius, out hit))
        {
            // Debug.Log("Hit: " + hit.transform.gameObject);

            if (hit.transform.gameObject == target)
            {
                if (curOffset.z != offset.z)
                {
                    
                    Vector3 nextOffset = new Vector3(
                    offset.x,
                    offset.y,
                    Mathf.Lerp(curOffset.z, offset.z, flankSmoothing * Time.deltaTime));
                    
                    Vector3 checkNextCamPos = CalcNextCamPos(nextOffset);

                    if (!CheckObstacleAtCamPosition(checkNextCamPos))
                    {
                        curOffset.Set(
                          nextOffset.x,
                          nextOffset.y,
                          nextOffset.z);
                    }
                }
            }
            else
            {
                
                curOffset.Set(
                      offset.x,
                      offset.y,
                      Mathf.Lerp(curOffset.z, 0, flankSmoothing * Time.deltaTime));
                      
            }
        }

        SetCamPosition(CalcNextCamPos(curOffset));

    }

    private void SetCamPosition(Vector3 position)
    {
        cam.transform.position = position;

        cam.transform.LookAt(target.transform);

        Vector3 tr = cam.transform.rotation.eulerAngles;

        cam.transform.rotation = Quaternion.Euler(
            freezeRotationX ? startRotation.x : tr.x,
            freezeRotationY ? startRotation.y : tr.y,
            freezeRotationZ ? startRotation.z : tr.z
            );
    }

    private Vector3 CalcNextCamPos(Vector3 offset)
    {
        Vector3 nextCamPos = Vector3.SmoothDamp(
           cam.transform.position,
           target.transform.position + offset,
           ref smoothingVelocity,
           smoothTime,
           maxSmoothSpeed,
           Time.deltaTime);

        nextCamPos.Set(
            (float)Math.Round(nextCamPos.x, 2),
            (float)Math.Round(nextCamPos.y, 2),
            (float)Math.Round(nextCamPos.z, 2));

        return nextCamPos;
    }

    private bool CheckObstacleAtCamPosition(Vector3 desiredPosition)
    {
        Vector3 curCamPosition = cam.transform.position;
        Quaternion curCamRotation = cam.transform.rotation;

        SetCamPosition(desiredPosition);

        bool obstacleHit = false;

        RaycastHit hit;
        if (Physics.SphereCast(cam.ViewportPointToRay(screen_center), sphereRadius, out hit))
        {
            if (hit.transform.gameObject != target)
            {
                obstacleHit = true;
            }
        }

        cam.transform.position = curCamPosition;
        cam.transform.rotation = curCamRotation;

        return obstacleHit;
    }

    public void Follow(GameObject target)
    {
        this.target = target;
    }

}