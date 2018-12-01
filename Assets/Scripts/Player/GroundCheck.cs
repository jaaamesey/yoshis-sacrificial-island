using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class GroundCheck : MonoBehaviour
{
    [SerializeField]
    private List<Collider2D> _ignoredColliders = new List<Collider2D>();
    
    private List<Collider2D> _overlappingColliders = new List<Collider2D>();

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
}
