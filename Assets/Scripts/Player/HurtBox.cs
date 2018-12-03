using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class HurtBox : MonoBehaviour
{
    [SerializeField]
    private List<Collider2D> _ignoredColliders = new List<Collider2D>();
    
    private List<Collider2D> _overlappingColliders = new List<Collider2D>();

    private PlayerController _player = null;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }
    
    
    private void FixedUpdate()
    {
        foreach (var other in _overlappingColliders)
        {
            // Handle getting hurt
            var shouldHurt = other.CompareTag("Enemy") && !other.GetComponent<ShyGuy>().IsDead;
            shouldHurt = shouldHurt || other.CompareTag("Enemy") && other.GetComponent<ShyGuy>() == null;

            var dir = -(other.transform.position - _player.transform.position).normalized;
        
            if (shouldHurt)
                _player.Hurt(dir);
        }
        
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
