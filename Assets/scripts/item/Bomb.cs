using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    // public GameObject explosion;

    public GameObject explosionPrefab;
    
    [Header("Bomb Attributes")]
    public float firingTimeSec = 5;
    public bool activateOnSpawn = true;

    private float timer;

    private Blinking blinking;

    private bool fired;

	// Use this for initialization
	void Awake () {

        if(explosionPrefab == null ) {
            Debug.Log("Explosion prefab not attached, bomb deactivated");
            this.enabled = false;
            return;
        }
        
        blinking = GetComponent<Blinking>();

        if(blinking == null)
        {
            Debug.Log("Blinking component not attached");
        }

        timer = 0;
        fired = false;

        if (activateOnSpawn)
            Fire();

        // Debug.Log("Bomb awake");
	}
	
	// Update is called once per frame
	void Update () {
        if (!fired)
            return;

        timer += Time.deltaTime;
        if (timer >= firingTimeSec)
        {
            GameObject explosionObject = Instantiate(explosionPrefab, transform.position, transform.rotation);

            Explosion explosion = explosionObject.GetComponent<Explosion>();
            explosion.Explode();

            timer = 0;

            Destroy(this.gameObject);
            
        }

	}

    public void Fire()
    {
        this.fired = true;
        blinking.StartBlinking(firingTimeSec);
    }

}
