using System.Linq.Expressions;
using UnityEngine;

public class AnimalRevive : MonoBehaviour
{
    [SerializeField] private AudioSource reviveSound;

    private Animator animator;
    private bool playerInRange = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            GameData.AnimalsRevived += 1;
            reviveSound.Play();
            animator.SetBool("Revived", true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = false;
    }

    public void OnAnimationEnd()
    {
        Destroy(gameObject);
    }
}
