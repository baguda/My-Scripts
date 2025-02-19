using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
namespace CogSim
{

    /*
    public class Observation
    {
        public List<Sight> sights; //sight vestors 
        private AgentObject agentObject;
        public Vector3 bearings;
        public bool FoodSighted;

        public Observation(AgentObject agentObject) 
        { 
            this.agentObject = agentObject;
            sights = GetSights(agentObject.visionRange).ToList();
            
        }

        public IEnumerable<Sight> GetSights(int range)
        {

            foreach (Vector3Int tile in MapUtility.GetTilesInRangeMax(agentObject.Position, range))
            {
                yield return SenseSight(agentObject.Position, tile);
            }

        }
        public Vector3 GetBearing(Vector3Int location, Plan )
        private Sight SenseSight(Vector3Int start, Vector3Int end)
        {
            Sight sight = new Sight();
            sight.tiles = new List<Vector3Int>();
            sight.terrains = new List<float>();
            sight.agentObjects = new List<AgentObject>();
            sight.foodObjects = new List<FoodObject>();
            int x0 = start.x;
            int z0 = start.z;
            int x1 = end.x;
            int z1 = end.z;

            int dx = Mathf.Abs(x1 - x0);
            int dz = Mathf.Abs(z1 - z0);
            int sx = (x0 < x1) ? 1 : -1;
            int sz = (z0 < z1) ? 1 : -1;
            int err = dx - dz;

            while (true)
            {
                // Check if the current cell is obstructed
                Vector3Int current = new Vector3Int(x0, start.y, z0);
                if (Finder.Manager.IsWalled(current))
                {
                    break; // Line-of-sight is blocked
                }
                // passed!
                sight.terrains.Add(Finder.Map.TerrainAt(current));
                sight.tiles.Add(current);

                AgentObject tempAgent;
                if (Finder.Map.TryGetAgentAt(current,out tempAgent))
                {
                    sight.agentObjects.Add(tempAgent);
                }
                FoodObject tempFood;
                if (Finder.Map.TryGetMapObject<FoodObject>(current, out tempFood))
                {
                    sight.foodObjects.Add(tempFood);
                }
                // Stop if we've reached the end point
                if (x0 == x1 && z0 == z1)
                {
                    break;
                }

                int e2 = 2 * err;
                if (e2 > -dz)
                {
                    err -= dz;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    z0 += sz;
                }
            }
            return sight;
            // Line-of-sight is clear
        }
    
        
        
    }



    public class Sight
    {
        public List<float> terrains;
        public List<Vector3Int> tiles;
        public List<FoodObject> foodObjects;
        public List<AgentObject> agentObjects;

        public FoodObject ClosestFood => foodObjects.FirstOrDefault();
        public AgentObject ClosestAgent => agentObjects.FirstOrDefault();
        private float seclusion => TerrainSum/(float)TileCount ;
        public float TerrainSum => terrains.Sum();
        public float TerrainMax => terrains.Max();
        public float TerrainMin => terrains.Min();
        public int TileCount => tiles.Count;
        public Vector3Int FurthestTile => tiles.Last();
        public Vector3Int ClosestTile => tiles.First();
        public Vector3Int HighestTile => tiles.ElementAt(terrains.IndexOf(terrains.Find(s => s == TerrainMax)));
        public Vector3Int LowestTile => tiles.ElementAt(terrains.IndexOf(terrains.Find(s => s == TerrainMin)));
        public int StepsToHighest => terrains.IndexOf(terrains.Find(s => s == TerrainMax));
        public Sight() 
        { 
        }*/

    }