using UnityEngine;
using CogSim;
using System.Linq;
using System.Collections.Generic;
using System;

namespace CogSim
{


    public class SimulationManager : MonoBehaviour
    {
        public int width = 50;
        public int height = 50;
        public float scale = 20f;
        GameObject MapParent;
        public float waterThreshold = 0.3f;
        public float wallThreshold = 0.75f;
        private float[,] terrainField;
        private SimulationMap map;

        public bool isLevelGround = false;
        public bool hasWater = false;
        public List<GameObject> environmentObjects = new List<GameObject>();
        public GameObject foodObject_mod;

        private MapObjectIdentificationHandler IdMaker;
        public SimulationMap Map
        {
            get
            {
                
                if (map == null)
                {
                    this.map=this.GenerateMap();
                }
                return map;
            }
        }

        void Start()
        {
            IdMaker = new MapObjectIdentificationHandler();
            this.map = GenerateMap();
            SpawnAgent(new Vector3Int(50, 1, 50),Color.yellow);
            SpawnFoods(Color.green);
            
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
                    float falloffValue = Mathf.Pow(distanceFromCenter, 4); 

                    // Clamp the falloff value to [0, 1]
                    falloffMap[x, y] = Mathf.Clamp01(falloffValue);
                }
            }

            return falloffMap;
        }
        public SimulationMap GenerateMap()
        {
            MapParent = new GameObject("Map Parent");
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

            SimulationMap newMap = new SimulationMap(width, height);
            newMap.Grid = this.terrainField;
            float Height = 0f;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Height = terrainField[x, y];
                    if (terrainField[x, y] >= wallThreshold)
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
            else if (terrainValue >= wallThreshold)
            {
                renderer.material.color = Color.black;
            }
            else
            {
                renderer.material.color = new Color(117f / 256f, 66f / 256f, 38f / 256f, 1f);
            }
            this.environmentObjects.Add(tile);
            tile.transform.SetParent(MapParent.transform, false);
        }


        public void SpawnAgent(Vector3Int location, Color color)
        {
            GameObject agentObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            agentObj.transform.position = new Vector3(location.x, Map.TerrainAt(location) + 1f, location.z); 
            agentObj.GetComponent<Renderer>().material.color = color;
            var agent = new AgentObject(agentObj, IdMaker.GenerateIndex("Agent"), location);
            agent.Position = location;
            agentObj.AddComponent<AgentObjectComponent>().Initialize(agent,agentObj,this);
            agentObj.GetComponent<AgentObjectComponent>().targetLocation = location;
            agentObj.AddComponent<Rigidbody>();
            agentObj.GetComponent<Rigidbody>().useGravity = false;
            Map.RegisterMapObject(agent);
        }
        public void SpawnFood(Vector3Int location,int value, Color color)
        {

            GameObject foodObj = GameObject.CreatePrimitive(PrimitiveType.Capsule); //Instantiate<GameObject>(this.foodObject_mod,location.ToVector3(),Quaternion.identity);
            foodObj.transform.position = new Vector3(location.x, Map.TerrainAt(location) + 1f, location.z); // Slightly above the ground
            
            foodObj.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            foodObj.GetComponent<Renderer>().material.color = color;
            var food = new ResourceObject(foodObj, IdMaker.GenerateIndex("Food"), location, value);
            foodObj.AddComponent<ResourceObjectComponent>().Initialize(food,foodObj);
            foodObj.AddComponent<Rigidbody>();
            foodObj.GetComponent<Rigidbody>().useGravity = false;
            foodObj.GetComponent<Collider>().isTrigger = true;
            Map.RegisterMapObject(food);
        }
        public bool IsWalled(Vector3Int location)
        {
            if (Map.TerrainAt(location) >= wallThreshold) return true;
            return false;

        }
        public bool IsWatered(Vector3Int location)
        {
            if (Map.TerrainAt(location) <= waterThreshold && hasWater) return true;
            return false;

        }
        public bool IsWithinMap(Vector3Int location)
        {
            if (location.x < 0 || location.z < 0) return false;
            if (location.x > width || location.z > height) return false;
            return true;
        }
        public bool IsPathable(Vector3Int location)
        {
            if (!IsWithinMap(location) || IsWalled(location) || IsWatered(location)) 
            {
                return false; 
            }
            return true;
        }

        public void SpawnFoods(Color color)
        {
            foreach(var loc in GetFoodSpawnLocations(map.Grid, IsNextToExtrema))
            {
                if(UnityEngine.Random.value > 0.0f)
                {
                    SpawnFood(loc, 100, color);
                }
            }
        }
        public HashSet<Vector3Int> GetFoodSpawnLocations(float[,] map, Func<Vector3Int, float[,], bool> criteria)
        {
            HashSet<Vector3Int> eligibleCells = new HashSet<Vector3Int>();

            int width = map.GetLength(0);
            int height = map.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Vector3Int cell = new Vector3Int(x, 1, z); 
                    if (criteria(cell, map) && UnityEngine.Random.value < 0.5f && !(IsWalled(cell) || IsWatered(cell)))
                    {
                        eligibleCells.Add(cell);
                    }
                }
            }

            return eligibleCells;
        }

        
        private bool IsNextToWall(Vector3Int cell, float[,] map)
        {
            int x = cell.x;
            int z = cell.z;
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            // Check if the cell is within 1 space from a wall
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    int nx = x + dx;
                    int nz = z + dz;
                    if (nx >= 0 && nx < width && nz >= 0 && nz < height && IsWalled(new Vector3Int(nx,1,nz)) && !IsWalled(cell))
                    {
                        Debug.Log("SimulationManager.IsEligibleForFood: " + cell.ToString());
                        return true;
                    }
                }
            }
            return false;
        }
        private bool IsNextToWater(Vector3Int cell, float[,] map)
        {
            int x = cell.x;
            int z = cell.z;
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            // Check if the cell is within 1 space from a wall
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    int nx = x + dx;
                    int nz = z + dz;
                    if (nx >= 0 && nx < width && nz >= 0 && nz < height && IsWatered(new Vector3Int(nx, 1, nz)) && !IsWatered(cell))
                    {
                        Debug.Log("SimulationManager.IsEligibleForFood: " + cell.ToString());
                        return true;
                        /*
                        // Count walls within a 4 cell radius
                        int wallCount = 0;
                        for (int i = -4; i <= 4; i++)
                        {
                            for (int j = -4; j <= 4; j++)
                            {
                                if (i * i + j * j <= 16) // Check if within a 4-cell radius (4^2=16)
                                {
                                    int nearX = nx + i;
                                    int nearZ = nz + j;
                                    if (nearX >= 0 && nearX < width && nearZ >= 0 && nearZ < height && map[nearX, nearZ] > 1)
                                    {
                                        wallCount++;
                                        if (wallCount >= 4) return true;  // If we've found 4 or more walls, this cell is eligible
                                    }
                                }
                            }
                        }
                        */
                    }
                }
            }
            return false;
        }
        private bool IsNextToExtrema(Vector3Int cell, float[,] map)
        {
            int x = cell.x;
            int z = cell.z;
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            float centerValue = map[x, z];

            bool allGreater = true;
            bool allLesser = true;

            // Check all 8 surrounding cells
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    if (dx == 0 && dz == 0) continue; // Skip the cell itself

                    int nx = x + dx;
                    int nz = z + dz;

                    // Check if the neighbor is within the map bounds
                    if (nx >= 0 && nx < width && nz >= 0 && nz < height)
                    {
                        float neighborValue = map[nx, nz];

                        // Update flags based on comparison with central cell
                        if (neighborValue >= centerValue)
                        {
                            allLesser = false;
                        }
                        if (neighborValue <= centerValue)
                        {
                            allGreater = false;
                        }
                    }
                    else
                    {
                        // If a neighbor is out of bounds, we can't consider this cell an extrema
                        return false;
                    }
                }
            }

            // If all neighbors are either all greater or all lesser than the center, it's an extrema
            return (allGreater || allLesser) && !(IsWalled(cell) || IsWatered(cell));
        }
        private static bool IsNextToExtrema(Vector3Int cell, float[,] map, int requiredCount, ExtremumType extremumType)
        {
            int x = cell.x;
            int z = cell.z;
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            float centerValue = map[x, z];

            int countGreater = 0;
            int countLesser = 0;
            int validNeighbors = 0;

            // Check all 8 surrounding cells (but we'll only consider the 4 orthogonal neighbors for this example)
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    if (dx == 0 && dz == 0) continue; // Skip the cell itself
                    if (Mathf.Abs(dx) + Mathf.Abs(dz) > 1) continue; // Only check orthogonal neighbors for simplicity

                    int nx = x + dx;
                    int nz = z + dz;

                    // Check if the neighbor is within the map bounds
                    if (nx >= 0 && nx < width && nz >= 0 && nz < height)
                    {
                        float neighborValue = map[nx, nz];
                        validNeighbors++;

                        if (neighborValue > centerValue) countGreater++;
                        else if (neighborValue < centerValue) countLesser++;
                    }
                }
            }

            switch (extremumType)
            {
                case ExtremumType.Maximum:
                    return countGreater >= requiredCount;
                case ExtremumType.Minimum:
                    return countLesser >= requiredCount;
                case ExtremumType.Both:
                    return countGreater >= requiredCount || countLesser >= requiredCount;
                default:
                    throw new ArgumentException("Unexpected extremum type.");
            }
        }

        public enum ExtremumType
        {
            Maximum,
            Minimum,
            Both
        }
    }
}
/*
 * 
 * Simulation 1 Roadmap:
 * v0.4 : 
 *  Timer X
 *  Map X                         
 * v0.5 : 
 *  MapObjects 
 *  Food collectible
 *  Agent Player-Control
 * v0.6 :
 *  Agent Actions
 *  
 * v0.7 :
 *  Interoception
 *  Needs
 * v0.8 :
 *  Observation
 *  Actions
 * v0.9 :
 *  Bevavioral Hierarchy
 * 
 * 
 * v1.0 :
 *  
 * 
 */