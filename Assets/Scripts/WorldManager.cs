using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml.Serialization;
using Mono.Cecil;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> animalPrefabs;
    [SerializeField] private List<TileBase> tiles;
    [SerializeField] private GameObject resourcesPrefab;
    [SerializeField] private GameObject animalsParent;
    [SerializeField] private GameObject resourcesParent;
    [SerializeField] private GameObject player;
    [SerializeField] private Tilemap tilemap;

    [SerializeField] private float worldSize = 100.0f;
    [SerializeField] private int worldBorderZone = 2; // range where spawn items and animal are not allowed
    [SerializeField] private int resourcesSpawnRadius = 10;  // range where resources can spawn (start count towards player position)
    //[SerializeField] private int resourcesSafeZoneRadius = 2; // range where resources cant spawn (start count towards player position)

    [SerializeField] private int animalsNumber = 10;
    [SerializeField] private int resourcesNumber = 20;
    [SerializeField] private float resourcesRespawnTime = 30; // seconds

    [SerializeField] private Text resourcesCount;
    [SerializeField] private Text animalsRevivedCount;
    [SerializeField] private Text endText;

    // for perlin noise
    [SerializeField] private float scale = 10f;
    private float offsetX = 1f;
    private float offsetY = 1f;

    private void Start()
    {
        offsetX = Random.Range(0f, 1000f);
        offsetY = Random.Range(0f, 1000f);

        TileMapGenerator();
        StartCoroutine(ResourcesSpawner());
        AnimalsSpawn();
    }

    private void Update()
    {
        resourcesCount.text = GameData.ResourcesCollected.ToString();
        animalsRevivedCount.text = GameData.AnimalsRevived.ToString();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameData.GameEnded)
            {
                Time.timeScale = 1f;

                GameData.GameEnded = false;
                GameData.AnimalsRevived = 0;
                GameData.ResourcesCollected = 0;

                FindAnyObjectByType<TimeBar>().Start();

                player.transform.position = Vector3.zero;

                endText.gameObject.SetActive(false);

                Start();
            }
        }
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

    private void TileMapGenerator()
    {
        // Perlin noise

        if (tilemap == null || tiles == null)
            return;

        for (int y = -(int)worldSize; y <= worldSize; y++)
        {
            for (int x = -(int)worldSize; x <= worldSize; x++)
            {
                float perlinValue = Mathf.PerlinNoise((x + offsetX) / scale, (y + offsetX) / scale);

                if (perlinValue >= 0.95 && perlinValue <= 1.0 || perlinValue >= 0.44 && perlinValue <= 0.50)
                    tilemap.SetTile(new Vector3Int(x, y, 0), tiles[Random.Range(0, tiles.Count)]);
                else if (perlinValue >= 0 && perlinValue <= 0.05 || perlinValue >= 0.62 && perlinValue <= 0.66)
                    tilemap.SetTile(new Vector3Int(x, y, 0), tiles[Random.Range(0, tiles.Count)]);
                else if (perlinValue >= 0.21 && perlinValue <= 0.25 || perlinValue >= 0.72 && perlinValue <= 0.79)
                    tilemap.SetTile(new Vector3Int(x, y, 0), tiles[Random.Range(0, tiles.Count)]);
                else if (perlinValue >= 0.10 && perlinValue <= 0.15 || perlinValue >= 0.82 && perlinValue <= 0.86)
                    tilemap.SetTile(new Vector3Int(x, y, 0), tiles[Random.Range(0, tiles.Count)]);
                else
                    tilemap.SetTile(new Vector3Int(x, y, 0), tiles[0]);
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
