using UnityEngine;

public class MainMenuMusic: MonoBehaviour
{
    public AudioClip[] sounds;
    private AudioSource audioSrc => GetComponent<AudioSource>();

    public void PlaySound(AudioClip clip, float volume, bool destroyed, float p1 = 0.85f, float p2 = 1.2f)
    {
        audioSrc.PlayOneShot(clip, volume);
    }
}
