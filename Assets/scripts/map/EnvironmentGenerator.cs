using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentGenerator : MonoBehaviour {

    private MapGenerator mapGenerator;

	// Use this for initialization
	void Start () {

        this.mapGenerator = GetComponent<MapGenerator>();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
