using UnityEngine;
namespace CogSim
{
    public class SimulationGUI : MonoBehaviour
    {
        private GameTimer _gameTimer;

        // Title and description for the GUI
        private string _title = "Simulation Controls";
        private string _description = "Use the checkbox to switch between Automatic and Manual modes.";

        // Checkbox state
        private bool _isAutomaticMode = true;

        private void Start()
        {
            _gameTimer = Finder.Timer;

            // Initialize the mode based on the checkbox state
            _gameTimer.Mode = _isAutomaticMode ? GameTimer.SimulationMode.Automatic : GameTimer.SimulationMode.Manual;
        }

        private void OnGUI()
        {
            // Define the GUI layout
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));

            // Display the title
            GUILayout.Label(_title, GUI.skin.label);

            // Display the description
            GUILayout.Label(_description, GUI.skin.label);

            // Add some space
            GUILayout.Space(20);

            // Checkbox for switching between Automatic and Manual modes
            _isAutomaticMode = GUILayout.Toggle(_isAutomaticMode, "Automatic Mode");

            // Update the GameTimer mode based on the checkbox state
            _gameTimer.Mode = _isAutomaticMode ? GameTimer.SimulationMode.Automatic : GameTimer.SimulationMode.Manual;

            // Display the "Proceed to Update Phase" button only in Manual Mode during the Upkeep Phase
            if (_gameTimer.Mode == GameTimer.SimulationMode.Manual && _gameTimer.CurrentPhase == GameTimer.GamePhase.Upkeep)
            {
                if (GUILayout.Button("Proceed to Update Phase"))
                {
                    _gameTimer.ManualSwitchPhase();
                }
            }

            GUILayout.EndArea();
        }
    }
}