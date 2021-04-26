using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayGenerator : MonoBehaviour, Receiver<SendNextWallMessage.SpawnHallInfo>
{
    public GameObject hallwayPrefab;
    public List<GameObject> hallwayObjectPrefabs;

    public float lengthDivisions;
    public float widthDivisions;

    [SerializeField]
    private float gridSize;

    public void receive(SendNextWallMessage.SpawnHallInfo o, GameObject sender)
    {
        Debug.Log("Creating Hallway");
        createHallway(o.lowerLeftCorner);
    }

    public void createHallway(Vector3 lowerLeftCorner)
    {
        var hallway = Instantiate(hallwayPrefab, lowerLeftCorner, Quaternion.identity);

        for (var zdiv = 0; zdiv < lengthDivisions; zdiv++)
        {
            for (var xdiv = 0; xdiv < widthDivisions; xdiv++)
            {
                Vector3 spawnPosition = hallway.transform.position + Vector3.forward * zdiv * gridSize + Vector3.right * xdiv * gridSize;
                var randomPrefab = Random.Range(0, hallwayObjectPrefabs.Count);

                if (Random.value <= 0.65f)
                {
                    var steps = Instantiate(hallwayObjectPrefabs[randomPrefab], spawnPosition, Quaternion.identity);
                    steps.transform.parent = hallway.transform;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.MessageSystemRegister();
    }

    private void OnDestroy()
    {
        this.MessageSystemUnregister();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
