using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CogSim
{
    public class MapObjectList
    {
        private List<MapObject> objectList;

        public List<string> ObjectIDs => objectList.Select(s => s.ID).ToList();
        public List<Vector3Int> AllPositions => objectList.Select(s => s.Position).ToList();
        public List<AgentObject> AllAgents => (objectList.Select(s => s is AgentObject) as List<AgentObject>);
        public List<ResourceObject> AllFood => (objectList.Select(s => s is ResourceObject) as List<ResourceObject>);
        public List<MapObject> AllObjects => objectList;
        public MapObjectList()
        {
            this.objectList = new List<MapObject>();
        }
        public void AddMapObject(MapObject newObject)
        {
            if (!ValidateObject(newObject.ID))
            {
                this.objectList.Add(newObject);
            }


        }
        public void AddMapObject(List<MapObject> newObjectList)
        {
            foreach (MapObject obj in newObjectList) AddMapObject(obj);
        }
        public MapObject GetMapObject(string ID)
        {
            if (ValidateObject(ID)) return this.objectList.Find(s => s.ID == ID);

            return null;

        }
        public MapObject GetMapObject(Vector3Int position)
        {
            if (ValidateObject(position)) return this.objectList.Find(s => s.Position == position);

            return null;

        }
        public void RemoveMapObject(string ID)
        {
            if (ValidateObject(ID)) this.objectList.Remove(objectList.Find(s => s.ID == ID));


        }
        public bool ValidateObject(string ID)
        {
            if (objectList.FindAll(s => s.ID == ID).Any()) return true;
            else return false;

        }
        public bool ValidateObject(Vector3Int position)
        {
            if (objectList.FindAll(s => s.Position == position).Any()) return true;
            else return false;

        }

    }
}