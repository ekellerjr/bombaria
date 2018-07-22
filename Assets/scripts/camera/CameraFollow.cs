using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private static readonly Vector3 screen_center = new Vector3(0.5F, 0.5F, 0);

    public GameObject target;

    [Header("Smoothing")]
    public float camFollowSmoothing = 5f;
    public float obstacleFlankingSmoothing = 2f;

    [Header("Obstacle Flanking")]
    public float stayOnTargetAfterFlankingTime = 1f;
    public float timeBeforeBeginFlanking = 0.5f;

    [Header("Casting")]
    public float sphereCastRadius = 0.5f;

    [Header("Positioning")]
    public Vector3 offset;
    public bool freezeRotationX;
    public bool freezeRotationY;
    public bool freezeRotationZ;

    private Vector3 curOffset;

    private Vector3 startRotation;

    // timer
    private float beforeBeginFlankingTimer;
    private float stayOnTargetAfterFlankingTimer;

    private Camera cam;

    private bool obstacleHit;
    private bool checkForObstaclesOnTheWayBack;

    void Awake()
    {
        cam = CommonUtils.GetComponentOrPanic<Camera>(this.gameObject);

        startRotation = cam.transform.rotation.eulerAngles;

        curOffset = offset;

        obstacleHit = false;
        checkForObstaclesOnTheWayBack = true;

        beforeBeginFlankingTimer = 0;

        stayOnTargetAfterFlankingTimer = 0;
    }



    private void FixedUpdate()
    {
        if (target == null)
            return;

        FlankObstacles();

        FollowTarget();

    }

    private void FlankObstacles()
    {
        // flank obstacles
        RaycastHit hit;

        if (Physics.SphereCast(cam.ViewportPointToRay(screen_center), sphereCastRadius, out hit))
        {
            Debug.Log("Hit: " + hit.transform.gameObject);

            // target in sight field
            if (hit.transform.gameObject == target)
            {
                // Do nothing if we are already at end position
                if (curOffset.z == offset.z)
                    return;

                // after obstacle was hit
                if (obstacleHit)
                {
                    obstacleHit = false;

                    // reset beforeBeginFlankingTimer
                    beforeBeginFlankingTimer = 0;
                }
                // no obstacle hit before
                else
                {
                    // TODO: Do nothing?
                }

                // stay as long as stayOnTargetAfterFlankingTime and then go back to offset position ...
                stayOnTargetAfterFlankingTimer += Time.deltaTime;

                if (stayOnTargetAfterFlankingTimer >= stayOnTargetAfterFlankingTime)
                {
                    // before we can go back to target position with offset, we have to check if there is no obstacle

                    Vector3 desiredPosition = target.transform.position + offset;

                    // if we already have checked for obstacles OR there is NO obstacle at desired position then ...
                    if (!checkForObstaclesOnTheWayBack || !CheckObstacleAtCamPosition(desiredPosition))
                    {
                        // .. go back by setting curOffsets z component linear back to original offsets z component
                        curOffset.Set(
                            curOffset.x,
                            curOffset.y,
                            Mathf.Lerp(curOffset.z, offset.z, obstacleFlankingSmoothing * Time.deltaTime));

                        // and notice that we have already checked for obstacles on the way back, so we do not have to check every frame
                        checkForObstaclesOnTheWayBack = false;
                    }
                    else
                    {
                        // otherwise stay
                        stayOnTargetAfterFlankingTimer = 0;
                    }
                }
            }
            // obstacle in sight field
            else
            {
                // No flanking if we are already at end position
                if (curOffset.z == 0)
                    return;

                // TODO: check type of obstacle 

                // we alread hit an obstacle before
                if (obstacleHit)
                {
                    // TODO: Do nothing?
                }
                // no obstacle hit before
                else
                {
                    obstacleHit = true;

                    // we have now a new situation, so we have to check everything anew
                    checkForObstaclesOnTheWayBack = true;

                    stayOnTargetAfterFlankingTimer = 0;
                }

                // add time to beforeBeginFlankingTimer
                beforeBeginFlankingTimer += Time.deltaTime;

                // begin flanking if beforeBeginFlankingTimer is up
                if (beforeBeginFlankingTimer >= timeBeforeBeginFlanking)
                {
                    // set z position of cam linear by obstacleFlankingSmoothing to z position of target
                    // doing this by reducing curOffsets z component linear to 0
                    curOffset.Set(
                        curOffset.x,
                        curOffset.y,
                        Mathf.Lerp(curOffset.z, 0, obstacleFlankingSmoothing * Time.deltaTime)
                    );
                }
            }
        }
    }

    private bool CheckObstacleAtCamPosition(Vector3 desiredPosition)
    {
        Vector3 curCamPosition = cam.transform.position;

        cam.transform.position = desiredPosition;
        cam.transform.LookAt(target.transform);

        cam.transform.rotation = GetTargetCamRotation(cam.transform.rotation);

        bool obstacleHit = false;

        RaycastHit hit;
        if (Physics.SphereCast(cam.ViewportPointToRay(screen_center), sphereCastRadius, out hit))
        {
            if (hit.transform.gameObject != target)
            {
                obstacleHit = true;
            }
        }

        cam.transform.position = curCamPosition;
        cam.transform.LookAt(target.transform);

        cam.transform.rotation = GetTargetCamRotation(cam.transform.rotation);

        return obstacleHit;
    }

    private void FollowTarget()
    {
        Vector3 targetCamPos = GetTargetCamPosition(target.transform.position);

        cam.transform.position = Vector3.Lerp(cam.transform.position, targetCamPos, camFollowSmoothing * Time.deltaTime);
        cam.transform.LookAt(target.transform);

        cam.transform.rotation = GetTargetCamRotation(cam.transform.rotation);
    }

    private Vector3 GetTargetCamPosition(Vector3 targetPosition)
    {
        return targetPosition + curOffset;
    }

    private Quaternion GetTargetCamRotation(Quaternion targetRotation)
    {
        Vector3 tr = targetRotation.eulerAngles;

        return Quaternion.Euler(
            freezeRotationX ? startRotation.x : tr.x,
            freezeRotationY ? startRotation.y : tr.y,
            freezeRotationZ ? startRotation.z : tr.z
            );
    }

    public void Follow(GameObject target)
    {
        this.target = target;
    }

}
