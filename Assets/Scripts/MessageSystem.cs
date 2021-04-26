using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

public static class GameObjectExtensionMethods
{
    public static void MessageSystemPush<T>(this MonoBehaviour sender, T message)
    {
        MessageSystem.GetMessageSystem()?.pushMessage(message, sender.gameObject);
        return;
    }

    public static void MessageSystemRegister<U>(this U receiver) where U : MonoBehaviour
    {
        foreach (var inter in typeof(U).GetInterfaces().Where(
            i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(Receiver<>)))
        {
            var messageType = inter.GetGenericArguments().First();
            MessageSystem.GetMessageSystem()?.registerReceiver(messageType, receiver);
        }
    }

    public static void MessageSystemUnregister<U>(this U receiver) where U : MonoBehaviour
    {
        foreach (var inter in typeof(U).GetInterfaces().Where(
            i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(Receiver<>)))
        {
            var messageType = inter.GetGenericArguments().First();
            MessageSystem.GetMessageSystem()?.unregisterReceiver(messageType, receiver);
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
    private static bool isQuitting = false;
    private static int instanceCount = 0;

    public static bool IsQuitting()
    {
        return isQuitting;
    }

    public static MessageSystem GetMessageSystem()
    {
        // To avoid Unity errors during shutdown, GetMessageSystem should be called with ?
        if (theMessageSystem == null && IsQuitting())
        {
            Debug.Log("MessageSystem null on shutdown, expected behavior");
            return null;
        }
        else if (theMessageSystem == null && !IsQuitting())
        {
            var go = new GameObject("Message System");
            theMessageSystem = go.AddComponent<MessageSystem>();
            return theMessageSystem;
        }
        else
        {
            return theMessageSystem;
        }
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

    public MessageSystem()
    {
        instanceCount++;
        if (instanceCount > 1)
        {
            Debug.LogWarning("Singleton pattern broken");
        }
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

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
                Debug.LogWarning($"No one interested in {message} of type {message.messageType}");
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
            throw new System.Exception($"Should not register twice! {receiver.GetInstanceID()}");
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
            throw new System.Exception($"Bad deregister type {messageType}");
        }

        if (!actionById.Remove(receiver.GetInstanceID()))
        {
            throw new System.Exception($"Bad deregister instanceId {receiver.GetInstanceID()}");
        }
    }

    public void pushMessage<T>(T message, GameObject sender)
    {
        messageQueue.Enqueue(new MessageWrapper() { messageType = typeof(T), theMessage = message, sender = sender });
    }
}
