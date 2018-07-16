using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquarePatternGenerator : MonoBehaviour
{

    public GameObject stonePrefab;

    public float width = 50;
    public float height = 50;

    public float squareSize = 1;

    public KeyCode resetKey = KeyCode.R;

    // Use this for initialization
    void Start()
    {
        GenerateMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(resetKey))
        {
            GenerateMap();
        }
    }
    private void GenerateMap()
    {
        ClearMap();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(
                    transform.position.x + (-width / 2 + x * squareSize),
                    0,
                    transform.position.y + (-height / 2 + y * squareSize));

                GameObject mapObject = Instantiate(
                    stonePrefab,
                    stonePrefab.transform.position + position,
                    stonePrefab.transform.rotation);

                mapObject.transform.parent = this.transform;

            }
        }
    }

    private void ClearMap()
    {
        foreach (Transform go in transform)
        {
            Destroy(go.gameObject);
        }
    }
}
