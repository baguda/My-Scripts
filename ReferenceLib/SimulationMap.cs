using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Linq;

namespace CogSim
{
    public class SimulationMap
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public float[,] Grid { get; set; }

        public MapObjectList MapObjects;
        public List<GameObject> environmentObjects;


        public SimulationMap(int width, int height)
        {
            Width = width;
            Height = height;
            Grid = new float[width, height];
            MapObjects = new MapObjectList();
        }


        public T GetMapObject<T>(string ID) where T : MapObject
        {
            return MapObjects.GetMapObject(ID) as T;
        }
        public T GetMapObject<T>(Vector3Int position) where T : MapObject
        {
            return MapObjects.GetMapObject(position) as T;
        }
        public void RegisterMapObject(MapObject mapObject)
        {
            MapObjects.AddMapObject(mapObject);
        }

        public void RemoveMapObject(string ID)
        {
            this.MapObjects.RemoveMapObject(ID);
        }

        public void RunUpkeepPhase()
        {
            foreach (var obj in this.MapObjects.AllObjects)
            {
                obj.Upkeep();
            }
        }
        public void RunUpdatePhase()
        {
            foreach (var obj in this.MapObjects.AllObjects)
            {
                obj.Update();
            }
        }
        public float TerrainAt(Vector3Int position)
        {
            return Grid[position.x, position.z];
        }
        public float TerrainAt(Vector3 position)
        {
            return Grid[(int)position.x, (int)position.z];
        }
        public float TerrainAt(int x, int z)
        {
            return Grid[x, z];
        }
        public float TerrainAt(Vector2Int position)
        {
            return Grid[position.x, position.y];
        }
        public bool HasObjectAt(Vector3Int position)
        {
            return this.MapObjects.AllPositions.Contains(position);
        }
        public bool HasAgentAt(Vector3Int position)
        {
            return this.MapObjects.AllAgents.Any(agent => agent.position == position);
        }
        public bool HasFoodAt(Vector3Int position)
        {
            return this.MapObjects.AllFood.Any(food => food.position == position);
        }
        public bool TryGetMapObject<T>(Vector3Int location, out T mapObject) where T : MapObject
        {
            if (HasObjectAt(location))
            {
                mapObject = GetMapObject<T>(location);
                return mapObject != null; // This ensures we actually found something
            }
            mapObject = default(T); // This sets mapObject to null for reference types, or default value for value types
            return false;
        }
        public bool TryGetAgentAt(Vector3Int location, out AgentObject agentObject) 
        {
            if (HasAgentAt(location))
            {
                agentObject = GetMapObject<AgentObject>(location);
                return agentObject != null; // This ensures we actually found something
            }
            agentObject = default; // This sets mapObject to null for reference types, or default value for value types
            return false;
        }
    

    
    }

}
