using UnityEditor;
using UnityEngine;

public class MapGeneratorEditorWindow : EditorWindow
{
    [MenuItem("Tools/Map Generator")]
    public static void ShowWindow()
    {
        GetWindow<MapGeneratorEditorWindow>("Map Generator");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Generate Map"))
        {
            // Call your procedural map generation method here
            GenerateMap();
        }
    }

    void GenerateMap()
    {
        var result = new FloatMapGenerator();
        result.GenerateTerrainField();
        result.GenerateMap();
        Debug.Log("Map Generated!");
        // Call your actual map generation method here
    }
}