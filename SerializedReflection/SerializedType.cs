using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class SerializedType
{
    #if UNITY_EDITOR
    public MonoScript Script;
    #endif

    public Type Type => _type ??= Type.GetType(AssemblyQualifiedName);
    private Type _type;

    [HideInInspector] public string AssemblyQualifiedName;

    public void OnValidate()
    {
        AssemblyQualifiedName = Script.GetClass().AssemblyQualifiedName;
    }
}

[CustomPropertyDrawer(typeof(SerializedType))]
public class SerializedTypeEditor : PropertyDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.ObjectField(rect, property.FindPropertyRelative("Script"), label);
    }
}
