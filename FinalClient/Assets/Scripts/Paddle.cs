using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{  
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void setPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
    public Vector3 getPosition()
    {
        return transform.position;
    }
}
