using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    [Header("Settings")]
    public float speed = 3;
    public float timeBeofeNextAction = 2;

    private Vector3 curMoveDirection;

    private float timer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        
    }

}
