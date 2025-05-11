using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal: MonoBehaviour
{
    [SerializeField] private float teleportDelay = 0.5f;

    private bool isPlayerInside;
    public string currentLevel;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            Invoke(nameof(TeleportPlayer), teleportDelay);
        }
    }

    private void Update()
    {
        currentLevel = SceneManager.GetActiveScene().name;
    }

    private void TeleportPlayer()
    {
        if (isPlayerInside)
        {
            switch (currentLevel)
            {
                case "Game": SceneManager.LoadScene("Game2"); break;
                case "Game2": SceneManager.LoadScene("Game3"); break;
                case "Game3": SceneManager.LoadScene("Game4"); break;
                case "Game4": SceneManager.LoadScene("Game5"); break;
            }
        }
    }
}
