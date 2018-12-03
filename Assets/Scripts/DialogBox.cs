using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
    public bool CanSkip = true;

    private Text _text = null;
    
    // Start is called before the first frame update
    private void Start()
    {
        _text = transform.Find("Text").GetComponent<Text>();
    }

    // Update is called once per frame
    private void Update()
    {
        Time.timeScale = 0f;
        if (CanSkip && Input.GetButtonDown("Jump"))
        {
            CloseDialog();
        }
    }

    public void ShowDialog(string text)
    {
        _text.text = text;
        gameObject.SetActive(true);
    }
    
    public void CloseDialog()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }
}