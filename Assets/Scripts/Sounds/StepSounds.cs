using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class StepSounds: MonoBehaviour
{
    public AudioClip[] stepSounds;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StepSoundPlay()
    {
        audioSource.PlayOneShot(stepSounds[Random.Range(0, stepSounds.Length)]);
    }
}
