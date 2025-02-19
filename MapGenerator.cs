using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using CogSim;

public class MapGenerator : MonoBehaviour
{
    public int width = 50;
    public int height = 50;
    public float scale = 20f;

    private int[,] terrainField;
    private Dictionary<Vector2Int, MapObject> mapObjects = new Dictionary<Vector2Int, MapObject>();

    void Start()
    {
        GenerateTerrainField();
        GenerateMap();
        //SpawnMapObjects();
    }

    void GenerateTerrainField()
    {
        terrainField = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float perlinValue = Mathf.PerlinNoise((float)x / width * scale, (float)y / height * scale);

                terrainField[x, y] = Mathf.RoundToInt(perlinValue * 2); // Scale to 0, 1, 2
            }
        }
    }

    void GenerateMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x, terrainField[x, y], y); // X and Z for 2D grid
                SpawnTile(position, terrainField[x, y]);
            }
        }
    }

    void SpawnTile(Vector3 position, int terrainValue)
    {
        GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tile.transform.position = position;

        Renderer renderer = tile.GetComponent<Renderer>();
        switch (terrainValue)
        {
            case 0:
                renderer.material.color = Color.blue; // Water
                tile.transform.position = position + new Vector3(0f,0.9f,0f);
                break;
            case 1:
                renderer.material.color = new Color(117f/256f,66f/256f,38f/256f,1f); // Ground
                break;
            case 2:
                renderer.material.color = Color.black; // Wall
                break;
        }
    }

   
}



