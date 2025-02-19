using UnityEngine;

namespace CogSim
{
    public class ResourceObject : MapObject
    {
        public int NutritionValue { get; private set; }
        public new string Name = "Food";
        public new string Description = "Food is consumed for Energy";
        public ResourceType Type {  get; private set; }
        public bool IsFood => Type == ResourceType.food;
        public bool IsWater => Type == ResourceType.water;
        public ResourceObject(GameObject gameObject, string id, Vector3Int position, int nutritionValue, bool isWater = false) : base(gameObject,id, position)
        {
            NutritionValue = nutritionValue;
            if(isWater) this.Type = ResourceType.water;
            else this.Type = ResourceType.food;
        }


        public override void Upkeep() 
        { 

        }
        public override void Update() 
        {
            base.Update();
        }
        public enum ResourceType
        {
            food =0,
            water=1
        }
    }
}
