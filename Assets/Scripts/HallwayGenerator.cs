using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayGenerator : MonoBehaviour
{
    public GameObject hallwayPrefab;
    public List<GameObject> hallwayObjectPrefabs;

    public float lengthDivisions;
    public float widthDivisions;

    [SerializeField]
    private float gridSize;

    // Start is called before the first frame update
    void Start()
    {
        var hallway = Instantiate(hallwayPrefab, Vector3.zero, Quaternion.identity);


        for (var zdiv = 0; zdiv < lengthDivisions; zdiv++)
        {
            for (var xdiv = 0; xdiv < widthDivisions; xdiv++)
            {
                Vector3 spawnPosition = hallway.transform.position + Vector3.forward * zdiv * gridSize + Vector3.right * xdiv * gridSize;
                if (Random.value <= 0.5f)
                {
                    var steps = Instantiate(hallwayObjectPrefabs[0], spawnPosition, Quaternion.identity);
                    steps.transform.parent = hallway.transform;
                }
            }
        }

        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
