using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour {

    public GameObject bomb;
    public Transform bombSpawn;

    [Header("Player Skills")]
    public int actionDelaySec = 1;
    
    float timer = 0;

	void Start () {
		
	}
	
	
	void Update () {

        timer += Time.deltaTime;
        bool actionDone = false;

        if (Input.GetKeyDown(KeyCode.Space) && timer >= actionDelaySec)
        {
            
                actionDone = DoAction();
           
        }

        if (actionDone) { timer= 0; }
	}

    private bool DoAction()
    {
        Instantiate(bomb, bombSpawn.position, bombSpawn.rotation);

        return true;
    }
}
