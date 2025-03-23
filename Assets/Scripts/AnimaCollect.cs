using System.Collections;
using UnityEngine;

public class AnimaCollect : MonoBehaviour
{
    [SerializeField] private AudioSource collectSound;

    private bool enable = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && enable)
            StartCoroutine(DestroyAfterSound());
    }

    private IEnumerator DestroyAfterSound()
    {
        collectSound.Play();
        
        GetComponent<Renderer>().enabled = false;
        enable = false;

        GameData.ResourcesCollected += 1;

        yield return new WaitForSeconds(collectSound.clip.length);

        Destroy(gameObject);
    }
}
