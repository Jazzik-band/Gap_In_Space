using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Skip: MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Main menu");
        }
    }
}
