// Makes a UI or sprite object face the main camera (billboard effect).

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(mainCamera.transform);
        transform.Rotate(90,0,0);
        Vector3 euler = transform.eulerAngles;
        //euler.z = 90;
        transform.eulerAngles = euler;

    }
}
