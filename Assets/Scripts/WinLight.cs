using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLight : MonoBehaviour
{
    public GameObject spotLight;
    public GameObject pointLight;
    public GameObject lightBase;

    // Update is called once per frame
    void Update()
    {
        spotLight.transform.Rotate(transform.right, Time.deltaTime * 200);
    }
}
