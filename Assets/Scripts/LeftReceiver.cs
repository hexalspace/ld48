using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LeftReceiver : MonoBehaviour, Receiver<LeftSender.Message>
{
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
    public void receive(LeftSender.Message message, GameObject sender)
    {
        Debug.Log($"{this.GetInstanceID()} {message}");
    }
}
