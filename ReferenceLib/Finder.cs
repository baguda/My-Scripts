using Unity.VisualScripting;
using UnityEngine;
namespace CogSim
{
    
    public static class Finder
    {
        public static GameTimer Timer => GameObject.Find("GameTimer").GetComponent<GameTimer>();
        public static SimulationManager Manager => GameObject.Find("SimulationManager").GetComponent<SimulationManager>();
        public static SimulationMap Map => Manager.Map;
        public static SimulationGUI SimGUI => GameObject.Find("SimulationGUI").GetComponent<SimulationGUI>();

    }
}

