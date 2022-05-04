using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxManager : MonoBehaviour
{
    [SerializeField] private GameObject cube;
    private GameObject[] gameObjects;
    void Start()
    {
        gameObjects = new GameObject[1000];
        // Create 1000 boxes indicating the full color cube
        for (int xIndex = 0; xIndex < 10; xIndex++)
        for (int yIndex = 0; yIndex < 10; yIndex++)
        for (int zIndex = 0; zIndex < 10; zIndex++)
        {
            Color color = new Color(xIndex / 10.0f, yIndex / 10.0f, zIndex / 10.0f);
            Vector3 position = new Vector3(xIndex - 5, yIndex - 5, zIndex - 5);

            GameObject newCube = Instantiate(cube, position, Quaternion.identity);
            newCube.transform.parent = transform;

            MeshRenderer meshRenderer = newCube.GetComponent<MeshRenderer>();
            meshRenderer.material.color = color;

            gameObjects[xIndex * 100 + yIndex * 10 + zIndex] = newCube;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
