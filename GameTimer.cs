using CogSim;
using UnityEngine;

namespace CogSim
{
    public class GameTimer : MonoBehaviour
    {
        public enum GamePhase
        {
            Upkeep,
            Update
        }

        public enum SimulationMode
        {
            Automatic,
            Manual
        }

        public GamePhase CurrentPhase { get; private set; } = GamePhase.Upkeep;
        public SimulationMode Mode { get; set; } = SimulationMode.Automatic;

        public float upkeepDuration = 2f;
        public float updateDuration = 2f;

        private float _phaseTimer;
        private SimulationManager _simulationManager;
        public SimulationManager simulationManager
        {
            get
            {
                if (_simulationManager == null)
                {
                    _simulationManager = GameObject.Find("SimulationManager").GetComponent<SimulationManager>();
                }
                return _simulationManager;
            }
        }

        private void Start()
        {
             //FindObjectOfType<SimulationMap>();

            //StartUpkeepPhase();
        }

        private void Update()
        {
            if (Mode == SimulationMode.Automatic)
            {
                _phaseTimer -= Time.deltaTime;

                if (_phaseTimer <= 0)
                {
                    SwitchPhase();
                }
            }
        }

        // Call this method to manually switch phases (used in Manual Mode)
        public void ManualSwitchPhase()
        {
            if (Mode == SimulationMode.Manual && CurrentPhase == GamePhase.Upkeep)
            {
                SwitchPhase();
            }
        }

        private void SwitchPhase()
        {
            if (CurrentPhase == GamePhase.Upkeep)
            {
                EndUpkeepPhase();
                StartUpdatePhase();
            }
            else
            {
                EndUpdatePhase();
                StartUpkeepPhase();
            }
        }

        private void StartUpkeepPhase()
        {
            CurrentPhase = GamePhase.Upkeep;
            _phaseTimer = upkeepDuration;

            Debug.Log("Upkeep Phase Started");
            simulationManager.Map.RunUpkeepPhase();
        }

        private void EndUpkeepPhase()
        {
            Debug.Log("Upkeep Phase Ended");
        }

        private void StartUpdatePhase()
        {
            CurrentPhase = GamePhase.Update;
            _phaseTimer = updateDuration;

            Debug.Log("Update Phase Started");
            simulationManager.Map.RunUpdatePhase();
        }

        private void EndUpdatePhase()
        {
            Debug.Log("Update Phase Ended");
        }
    }

    
}