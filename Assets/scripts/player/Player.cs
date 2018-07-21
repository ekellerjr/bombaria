using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	// Use this for initialization
	void Start () {

        AttachToMainCamera();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void AttachToMainCamera()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag(CommonTags.MAIN_CAMERA);

        if (mainCamera == null)
        {
            Debug.Log("Main camera not found");
            return;
        }

        CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();

        if (cameraFollow == null)
        {
            Debug.Log("CameraFollow component not attached to main camera: " + mainCamera);
            return;
        }

        cameraFollow.Follow(this.transform);

    }

}
