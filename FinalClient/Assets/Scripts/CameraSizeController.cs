using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSizeController : MonoBehaviour
{
    public GameObject Background;

    // Update is called once per frame
    void Update()
    {
        var scale = transform.localScale;
        if (scale != Vector3.one)
        {
            transform.localScale = Vector3.one;
            Background.GetComponent<SpriteRenderer>().size = scale;
        }
    }
}
