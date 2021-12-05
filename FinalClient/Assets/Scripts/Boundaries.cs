using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundaries : MonoBehaviour
{
    // Followed this tutorial: https://pressstart.vip/tutorials/2018/06/28/41/keep-object-in-bounds.html

    public Camera MainCamera;
    private Vector2 screenLimits;
    private float objectWidth;
    private float objectHeight;

    void Start()
    {
        screenLimits = MainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, MainCamera.transform.position.z));
        objectWidth = transform.GetComponent<SpriteRenderer>().bounds.extents.x;
        objectHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y;
    }

    void LateUpdate()
    {
        Vector3 viewPos = transform.position;
        viewPos.x = Mathf.Clamp(viewPos.x, screenLimits.x * -1 + objectWidth, screenLimits.x - objectWidth);
        viewPos.y = Mathf.Clamp(viewPos.y, screenLimits.y * -1 + objectHeight, screenLimits.y - objectHeight);
        transform.position = viewPos;
    }
}
