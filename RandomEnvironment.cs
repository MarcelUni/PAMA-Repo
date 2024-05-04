using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RandomEnvironment : MonoBehaviour
{
    // Private variables
    private float addToZHospital = 15f;
    private float lastZ = 0;
    private int smallHouseLength = 0;
    private int bigHouseLenght = 0;
    private bool bigCity = false;
    private List<GameObject> RoadChunks = new List<GameObject>();
    private GameManager gameManager;
    private bool spawnOnce;

    // Inspector variables
    [Header ("Prefabs")]
    [SerializeField] private List<GameObject> smallHouses = new List<GameObject>();
    [SerializeField] private List<GameObject> bigHouses = new List<GameObject>();
    [SerializeField] private GameObject hospitalPrefab;

    [Header ("Settings")]
    [SerializeField] private float instantiateDistance = -100;
    [SerializeField] private float bigHouseThreshold = 250;
    [SerializeField] private float removeDistance = 25;
    [SerializeField] private GameObject environment;
    [SerializeField] private float xValueTweak = 0.6f;
    [SerializeField] private float hospitalYValueTweak = -3.64f;

    [Header ("Distance between center of chunks")]
    [SerializeField] private float addToZ = 0;

    void Start()
    {
        // Destroy all the children of the environment so we can start fresh
        // Used we can build a world to in the editor and then delete it before new environment is created
        foreach(Transform child in environment.transform)
        {
            Destroy(child.gameObject);
        }

        // Initialize variables
        spawnOnce = false;
        bigCity = false;
        lastZ = transform.position.z;

        addToZ = 23.83068f; // This is the distance between the center of each chunk
        // I couldn't find a way to calculate this distance so I had to measure it manually

        smallHouseLength = smallHouses.Count;
        bigHouseLenght = bigHouses.Count;

        gameManager = GameManager.instance;
    }
    void Update()
    {
        // Checks if the player is far enough to spawn big houses
        if(transform.position.z > bigHouseThreshold)
        {
            bigCity = true;
        }
        
        // If the list with big house prefabs is empty it wont change to big city
        if(bigHouses.Count == 0)
            bigCity = false;

        // Only spawns new chunks if the player has not won the game
        if(transform.position.z < gameManager.winDistance)
        {
            InstantiateChunks();
            RemoveChunks();
        }
        // If the player has won the game it will spawn the hospital
        else
            SpawnHospital();
    }
    private void RemoveChunks()
    {
        // Return if there are no chunks to remove to avoid errors
        if(RoadChunks.Count == 0)
            return;

        // If the chunk that is the first in the list, which is the one furthest back, is far enough behind the player it will be destroyed and removed from the list
        if(RoadChunks[0].transform.position.z < transform.position.z - removeDistance)
        {
            GameObject toRemove = RoadChunks[0];
            RoadChunks.RemoveAt(0);
            Destroy(toRemove);
        }
    }
    private void InstantiateChunks()
    {
        // If the players position minus the last instantiated chunks position, is greater than the instantiate distance, a new chunk will be instantiated
        if(transform.position.z - lastZ > instantiateDistance)
        {
            // The position of the new chunk is calculated
            Vector3 position = new Vector3(xValueTweak, 0, lastZ + addToZ);

            // Creating room in memory for the prefab that will be instantiated and choosing a prefab based on if it is a big city or not
            GameObject prefabtoInstantiate;
            if(bigCity)
                prefabtoInstantiate = bigHouses[Random.Range(0, bigHouseLenght)];
            else
                prefabtoInstantiate = smallHouses[Random.Range(0, smallHouseLength)];
            
            // Instantiating the new chunk 
            // Adding it to the list of chunks
            // Setting the lastZ to the new chunks position
            GameObject newChunk = Instantiate(prefabtoInstantiate, position, Quaternion.identity);
            newChunk.transform.parent = environment.transform;
            RoadChunks.Add(newChunk);
            lastZ = newChunk.transform.position.z;
        }
    }
    public void SpawnHospital()
    {
        // Calculate the position of the hospital
        Vector3 position = new Vector3(-xValueTweak, hospitalYValueTweak, lastZ + addToZHospital);

        // Instantiate the hospital in the same way as the chunks
        GameObject HospitalInstantiate = hospitalPrefab;
        GameObject newChunk = Instantiate(HospitalInstantiate, position, Quaternion.identity);

        newChunk.transform.parent = environment.transform;
        RoadChunks.Add(newChunk);
        lastZ = newChunk.transform.position.z;
    }

}
