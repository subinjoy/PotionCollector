using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PotionData))]
public class PotionDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PotionData data = (PotionData)target;

        EditorGUILayout.LabelField("Potion Properties Editor", EditorStyles.boldLabel);

        //Potion Properties
        data.potionName = EditorGUILayout.TextField("Name", data.potionName);
        data.potency = EditorGUILayout.IntSlider("Potency (Score)", data.potency, 1, 100);
        data.icon = (Sprite)EditorGUILayout.ObjectField("Icon", data.icon, typeof(Sprite), false);
        data.addressableLabel = EditorGUILayout.TextField("Addressable Label", data.addressableLabel);
        EditorGUILayout.LabelField("Description");
        data.description = EditorGUILayout.TextArea(data.description, GUILayout.Height(50));

        EditorGUILayout.Separator();

        if (GUI.changed)  // Changes
        {
            EditorUtility.SetDirty(data);
        }
    }
}