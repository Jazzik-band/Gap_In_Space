using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal: MonoBehaviour
{
    [SerializeField] private float teleportDelay = 0.5f;
    [SerializeField] private GameObject player;

    private bool isPlayerInside;
    private OrbHandler orbHandler;
    public string currentLevel;

    private void Start()
    {
        orbHandler = player.GetComponent<OrbHandler>();
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
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
            orbHandler.ResetOrbs();
            switch (currentLevel)
            {
                case "Education": SceneManager.LoadScene("Main menu"); break;
                case "Game": SceneManager.LoadScene("Game2"); break;
                case "Game2": SceneManager.LoadScene("Game3"); break;
                case "Game3": SceneManager.LoadScene("FinalCutScene"); break;
            }
        }
    }
}
