using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayStarter : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        this.MessageSystemPush(new SendNextWallMessage.SpawnHallInfo { lowerLeftCorner = this.transform.position });
        this.MessageSystemPush(new SendNextWallMessage.SpawnHallInfo { lowerLeftCorner = this.transform.position - 20* Vector3.up });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
