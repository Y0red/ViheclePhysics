using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoadGenerator))]
public class InspectorTool : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RoadGenerator con = (RoadGenerator)target;

        if (GUILayout.Button("Spawn"))
        {
            con.GeneratePlatforms();
        }
        if (GUILayout.Button("Reset"))
        {
            con.ResetPlatforms();
        }
    }
}
