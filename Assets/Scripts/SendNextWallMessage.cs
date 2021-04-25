using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendNextWallMessage : MonoBehaviour
{
    public struct SpawnHallInfo
    {
        public Vector3 lowerLeftCorner;
    }

    public Transform upperLeftCorner;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        this.MessageSystemPush(new SpawnHallInfo { lowerLeftCorner = upperLeftCorner.position });
    }
}
