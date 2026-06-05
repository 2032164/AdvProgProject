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
        transform.Rotate(0,90,90);//(clockwise),(idk,left-right?),(fallback-facing up to the player)x,y,z
        Vector3 euler = transform.eulerAngles;
        euler.z = 90;
        transform.eulerAngles = euler;

    }
}
