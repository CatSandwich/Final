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
    void Start()
    {
        Client.MessageReceived += _messageReceivedHandler;
    }

    private void _messageReceivedHandler(object sender, MessageReceivedEventArgs args)
    {
        switch ((ServerToClient)args.Tag)
        {
            case ServerToClient.ResizeObject:
            {
                var data = args.GetMessage().GetReader().ReadSerializable<ResizeObjectData>();
                gameManager.ResizeObjectHandler(data.Size, data.Id);
                return;
            }
            case ServerToClient.MoveObject:
            {
                var data = args.GetMessage().GetReader().ReadSerializable<MoveObjectData>();
                gameManager.MoveObjectHandler(data.Position, data.Id);
                return;
            }
            case ServerToClient.SetOwnership:
            {
                var data = args.GetMessage().GetReader().ReadSerializable<SetOwnershipData>();
                gameManager.SetOwnershipHandler(data);
                return;
            }
        }
    }
    
    public void SendPosition(UnityEngine.Vector3 position)
    {
        using var writer = DarkRiftWriter.Create();
        var data = new PaddlePositionData
        {
            Position = new FinalCommon.Data.Vector3 { X = position.x, Y = position.y, Z = position.z }
        };

        writer.Write(data);
        Client.SendMessage(Message.Create((ushort)ClientToServer.PaddlePosition, writer), SendMode.Unreliable);
    }
}
