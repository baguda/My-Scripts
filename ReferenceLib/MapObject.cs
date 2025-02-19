using UnityEngine;

namespace CogSim
{
    public class MapObject
    {
        private string id;
        public Vector3Int position;
        public int AgeTurns;
        public GameObject gameObject; 
        public string ID 
        {
            get
            {
                return id;
            }
            private set
            {
                id = value;
            }
            }
        public Vector3Int Position { get; set; }

        public string Name;
        public string Description;
        public MapObject(GameObject gameObject, string id, Vector3Int position)
        {
            this.id = id;
            this.position = position;
            this.gameObject = gameObject;
        }
        public virtual void Update() 
        {
            this.AgeTurns++;
        }
        public virtual void Upkeep() { }

    }


}

