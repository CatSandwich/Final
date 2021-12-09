using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRendererHack : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        var size = transform.localScale;
        if (size != Vector3.one)
        {
            transform.localScale = Vector3.one;
            GetComponent<SpriteRenderer>().size = size;
        }
    }
}
