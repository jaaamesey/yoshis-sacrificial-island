using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPentagram : MonoBehaviour
{
    public int SacrificesNeeded = 3;
    
    [SerializeField]
    private List<Collider2D> _ignoredColliders = new List<Collider2D>();
    
    private List<Collider2D> _overlappingColliders = new List<Collider2D>();

    private PlayerController _player = null;
    private SpriteRenderer _bigBlack = null;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _bigBlack = GameObject.FindWithTag("BigBlack").GetComponent<SpriteRenderer>();

    }
    
    
    private void FixedUpdate()
    {
        var shyGuyCount = 0;
        foreach (var other in _overlappingColliders)
        {
            if (other.gameObject.GetComponent<ShyGuy>() != null)
                shyGuyCount++;
        }

        if (shyGuyCount >= SacrificesNeeded)
        {
            var bigBlackColor = _bigBlack.color;
            bigBlackColor.a += Time.fixedDeltaTime;
            _bigBlack.color = bigBlackColor;
            if (bigBlackColor.a >= 0.99f)
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
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
