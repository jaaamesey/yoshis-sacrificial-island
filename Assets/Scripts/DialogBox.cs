using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogBox : MonoBehaviour
{
    public bool CanSkip = true;

    // Start is called before the first frame update
    private void Start()
    {
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

    public void CloseDialog()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }
}