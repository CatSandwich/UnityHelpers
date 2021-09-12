using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SerializedReflection;
using UnityEditor;
using UnityEngine;

// Requires EditorUtils with grid splitting
// Allows a type and field name to be serialized

[Serializable]
public class SerializedField
{
    public FieldInfo FieldInfo => Type.Type.GetField(Field);

    public SerializedType Type;
    public string Field;
}

[CustomPropertyDrawer(typeof(SerializedField))]
public class SerializedFieldEditor : PropertyDrawer
{
    private bool _show;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
        EditorUtils.TITLE_HEIGHT * (_show ? 3 : 1);

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        var grid = rect.Split(1, _show ? 3 : 1);

        #region Values

        var type = property.FindPropertyRelative("Type");
        var field = property.FindPropertyRelative("Field");
        var script = (MonoScript) type.FindPropertyRelative("Script").GetValue();

        // Try to get the class from the MonoScript
        Type @class;
        try
        {
            @class = script.GetClass();
        }
        catch (NullReferenceException)
        {
            @class = null;
        }

        // Get all fields in the class
        var options = Type.GetType(@class?.AssemblyQualifiedName ?? "")
            ?.GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var optionNames = options?.Select(f => f.Name).ToArray() ?? new string[] { };
        var optionNameLongs = options?.Select(f => $"{f.Name}: {f.FieldType.Name}").ToArray() ?? new string[] { };

        #endregion

        #region Draw

        // Delay folding out by one tick to allow grid to update
        var before = _show;
        _show = EditorGUI.Foldout(grid[0, 0], _show, label);

        // If foldout closed, return
        if (!_show || !before) return;

        // Type/MonoScript Field
        EditorGUI.PropertyField(grid[0, 1], type);

        // Field select menu
        var selected = Array.IndexOf(optionNames, field.stringValue);
        selected = EditorGUI.Popup(grid[0, 2], "Field", selected, optionNameLongs);
        field.stringValue = selected == -1 ? null : optionNames[selected];

        #endregion
    }
}

// Courtesy of vedram on Unity Forum: https://forum.unity.com/threads/get-a-general-object-value-from-serializedproperty.327098/#post-4098484
public static class MyExtensions
{
    #if UNITY_EDITOR
    // Gets value from SerializedProperty - even if value is nested
    public static object GetValue(this SerializedProperty property)
    {
        object obj = property.serializedObject.targetObject;

        foreach (var path in property.propertyPath.Split('.'))
        {
            var type = obj.GetType();
            var field = type.GetField(path);
            obj = field.GetValue(obj);
        }

        return obj;
    }

    // Sets value from SerializedProperty - even if value is nested
    public static void SetValue(this SerializedProperty property, object val)
    {
        object obj = property.serializedObject.targetObject;
        var list = new List<(FieldInfo field, object val)>();

        foreach (var path in property.propertyPath.Split('.'))
        {
            var type = obj.GetType();
            var field = type.GetField(path);
            list.Add((field, obj));
            obj = field.GetValue(obj);
        }

        // Now set values of all objects, from child to parent
        for (var i = list.Count - 1; i >= 0; --i)
        {
            list[i].field.SetValue(list[i].val, val);
            // New 'val' object will be parent of current 'val' object
            val = list[i].val;
        }
    }
    #endif
}
