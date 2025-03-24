using System.Linq.Expressions;
using System.Resources;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AnimalRevive : MonoBehaviour
{
    [SerializeField] private AudioSource reviveSound;
    [SerializeField] private int minumumRequiredResources;
    [SerializeField] private int maximumRequiredResources;

    private Animator animator;
    private Canvas resourcesCanvas;
    private Text resourcesText;
    private bool playerInRange = false;
    private int requiredResources = 0;
    private int recievedResources = 0;

    private void Awake()
    {
        resourcesCanvas = GameObject.Find("AnimalResourcesCanvas").GetComponent<Canvas>();
        resourcesText = resourcesCanvas.transform.Find("Resources").GetComponent<Text>();
        animator = GetComponent<Animator>();

        requiredResources = Random.Range(minumumRequiredResources, maximumRequiredResources);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Reviving();
        }
    }

    private void Reviving()
    {
        while (recievedResources < requiredResources && GameData.ResourcesCollected != 0)
        {
            GameData.ResourcesCollected -= 1;
            recievedResources += 1;
            resourcesText.text = recievedResources + "/" + requiredResources;
        }

        if (requiredResources == recievedResources)
        {
            resourcesText.gameObject.SetActive(false);
            GameData.AnimalsRevived += 1;
            reviveSound.Play();
            animator.SetBool("Revived", true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            resourcesCanvas.transform.SetParent(gameObject.transform, true);
            resourcesCanvas.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f * transform.localScale.x, 0);
            resourcesText.text = recievedResources + "/" + requiredResources;
            resourcesText.gameObject.SetActive(true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            resourcesText.gameObject.SetActive(false);
            playerInRange = false;
        }
    }

    public void OnAnimationEnd()
    {
        resourcesCanvas.transform.SetParent(null);
        Destroy(gameObject);
    }
}
