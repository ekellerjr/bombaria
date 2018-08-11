using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    [Header("Transform Scaling")]
    public bool scaleTransform = false;
    public float maxScaleX = 5;
    public float maxScaleY = 5;
    public float maxScaleZ = 5;
    public float transformScalingXSpeed = 5;
    public float transformScalingYSpeed = 5;
    public float transformScalingZSpeed = 5;

    [Header("Sphere Colliding")]
    public float maxSphereRadius = 3;
    public float sphereGrowingSpeed = 3;

    [Header("Light Handling")]
    public Light explosionLight;
    public float maxLightIntensity = 5;
    public float lightIntensityIncreasingSpeed = 10;
    public float maxLightRange = 5;
    public float lightRangeIncreasingSpeed = 5;

    private Vector3 targetScale;
    private bool exploding;

    private float curSphereRadius;

    private void Init()
    {
        targetScale = new Vector3(maxScaleX, maxScaleY, maxScaleZ);
        curSphereRadius = 0;
        exploding = false;
    }

    private void Update()
    {
        if (!exploding)
            return;

        bool finish = true;

        finish &= DoScaling();

        finish &= DoSphereCasting();

        finish &= DoLighting();

        if (finish)
        {
            exploding = false;
            Destroy(this.gameObject);
        }

    }

    private bool DoLighting()
    {

        if (explosionLight == null)
            return true;

        explosionLight.range = explosionLight.range >= maxLightRange ? maxLightRange : explosionLight.range + lightRangeIncreasingSpeed * Time.deltaTime;

        explosionLight.intensity = explosionLight.intensity >= maxLightIntensity ? maxLightIntensity :
            explosionLight.intensity + lightIntensityIncreasingSpeed * Time.deltaTime;

        return explosionLight.range >= maxLightRange && explosionLight.intensity >= maxLightIntensity;
    }

    private bool DoSphereCasting()
    {

        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position,
            (curSphereRadius >= maxSphereRadius) ? maxSphereRadius : curSphereRadius += sphereGrowingSpeed * Time.deltaTime);

        foreach (Collider c in hitColliders)
        {
            if (CommonUtils.CompareTags(c.gameObject, CommonUtils.CompareTagsMode.MatchOneTag,
                BombariaTags.DESTRUCTIBLE,
                BombariaTags.ENEMY))
            {
                Destroy(c.gameObject);
            }
        }

        return curSphereRadius >= maxSphereRadius;
    }

    private bool DoScaling()
    {
        if (!scaleTransform)
            return true;
        
        transform.localScale = new Vector3(
            transform.localScale.x >= maxScaleX ? maxScaleX : transform.localScale.x + transformScalingXSpeed * Time.deltaTime,
            transform.localScale.y >= maxScaleY ? maxScaleY : transform.localScale.y + transformScalingYSpeed * Time.deltaTime,
            transform.localScale.z >= maxScaleZ ? maxScaleZ : transform.localScale.z + transformScalingZSpeed * Time.deltaTime);

        return transform.localScale.x >= targetScale.x
            && transform.localScale.y >= targetScale.y
            && transform.localScale.z >= targetScale.z;
    }

    public void Explode()
    {
        Init();

        this.exploding = true;
    }

}
