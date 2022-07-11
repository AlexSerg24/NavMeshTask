using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera cam;
    void Start()
    {
        cam = FindObjectOfType<Camera>();
    }

    // задание направления билборда бота по направлению к камере
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.transform.forward);
    }
}
