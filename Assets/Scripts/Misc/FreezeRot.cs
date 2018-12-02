using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeRot : MonoBehaviour
{
    public bool OnlyWhenVisible = true;
    
    private void FixedUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
    
    private void OnBecameVisible()
    {
        enabled = true;
    }
    
    private void OnBecameInvisible()
    {
        if (OnlyWhenVisible)
            enabled = false;
    }
}
