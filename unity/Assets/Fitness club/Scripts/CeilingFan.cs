using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingFan : MonoBehaviour
{

    public float speed = 50f;

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.Rotate(0, -speed * Time.deltaTime, 0);
    }
}
