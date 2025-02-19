using System;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
namespace CogSim
{
    public class Behavior
    {
        private AgentObject agent;
        public SensoryMatrix freeEnergy;
        public Affordance curAffordance;
        public Policy curPolicy;
        public BehaviorType curBehavior;
        public StrategyType curStrategy;
        public Act IntendedAct;
        public Vector3Int AffordanceTarget => curAffordance.GetNextWaypoint;
        public Vector3Int PolicyTarget => curPolicy.GetNextWaypoint;
        /*
         * Agnet has no "path" 
         * the path the agent takes is stored acorss the behavior tiers
         * The agent will select the act based on the affordance waypoints.
         * When needed, the agent will update the affordance based on the policy waypoints
         * When needed, the policy resets based on the selected strategy
         * 
         */

        public Behavior(AgentObject agentObject)
        {
            this.agent = agentObject;

        }
        private BehaviorType BehaviorMapping()
        {
            // super simple... only two needs
            if (agent.Energy < agent.Satiety)
            {
                return BehaviorType.FoodSeeking;
            }
            else { return BehaviorType.RestSeeking; }
        }
        private StrategyType StrategicPlanning()
        {
            if (curBehavior == BehaviorType.RestSeeking)
            {


            }
            if (curBehavior == BehaviorType.FoodSeeking)
            {
                if (agent.Memory.HasFoodMemory())
                {
                    return StrategyType.Recollect;
                }
                else if (agent.Memory.HasFoodMemory())
                {
                    // 
                    return StrategyType.Recollect;
                }
                else if (agent.Memory.HasFoodIdea())
                {
                    return StrategyType.Search; // go to then search
                }
            }
            return StrategyType.Search;
        }
        private Policy EngagementPhantasia()
        {
            Policy result = new Policy();
            /*
             * imagined path to strategy - 
             * read strategy type, determine destination based on Strat
             * create waypath to destination 
             * 
             */
            return result; 
        }
        private Affordance AffordanceProjection(AgentObject agent, Vector3Int origin, Policy policy) 
        {
            Affordance affordance = new Affordance();

            return affordance;
        }
        private Act ActiveInference(AgentObject agent, Affordance affordance) 
        { 
            return Act.Stay; 
        }





    }
    public abstract class BehaviorTier
    {
        public Queue<Vector3Int> waypoints;
        public float urgency;
        public Vector3Int origin;
        public Vector3Int destination;
        protected BehaviorTier() { }
        public virtual Vector3Int GetNextWaypoint=>waypoints.Peek();
        public virtual Vector3Int PopNextWaypoint=>waypoints.Dequeue();


    }
    public class Policy : BehaviorTier
    {    
        public Policy() { }


    }
    public class Affordance : BehaviorTier
    {
        public Affordance() {   }


    }

    public class Memory
    {
        private AgentObject agent;
        public Memory(AgentObject agent) { this.agent = agent;}
        public bool HasFoodMemory() {  return false; }
        public bool HasFoodIdea() { return false; }
    }
    public class Engram
    {
        // States memory, saved every turn into a list 
    }
    public class Schema
    {

    }


    public class Homeostasis
    {
        private AgentObject agent;
        public Homeostasis(AgentObject agent) 
        { 
            this.agent = agent; 
        }
        public void Upkeep()
        {
            if (agent.Energy < agent.MAX) Digestion();
            if (agent.HeatCapacity < agent.MAX) Thermoregulation();
        }
        public void Digestion() 
        {
            agent.Satiety -= 2;
            agent.Energy += 1;

        } //Decrease Satiety to increase Energy
        public void Thermoregulation() 
        {
            agent.Hydration -= 4;
            agent.HeatCapacity += 3;
        } //Decrease Hydration to increase HeatCapacity
        public void ActionEntropy() 
        {
            agent.HeatCapacity -= 3;
        } //Decrease HeatCapacity when non-stay action is done
        public void Exhaustion() 
        {
            agent.Energy -= 2;
        } //Decrease Energy to do Action


    
    }

    public class Foresight
    {
        /*
         * This class will do the predictive processes 
         * 
         */
        private AgentObject agent;
        public Foresight(AgentObject agent) { this.agent = agent; }
        public SensoryMatrix GetExpectationMatrix()
        {
            return new SensoryMatrix();
        }
    }
    public class Senses
    {
        public List<Sight> sights; //sight vestors 
        public AgentObject agentObject;
        public Vector3Int position;
        public Vector3 bearings;
        public bool FoodSighted => AllVisibleFood.Any();
        public bool AgentSighted => AllVisibleAgents.Any();

