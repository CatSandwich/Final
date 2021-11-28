using System.Collections;
using DarkRift;
using DarkRift.Client;
using FinalServer.Data;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public DarkRift.Client.Unity.UnityClient Client;
    
    IEnumerator Start()
    {
        Client.MessageReceived += _messageReceivedHandler;
        
        while (Client.ConnectionState != ConnectionState.Connected)
        {
            yield return null;
        }
        
        using var writer = DarkRiftWriter.Create();
        writer.Write(0);
        
        Client.SendMessage(Message.Create(0, writer), SendMode.Reliable);
    }

    private void _messageReceivedHandler(object sender, MessageReceivedEventArgs args)
    {
        Debug.Log($"Tag: {args.Tag}");
        var testData = args.GetMessage().GetReader().ReadSerializable<TestData>();
        Debug.Log($"Name: {testData.FirstName} {testData.LastName} Age: {testData.Age}");
    }
}
