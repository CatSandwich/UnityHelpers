[Serializable]
public class SerializedScene
{
    public Scene Scene => UnityEngine.SceneManagement.SceneManager.GetSceneByName(_scene);
    [SerializeField] private string _scene;
}

[CustomPropertyDrawer(typeof(SerializedScene))]
public class SerializedSceneDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var sceneString = property.FindPropertyRelative("_scene");

        var unselected = "Select...";
        
        var arr = Enumerable.Range(0, UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
            .Select(SceneUtility.GetScenePathByBuildIndex)
            .Select(System.IO.Path.GetFileNameWithoutExtension)
            .Prepend(unselected)
            .ToArray();
        var index = Array.IndexOf(arr, sceneString.stringValue);
        index = EditorGUI.Popup(position, label.text, index, arr);
        sceneString.stringValue = index < 0 ? unselected : arr[index];
    }
}
