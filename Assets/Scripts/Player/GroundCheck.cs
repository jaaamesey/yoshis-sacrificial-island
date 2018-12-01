using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class GroundCheck : MonoBehaviour
{
    [SerializeField]
    private List<Collider2D> _ignoredColliders = new List<Collider2D>();
    
    private List<Collider2D> _overlappingColliders = new List<Collider2D>();

    private void FixedUpdate()
    {
        // Force consistent rotation
        //var transformRotation = transform.rotation;
        //transformRotation.z = 0;
        //transform.rotation = transformRotation;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_ignoredColliders.Contains(other))
            return;
        
        if (!_overlappingColliders.Contains(other))
            _overlappingColliders.Add(other);
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {       
        if (_overlappingColliders.Contains(other))
            _overlappingColliders.Remove(other);
    }

    public List<Collider2D> GetOverlappingColliders()
    {
        return _overlappingColliders;
    }

    public bool IsColliding()
    {
        return _overlappingColliders.Count > 0;
    }

    /*public Vector2 GetFloorNormal()
    {
        var hit = Physics2D.Raycast(transform.position, Vector2.down, 1000f);
        print(hit.point);
        return hit.normal;
    }*/
}
