using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private CameraFollow camFollow;

    // Use this for initialization
    void Start()
    {

        camFollow = CommonUtils.GetComponentInGameObjectFoundWithTag<CameraFollow>(CommonTags.MAIN_CAMERA);

        if (camFollow != null)
            camFollow.Follow(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
