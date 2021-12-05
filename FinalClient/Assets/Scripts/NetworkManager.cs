using System.Collections;
using DarkRift;
using DarkRift.Client;
using FinalCommon;
using FinalCommon.Data;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public DarkRift.Client.Unity.UnityClient Client;
    public GameManager gameManager;
    public static NetworkManager _instance = null;

    public static NetworkManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<NetworkManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("NetworkManager");
                    _instance = go.AddComponent<NetworkManager>();
                }
            }
            return _instance;
        }
    }
    IEnumerator Start()
    {
        Client.MessageReceived += _messageReceivedHandler;

        while (Client.ConnectionState != ConnectionState.Connected)
        {
            yield return null;
        }

        using (var writer = DarkRiftWriter.Create())
        {
            //writer.Write(0);

            //Client.SendMessage(Message.Create(0, writer), SendMode.Reliable);
            Client.MessageReceived += _messageReceivedHandler;
        }
    }

    private void _messageReceivedHandler(object sender, MessageReceivedEventArgs args)
    {
        using (Message message = args.GetMessage() as Message)
        {
            if (message.Tag == (ushort)ServerToClient.MoveObject)
            {
                using (DarkRiftReader reader = message.GetReader())
                {
                    MoveObjectData data = reader.ReadSerializable<MoveObjectData>();
                    gameManager.MoveObjectHandler(data.Position, (int)data.Id);
                }
            }
        }
    }
    public void sendPosition(int id, UnityEngine.Vector3 position)
    {

        Debug.Log("Trying to send");
        using (var writer = DarkRiftWriter.Create())
        {

            Debug.Log("Sending");
            PaddlePositionData data = new PaddlePositionData();
            data.Position = new FinalCommon.Data.Vector3 { X = position.x, Y = position.y, Z = position.z };
                
            writer.Write(data);
            Client.SendMessage(Message.Create((ushort)ClientToServer.PaddlePosition, writer), SendMode.Reliable);
        }
    }
}
