using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuMusic: MonoBehaviour
{
    public AudioClip music;
    private AudioSource audioSource;
    
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        audioSource.PlayOneShot(music);
    }
}
