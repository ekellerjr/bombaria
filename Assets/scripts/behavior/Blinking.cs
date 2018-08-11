using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinking : MonoBehaviour
{

    public Material blinkMaterial;

    private double duration; // { get; set; }

    private double duration025;
    private double duration05;
    private double duration075;
    private double durationDiv4;
    private double durationDiv8;
    private double durationDiv16;

    private Material originalMaterial;

    private Renderer rndr;

    private double timer;

    private double curBlinkTime;

    private bool blinking;

    private double blinkPeriod;

    // Use this for initialization
    private void Init()
    {

        rndr = GetComponent<Renderer>();
        originalMaterial = rndr.materials[0];
        blinkPeriod = 0;

        timer = 0;
        curBlinkTime = 0;

        this.blinking = false;

    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("blinking: " + this.blinking);

        if (!this.blinking)
            return;

        // Debug.Log("blinking: " + this.blinking);

        timer += Time.deltaTime;
        curBlinkTime += Time.deltaTime;

        // Debug.Log("timer: " + timer);
        // Debug.Log("curBlinkTime: " + curBlinkTime);



        if (timer < duration025)
        {
            blinkPeriod = duration;
        }
        else if (timer >= duration025 && timer < duration05)
        {
            blinkPeriod = durationDiv4;
        }
        else if (timer >= duration05 && timer < duration075)
        {
            blinkPeriod = durationDiv8;
        }
        else if (timer >= duration075 && timer < duration)
        {
            blinkPeriod = durationDiv16;
        }
        else if (timer >= duration)
        {
            this.blinking = false;
            timer = 0;
            return;
        }


        // Debug.Log("blinkPeriod: " + blinkPeriod);


        if (curBlinkTime >= blinkPeriod)
        {
            Blink();
            curBlinkTime = 0;
        }
    }

    private void Blink()
    {
        // Debug.Log("Blink");

        Material[] mats = rndr.materials;
        Material curMat = mats[0];

        if (curMat == originalMaterial)
        {
            mats[0] = blinkMaterial;
        }
        else
        {
            mats[0] = originalMaterial;
        }

        rndr.materials = mats;
    }

    public void StartBlinking(float duration)
    {
        Init();

        // Debug.Log("Start Blinking");
        this.duration = duration;

        this.duration025 = duration * 0.25;
        this.duration05 = duration * 0.50;
        this.duration075 = duration * 0.75;


        this.durationDiv4 = duration / 16;
        this.durationDiv8 = duration / 32;
        this.durationDiv16 = duration / 128;

        this.blinking = true;
    }

}
