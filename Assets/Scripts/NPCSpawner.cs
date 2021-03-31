using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour {

    [SerializeField] private int m_width = 50;
    [SerializeField] private int m_height = 50;
    [SerializeField] private GameObject[] buildings;

    private List<GameObject> houseList = new List<GameObject>();

    public void Awake() {
        SpawnGrid();
    }

    void SpawnGrid() {
        //Start instantiating on X
        for (int x = 0; x < m_width; x++) {

            //Instantiate m_height no of houses on Z
            for (int z = 0; z < m_height; z++) {
                int randRangeX = Random.Range(100, 900);
                int randRangeZ = Random.Range(100, 900);
                int rand = Random.Range(0, buildings.Length);
                GameObject houseInstance = Instantiate(buildings[rand], new Vector3(randRangeX, 100, randRangeZ), Quaternion.identity);
                houseInstance.transform.Rotate(0, Random.Range(0, 359), 0);
                houseList.Add(houseInstance);
            }
        }
    }
}
