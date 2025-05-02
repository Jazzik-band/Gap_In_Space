using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal: MonoBehaviour
{
    [SerializeField] public string targetSceneName = "Game2";
    [SerializeField] private float teleportDelay = 1f;

    private bool isPlayerInside = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            Invoke(nameof(TeleportPlayer), teleportDelay);
        }
    }

    private void TeleportPlayer()
    {
        if (isPlayerInside && !string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
