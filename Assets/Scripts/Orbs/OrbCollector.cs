using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OrbCollector : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private AudioClip sound;
    
    private OrbHandler orbHandler;
    private AudioSource source;
    
    private void Start()
    {
        source = GetComponent<AudioSource>();
        orbHandler = player.GetComponent<OrbHandler>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            source.PlayOneShot(sound);
            orbHandler.IncreaseOrbs();
            transform.GetChild(0).gameObject.SetActive(false);
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = false;
            GetComponent<Light2D>().enabled = false;
            Destroy(gameObject, sound.length);
        }
    }
}