        public HashSet<Vector3Int> AllVisibleWalls => (HashSet<Vector3Int>)AllVisibleTiles.Where(s => s.y >= 1f);
        public HashSet<Vector3Int> AllVisibleEdges
        {
            get
            {
                HashSet<Vector3Int> result = new HashSet<Vector3Int>();
                foreach(var sight in sights.FindAll(s=> !s.hasWall))
                {
                    result.Add(sight.FurthestTile);
                }
                return result;
            }
        }
        public HashSet<Vector3Int> AllVisibleTiles
        {
            get
            {
                var result = new HashSet<Vector3Int>();
                foreach (var sight in sights)
                {
                    result.UnionWith(sight.tiles);
                } 
                return result;
            }
        }
        public HashSet<ResourceObject> AllVisibleFood
        {
            get
            {
                var result = new HashSet<ResourceObject>();
                foreach (var sight in sights)
                {
                    result.UnionWith(sight.ResourceObjects.FindAll(s=>s.IsFood));
                }
                return result;
            }
        }
        public HashSet<AgentObject> AllVisibleAgents
        {
            get
            {
                var result = new HashSet<AgentObject>();
                foreach (var sight in sights)
                {
                    result.UnionWith(sight.agentObjects);
                }
                return result;
            }
        }
        public float[,] LocalGrid
        {
            get
            {
                HashSet<Vector3Int> tiles = this.AllVisibleTiles;

                // If no tiles are visible, return an empty grid
                if (tiles.Count == 0) return new float[0, 0];

                // Calculate the bounds of the visible area
                int minX = tiles.Min(t => t.x);
                int maxX = tiles.Max(t => t.x);
                int minZ = tiles.Min(t => t.z);
                int maxZ = tiles.Max(t => t.z);

                int width = maxX - minX + 1;
                int height = maxZ - minZ + 1;

                // Initialize the local grid
                float[,] grid = new float[width, height];

                // Copy data from the main map grid to the local grid
                for (int x = minX; x <= maxX; x++)
                {
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        // Map the world coordinates to local grid coordinates
                        int localX = x - minX;
                        int localZ = z - minZ;

                        // Directly copy from the main map's grid
                        grid[localX, localZ] = Finder.Map.Grid[x, z];
                    }
                }

                return grid;
            }
        }
       
        
        public Senses(AgentObject agentObject )
        {
            this.agentObject = agentObject;
            
             
        }
        public SensoryMatrix GetObservationMatrix(Vector3Int position)
        {
            sights = SenseSights(agentObject.visionRange).ToList();
            this.position = position;
            return new SensoryMatrix(this, position);

        }

        public IEnumerable<Sight> SenseSights(int range)
        {

            foreach (Vector3Int tile in MapUtility.GetTilesInRangeMax(agentObject.Position, range))
            {
                yield return SenseSight(agentObject.Position, tile);
            }

        }
        public Vector3 SenseBearing(Vector3Int location, Affordance affordance)
        {
            return new Vector3();
        }
        private Sight SenseSight(Vector3Int start, Vector3Int end)
        {
            Sight sight = new Sight();
            sight.tiles = new List<Vector3Int>();
            sight.terrains = new List<float>();
            sight.agentObjects = new List<AgentObject>();
            sight.ResourceObjects = new List<ResourceObject>();
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

                // passed!
                sight.terrains.Add(Finder.Map.TerrainAt(current));
                sight.tiles.Add(current);

                AgentObject tempAgent;
                if (Finder.Map.TryGetAgentAt(current, out tempAgent))
                {
                    sight.agentObjects.Add(tempAgent);
                }
                ResourceObject tempFood;
                if (Finder.Map.TryGetMapObject<ResourceObject>(current, out tempFood) && tempFood.IsFood)
                {
                    sight.ResourceObjects.Add(tempFood);
                }
                if (Finder.Manager.IsWalled(current))
                {
                    sight.hasWall = true;
                    break; // Line-of-sight is blocked
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

        public Engram Engraminate() { return null; }

    }



    public class Sight
    {
        public List<float> terrains;
        public List<Vector3Int> tiles;
        public List<ResourceObject> ResourceObjects;
        public List<AgentObject> agentObjects;
        public bool hasWall = false;

        public ResourceObject ClosestResource => ResourceObjects.FirstOrDefault();
        public AgentObject ClosestAgent => agentObjects.FirstOrDefault();
        private float seclusion => TerrainSum / (float)TileCount;
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
        }

    }







    public enum BehaviorType
    {
        RestSeeking = 0,
        FoodSeeking = 1,
        WaterSeeking = 2

    }
    public enum StrategyType
    {
        Get = 0,
        Search = 1,
        Recollect = 2,

    }
    public enum Act
    {
        Up = 0,    //+z
        Down = 1,  //-z
        Right = 2, //+x
        Left = 3,   //-x
        Stay = 4
    }
}


