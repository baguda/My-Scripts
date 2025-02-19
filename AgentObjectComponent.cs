using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.FilePathAttribute;
namespace CogSim
{
    /// <summary>
    /// physical handler, controls agent action results.  
    /// </summary>
    public class AgentObjectComponent : MonoBehaviour
    {
        public AgentObject AgentObject;
        public GameObject AgentSim;
        public SimulationManager Manager;
        public Vector3 targetLocation;
        public float moveSpeed = 3f;


        public void Initialize(AgentObject agentObject, GameObject gameObject, SimulationManager manager)
        {
            Manager = manager;
            this.AgentSim = gameObject;
            AgentObject = agentObject;

        }

        void Update()
        {
            if (Finder.Timer.CurrentPhase == GameTimer.GamePhase.Update)
            {

                //Debug.Log("AgentObjectComponent.Update: target = " + targetLocation.ToString());
                // Optional: Check if the object has reached the target position
                if (transform.position != targetLocation)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetLocation, moveSpeed * Time.deltaTime);

                }
            }


        }
        public void MoveTo(int x, int z)
        {
            var result = new Vector3(x, Manager.Map.TerrainAt(x, z), z);
            transform.position = result;
            AgentSim.transform.position = result;
            AgentObject.Position = new Vector3Int(x, 1, z);
        }
        public void MoveTo(Vector3Int location)
        {
            Debug.Log("AgentObjectComponent.MoveTo: location = " + location.ToString());
            var result = new Vector3(location.x, Manager.Map.TerrainAt(location.x, location.z), location.z);
            Debug.Log("AgentObjectComponent.MoveTo: result = " + result.ToString());
            targetLocation = result;

            AgentObject.Position = location;
        }
        public void MoveAction(Act direction)
        {
            Vector3Int result;
            if (direction == Act.Right)
            {
                result = AgentObject.Position + new Vector3Int(1, 2, 0);
                if ( Manager.IsPathable(result))
                {
                    MoveTo(result);
                }
            }
            else if (direction == Act.Left)
            {
                result = AgentObject.Position + new Vector3Int(-1, 2, 0);
                if (Manager.IsPathable(result))
                {
                    MoveTo(result);
                }

            }
            else if (direction == Act.Up)
            {
                result = AgentObject.Position + new Vector3Int(0, 2, 1);
                if (Manager.IsPathable(result))
                {
                    MoveTo(result);
                }

            }
            else if (direction == Act.Down)
            {
                result = AgentObject.Position + new Vector3Int(0, 2, -1);
                if (Manager.IsPathable(result))
                {
                    MoveTo(result);
                }
            }
            else
            {
                targetLocation = AgentObject.Position;
            }
        }
        public Vector3Int[] AdjacentTiles
        {
            get
            {
                return new Vector3Int[]
                {
                    AgentObject.Position + new Vector3Int(1,0,0),
                    AgentObject.Position + new Vector3Int(0,0,1),
                    AgentObject.Position + new Vector3Int(-1,0,0),
                    AgentObject.Position + new Vector3Int(0,0,-1)
                };
            }
        }

        public IEnumerable<Vector3Int> PathableTiles()
        {
            foreach (var ind in AdjacentTiles)
            {
                if (Manager.IsPathable(ind)) yield return ind;
            }
        }


        // Helper method to check if a position is within the range (Manhattan distance)
        private bool IsWithinRange(Vector3Int position, int range)
        {
            int distance = Mathf.Abs(position.x - AgentObject.Position.x) +
                           Mathf.Abs(position.y - AgentObject.Position.y) +
                           Mathf.Abs(position.z - AgentObject.Position.z);

            return distance <= range;
        }


    }

}

