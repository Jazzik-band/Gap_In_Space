using UnityEngine;

public class DeathSound: MonoBehaviour
{
    public AudioClip deathSound;
    private AudioSource audioSource;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(deathSound);
    }
}
