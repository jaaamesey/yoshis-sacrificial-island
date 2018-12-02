using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float Speed = 2.5f;
    private void FixedUpdate()
    {
        var transformRotation = transform.rotation;
        var transformRotationEulerAngles = transformRotation.eulerAngles;
        transformRotationEulerAngles.z += Speed;
        transformRotation = Quaternion.Euler(transformRotationEulerAngles);
        transform.rotation = transformRotation;
    }
}
