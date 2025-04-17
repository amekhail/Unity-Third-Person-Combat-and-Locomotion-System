using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DismembermentController))]
public class DismembermentControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DismembermentController controller = (DismembermentController)target;

        if (GUILayout.Button("Test Dismember Head"))
        {
            controller.DismemberHead();
        }
    }
}