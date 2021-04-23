using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

public static class GameObjectExtensionMethods
{
    public static void MessageSystemPush<U, T>(this U sender, T message) where U : MonoBehaviour
    {
        MessageSystem.GetMessageSystem().pushMessage(message, sender.gameObject);
        return;
    }

    public static void MessageSystemRegister<U>(this U receiver) where U : MonoBehaviour
    {
        foreach (var inter in typeof(U).GetInterfaces().Where(
            i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(Receiver<>)))
        {
            var messageType = inter.GetGenericArguments().First();
            MessageSystem.GetMessageSystem().registerReceiver(messageType, receiver);
        }
    }

    public static void MessageSystemUnregister<U>(this U receiver) where U : MonoBehaviour
    {
        foreach (var inter in typeof(U).GetInterfaces().Where(
            i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(Receiver<>)))
        {
            var messageType = inter.GetGenericArguments().First();
            MessageSystem.GetMessageSystem().unregisterReceiver(messageType, receiver);
        }
    }
}

public interface Receiver<T>
{
    public void receive(T o, GameObject sender);
}

public class MessageSystem : MonoBehaviour
{
    private static MessageSystem theMessageSystem;

    public static MessageSystem GetMessageSystem()
    {
        if (theMessageSystem == null)
        {
            theMessageSystem = FindObjectOfType<MessageSystem>();
        }
        return theMessageSystem;
    }

    private struct MessageWrapper
    {
        public System.Object theMessage;
        public System.Type messageType;
        public GameObject sender;
    }

    private delegate void MessageSendAction(MessageWrapper mw);

    private Queue<MessageWrapper> messageQueue = new Queue<MessageWrapper>();
    private Dictionary<System.Type, MethodInfo> invokeType = new Dictionary<System.Type, MethodInfo>();
    private Dictionary<System.Type, Dictionary<int, MessageSendAction>> messageDelegates = new Dictionary<System.Type, Dictionary<int, MessageSendAction>>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        while (messageQueue.Count > 0)
        {
            var message = messageQueue.Dequeue();
            Dictionary<int, MessageSendAction> actionById = null;
            if (messageDelegates.TryGetValue(message.messageType, out actionById))
            {
                foreach (KeyValuePair<int, MessageSendAction> kvp in actionById)
                {
                    kvp.Value.Invoke(message);
                }
            }
            else
            {
                Debug.LogWarning($"No one interested in {message}");
            }
        }
    }

    public void registerReceiver<U>(System.Type messageType, U receiver) where U : MonoBehaviour
    {
        MethodInfo methodInfo = null;
        if (!invokeType.TryGetValue(messageType, out methodInfo))
        {
            var receiverType = typeof(Receiver<>).MakeGenericType(messageType);
            invokeType[messageType] = receiverType.GetMethod("receive");
            methodInfo = invokeType[messageType];
        }

        Dictionary<int, MessageSendAction> actionById = null;
        if (!messageDelegates.TryGetValue(messageType, out actionById))
        {
            messageDelegates[messageType] = new Dictionary<int, MessageSendAction>();
            actionById = messageDelegates[messageType];
        }

        if (actionById.ContainsKey(receiver.GetInstanceID()))
        {
            throw new System.Exception("Should not register twice!");
        }

        actionById[receiver.GetInstanceID()] = (MessageWrapper mw) =>
        {
            methodInfo.Invoke(receiver, new object[] { mw.theMessage, mw.sender });
        };
    }

    public void unregisterReceiver<U>(System.Type messageType, U receiver) where U : MonoBehaviour
    {
        Dictionary<int, MessageSendAction> actionById = null;
        if (!messageDelegates.TryGetValue(messageType, out actionById))
        {
            throw new System.Exception("Bad deregister type");
        }

        if (!actionById.Remove(receiver.GetInstanceID()))
        {
            throw new System.Exception("Bad deregister instanceId");
        }
    }

    public void pushMessage<T>(T message, GameObject sender)
    {
        this.messageQueue.Enqueue(new MessageWrapper() { messageType = typeof(T), theMessage = message, sender = sender });
    }
}
