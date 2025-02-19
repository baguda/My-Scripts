using UnityEngine;
namespace CogSim
{
    public class ResourceObjectComponent : MonoBehaviour
    {
        public ResourceObject ResourceObject { get; private set; }
        public GameObject SimulationObject { get; private set; }
        public void Initialize(ResourceObject resourceObject, GameObject gameObject)
        {
            ResourceObject = resourceObject;
            this.SimulationObject = gameObject;
            
        }
        public void Update()
        {
            SimulationObject.transform.transform.Rotate(0.1f, 0f, 0.0f);
            //SimulationObject.transform.transform.Translate(new Vector3(0.01f,0f,0f));
        }
        private void OnTriggerEnter(Collider other)
        {
            // Destroy the collectible
            AgentObjectComponent component;
            if(other.gameObject.TryGetComponent<AgentObjectComponent>(out component))
            {
                //component.AgentObject.
                Destroy(gameObject);
            }
            
        }

    }
}
