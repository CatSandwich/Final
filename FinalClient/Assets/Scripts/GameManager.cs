using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FinalCommon.Data;

public class GameManager : MonoBehaviour
{
    public GameObject[] objects;

    public int myId;
    public int opponentId;
    float speed = 10;

    FinalCommon.Data.Vector3 currentPosition;


    void Start()
    {
        myId = 0;
    }

    void Update()
    {
        if (myId != 0)
        {
            if (Input.GetKey(KeyCode.W))
            {
                objects[myId].transform.position += UnityEngine.Vector3.up * Time.deltaTime * speed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                objects[myId].transform.position += UnityEngine.Vector3.down * Time.deltaTime * speed;
            }
            NetworkManager.Instance.sendPosition(myId, objects[myId].transform.position);
        }

    }

    public void MoveObjectHandler(FinalCommon.Data.Vector3 position, int id)
    {
        if (id == (int)ObjectIds.LeftPaddle && myId != (int)ObjectIds.RightPaddle)
        {
            myId = (int)ObjectIds.RightPaddle;
            Debug.Log("I am RightPaddle");
        }
        else if (id == (int)ObjectIds.RightPaddle && myId != (int)ObjectIds.LeftPaddle)
        {
            myId = (int)ObjectIds.LeftPaddle;
            Debug.Log("I am LeftPaddle");
        }

        UnityEngine.Vector3 position1 = new UnityEngine.Vector3(position.X, position.Y, position.Z);
        objects[id].transform.position = position1;

    }

    public void ResizeObjectHandler(FinalCommon.Data.Vector2 size, int id)
    {




    }
}

