using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FinalCommon.Data;

public class GameManager : MonoBehaviour
{
    public Dictionary<ObjectIds, GameObject> Objects;

    public ObjectIds myId;
    public int opponentId;
    float speed = 10;

    FinalCommon.Data.Vector3 currentPosition;

    void Start()
    {
        Objects = new Dictionary<ObjectIds, GameObject>
        {
            [ObjectIds.Ball] = GameObject.FindGameObjectWithTag("Ball"),
            [ObjectIds.LeftPaddle] = GameObject.FindGameObjectWithTag("LeftPaddle"),
            [ObjectIds.RightPaddle] = GameObject.FindGameObjectWithTag("RightPaddle"),
            [ObjectIds.Camera] = GameObject.FindGameObjectWithTag("Camera")
        };
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W)) Objects[myId].transform.position += UnityEngine.Vector3.up * Time.deltaTime * speed;
        if (Input.GetKey(KeyCode.S)) Objects[myId].transform.position += UnityEngine.Vector3.down * Time.deltaTime * speed;
        NetworkManager.Instance.SendPosition(Objects[myId].transform.position);
    }

    public void MoveObjectHandler(FinalCommon.Data.Vector3 position, ObjectIds id)
    {
        Objects[id].transform.position = new UnityEngine.Vector3(position.X, position.Y, position.Z);
    }

    public void ResizeObjectHandler(FinalCommon.Data.Vector2 size, ObjectIds id)
    {
        Objects[id].transform.localScale = new UnityEngine.Vector2(size.X, size.Y);
    }

    public void SetOwnershipHandler(SetOwnershipData data)
    {
        myId = data.Id;
    }
}

