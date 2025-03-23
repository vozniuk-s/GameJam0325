using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml.Serialization;
using Mono.Cecil;
using System.Collections;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> animalPrefabs;
    [SerializeField] private GameObject resourcesPrefab;
    [SerializeField] private GameObject animalsParent;
    [SerializeField] private GameObject resourcesParent;
    [SerializeField] private GameObject player;

    [SerializeField] private float worldSize = 100.0f;
    [SerializeField] private int worldBorderZone = 2; // range where spawn items and animal are not allowed
    [SerializeField] private int resourcesSpawnRadius = 10;  // range where resources can spawn (start count towards player position)
    //[SerializeField] private int resourcesSafeZoneRadius = 2; // range where resources cant spawn (start count towards player position)

    [SerializeField] private int animalsNumber = 10;
    [SerializeField] private int resourcesNumber = 20;
    [SerializeField] private float resourcesRespawnTime = 30; // seconds

    [SerializeField] private Text resourcesCount;
    [SerializeField] private Text animalsRevivedCount;

    private void Start()
    {
        StartCoroutine(ResourcesSpawner());
        AnimalsSpawn();
    }

    private void Update()
    {
        resourcesCount.text = GameData.ResourcesCollected.ToString();
        animalsRevivedCount.text = GameData.AnimalsRevived.ToString();
    }
    private IEnumerator ResourcesSpawner()
    {
        while (true)
        {
            ResourcesSpawn();

            yield return new WaitForSeconds(resourcesRespawnTime);

            DeleteAllResources();
        }
    }

    private void DeleteAllResources()
    {
        foreach (Transform child in resourcesParent.transform)
            Destroy(child.gameObject);
    }

    private void ResourcesSpawn()
    {
        int count = 0;

        while (count < resourcesNumber)
        {
            float leftRange = Mathf.Max(player.transform.position.x - resourcesSpawnRadius, -worldSize + worldBorderZone);
            float rightRange = Mathf.Min(player.transform.position.x + resourcesSpawnRadius, worldSize - worldBorderZone);
            float bottomRange = Mathf.Max(player.transform.position.y - resourcesSpawnRadius, -worldSize + worldBorderZone);
            float topRange = Mathf.Min(player.transform.position.y + resourcesSpawnRadius, worldSize - worldBorderZone);

            float x = Random.Range(leftRange, rightRange);
            float y = Random.Range(bottomRange, topRange);

            Vector3 pos = new Vector3(x, y, 0);

            if (isCoordsAllowedToUse(pos, 0.5f))
            {
                if (resourcesPrefab != null)
                {
                    GameObject resource = Instantiate(resourcesPrefab, pos, Quaternion.identity);

                    if (resourcesParent != null)
                        resource.transform.SetParent(resourcesParent.transform, false);

                    ++count;
                }
            }
        }
    }

    private void AnimalsSpawn()
    {
        int count = 0;

        while (count < animalsNumber)
        {
            float x = Random.Range(0 - worldSize + worldBorderZone, worldSize - worldBorderZone);
            float y = Random.Range(0 - worldSize + worldBorderZone, worldSize - worldBorderZone);
            Vector3 pos = new Vector3(x, y, 0);

            if(isCoordsAllowedToUse(pos, 2))
            {
                if (animalPrefabs != null)
                {
                    GameObject animal = Instantiate(animalPrefabs[Random.Range(0, animalPrefabs.Count)], pos, Quaternion.identity);

                    if(animalsParent != null)
                        animal.transform.SetParent(animalsParent.transform, false);
                        
                    ++count;
                }
            }
        }
    }

    private bool isCoordsAllowedToUse(Vector3 coords, float safeZone)
    {
        foreach(Transform child in resourcesParent.transform)
        {
            if (Vector3.Distance(coords, child.position) < safeZone)
                return false;
        }

        foreach (Transform child in animalsParent.transform)
        {
            if (Vector3.Distance(coords, child.position) < safeZone)
                return false;
        }

        return true;

    }
}
