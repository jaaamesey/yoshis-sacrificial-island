using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class TongueBall : MonoBehaviour
{
    public DistanceJoint2D TongueToPlayerDistanceJoint = null;
    public DistanceJoint2D TongueToObjectDistanceJoint = null;

    public Rigidbody2D Rb = null;
    
    [SerializeField]
    private List<Collider2D> _ignoredColliders = new List<Collider2D>();
    
    private List<Collider2D> _overlappingColliders = new List<Collider2D>();

    private PlayerController _player = null;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        Rb = GetComponent<Rigidbody2D>();
    }
    
    
    private void FixedUpdate()
    {
        print(_overlappingColliders);
        /*foreach (var other in _overlappingColliders)
        {
            
        }*/
        
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
