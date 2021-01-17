using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.LookAt(Camera.main.transform, Vector3.left);
        //transform.LookAt(target, Vector3.left);
    }
    private void Update()
    {
        transform.LookAt(Camera.main.transform, Vector3.forward);
    }
}
