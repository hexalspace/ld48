using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftSender : MonoBehaviour
{
    public struct Message
    {
        public string Name;
        public int Age;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.MessageSystemPush(new Message() { Age = 2, Name = "hello" });
        }
    }
}
