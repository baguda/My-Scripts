using UnityEngine;
using System.Collections.Generic;
using CogSim;



public class FloatMapGenerator : MonoBehaviour
{
    public int width = 100;
    public int height = 100;
    public float scale = 10f;

    public float waterThreshold = 0.3f;
    public float wallThreshold = 0.75f;
    private float[,] terrainField;
    private SimulationMap map;
    public GameObject MapParent;
    public bool isLevelGround = false;
    public bool hasWater = false;
    public List<GameObject> environmentObjects = new List<GameObject>();
    public SimulationMap Map => map;
    void Start()
    {
        GenerateTerrainField();
        this.map = GenerateMap();
        //SpawnMapObjects();
    }

    public void GenerateTerrainField()
    {
        terrainField = new float[width, height];
        float[,] fallOffField = GenerateFalloffMap(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float perlinValue = Mathf.PerlinNoise((float)x / width * scale, (float)y / height * scale);
                terrainField[x, y] = Mathf.Lerp(perlinValue, 1, fallOffField[x, y]); // perlinValue * fallOffField[x,y]; 
            }
        }
    }
    private float[,] GenerateFalloffMap(int width, int height)
    {
        float[,] falloffMap = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Normalize x and y to the range [0, 1]
                float normalizedX = (float)x / width * 2 - 1; // Range: [-1, 1]
                float normalizedY = (float)y / height * 2 - 1; // Range: [-1, 1]

                // Calculate the distance from the center
                float distanceFromCenter = Mathf.Max(Mathf.Abs(normalizedX), Mathf.Abs(normalizedY));

                // Apply a falloff function (e.g., quadratic)
                float falloffValue = Mathf.Pow(distanceFromCenter, 4); // You can adjust the exponent for sharper/softer falloff

                // Clamp the falloff value to [0, 1]
                falloffMap[x, y] = Mathf.Clamp01(falloffValue);
            }
        }

        return falloffMap;
    }
    public SimulationMap GenerateMap()
    {
        MapParent = new GameObject("Map Parent");
        SimulationMap newMap = new SimulationMap(width, height);
        newMap.Grid = this.terrainField;
        float Height = 0f;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                 Height = terrainField[x, y];
                if (terrainField[x,y] >= wallThreshold)
                {
                    if (isLevelGround)
                    {
                        Height = 1f;
                    }
                    else Height = 1.5f;
                }
                else if (hasWater && terrainField[x, y] <= waterThreshold)
                {
                    Height = 0f;
                }
                else if (isLevelGround)
                {
                    Height = 0.2f;
                }
                SpawnTile(new Vector3(x, Height, y), terrainField[x, y]);
            }
        }
        newMap.environmentObjects = this.environmentObjects;
        
        return newMap;
    }

    public void SpawnTile(Vector3 position, float terrainValue)
    {
        GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tile.transform.position = position;
        tile.name = "Tile: " + position.ToString();
        Renderer renderer = tile.GetComponent<Renderer>();
        if (hasWater && terrainValue <= waterThreshold)
        {
            renderer.material.color = Color.blue; // Water
            //tile.transform.position = position + new Vector3(0f, 0.9f, 0f);
        }
        else if(terrainValue >= wallThreshold)
        {
            renderer.material.color = Color.black;
        }
        else
        {
            renderer.material.color = new Color(117f / 256f, 66f / 256f, 38f / 256f, 1f);
        }
        tile.transform.SetParent(MapParent.transform, false);
        this.environmentObjects.Add(tile);
    }
    /*
    void SpawnMapObjects()
    {
        // Spawn Agent
        Vector2Int agentPosition = new Vector2Int(width / 2, height / 2);
        MapObject agent = new MapObject("Agent", agentPosition);
        mapObjects.Add(agentPosition, agent);
        SpawnObjectVisual(agentPosition, Color.red);

        // Spawn Food
        Vector2Int foodPosition = new Vector2Int(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
        MapObject food = new MapObject("Food", foodPosition);
        mapObjects.Add(foodPosition, food);
        SpawnObjectVisual(foodPosition, Color.yellow);
    }

    void SpawnObjectVisual(Vector2Int position, Color color)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.position = new Vector3(position.x, 1, position.y); // Slightly above the ground
        obj.GetComponent<Renderer>().material.color = color;
    }
    */
}