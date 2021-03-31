using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{

    [SerializeField] private int m_width = 50;
    [SerializeField] private int m_height = 50;
    [SerializeField] private float m_gridOffset = 1.25f;
    [SerializeField] private GameObject m_cubePrefab;


    void Awake()
    {
        SpawnGrid();
    }

    void SpawnGrid()
    {
        for (int x = 0; x < m_width; x++)
        {

            for (int z = 0; z < m_height; z++)
            {
                float yCoord = GetYCoordinate(x, z);
                Instantiate(m_cubePrefab, new Vector3(x * m_gridOffset, yCoord, z * m_gridOffset), Quaternion.identity);
            }
        }
    }

    float GetYCoordinate(float x, float z)
    {
        return Mathf.PerlinNoise(x * 0.3f, z * 0.3f);
    }

}
