using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundaries : MonoBehaviour
{
    public SpriteRenderer Self;
    public SpriteRenderer Bounds;

    void Update()
    {
        var min = Bounds.transform.position.y - Bounds.size.y / 2 + Self.size.y / 2;
        var max = Bounds.transform.position.y + Bounds.size.y / 2 - Self.size.y / 2;
        var pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, min, max);
        transform.position = pos;
    }
}
