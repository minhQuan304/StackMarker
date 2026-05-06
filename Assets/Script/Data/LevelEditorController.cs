using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(LevelEditor))]
public class LevelEditorController : Editor
{
    private LevelEditor levelEditor;
    private LevelData levelToLoad; // Biến để giữ file Level muốn load

    private void OnEnable()
    {
        levelEditor = (LevelEditor)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("--- LEVEL MANAGEMENT ---", EditorStyles.boldLabel);

        levelToLoad = (LevelData)EditorGUILayout.ObjectField("Level To Load", levelToLoad, typeof(LevelData), false);

        if (GUILayout.Button("LOAD LEVEL (ĐỂ SỬA)", GUILayout.Height(30)))
        {
            if (levelToLoad != null)
            {
                if (EditorUtility.DisplayDialog("Xác nhận Load", "Bạn có muốn xóa map hiện tại để load level này không?", "Có", "Không"))
                {
                    levelEditor.ClearObjects(); // Xóa map hiện tại trước khi load
                    levelEditor.LoadLevel(levelToLoad);
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Thông báo", "Vui lòng kéo file LevelData vào ô Level To Load!", "OK");
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("--- PREFAB SELECTION ---", EditorStyles.boldLabel);

        if (levelEditor.registry != null)
        {
            string[] options = levelEditor.registry.GetAllPrefabNames().ToArray();
            int selectedIndex = System.Array.IndexOf(options, levelEditor.currentPrefabId);
            if (selectedIndex == -1) selectedIndex = 0;

            int newIndex = EditorGUILayout.Popup("Select Prefab", selectedIndex, options);

            if (newIndex != selectedIndex)
            {
                levelEditor.currentPrefabId = options[newIndex];
                EditorUtility.SetDirty(levelEditor);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Chưa gán PrefabRegistry!", MessageType.Error);
        }

        EditorGUILayout.Space();


        Color defaultColor = GUI.backgroundColor;

        GUI.backgroundColor = levelEditor.isEditing ? Color.red : Color.green;
        string labelEdit = levelEditor.isEditing ? "ĐANG EDIT (Bấm để dừng)" : "BẮT ĐẦU EDIT";

        if (GUILayout.Button(labelEdit, GUILayout.Height(40)))
        {
            levelEditor.Editing();
            EditorUtility.SetDirty(levelEditor);
        }

        GUI.backgroundColor = defaultColor;

        if (GUILayout.Button("LƯU LEVEL", GUILayout.Height(40)))
        {
            // Lấy đường dẫn giống hệt bên SaveLevel để kiểm tra
            string path = $"Assets/Resources/Data/{levelEditor.newLevelName}.asset";

            // Kiểm tra file tồn tại
            var existingAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<LevelData>(path);

            if (existingAsset != null)
            {
                // Nếu thấy file đã có, hiện bảng hỏi
                if (UnityEditor.EditorUtility.DisplayDialog(
                    "Xác nhận ghi đè",
                    $"Level '{levelEditor.newLevelName}' đã tồn tại. Bạn có muốn lưu đè lên dữ liệu cũ không?",
                    "Ghi đè",
                    "Hủy bỏ"))
                {
                    levelEditor.SaveLevel();
                }
            }
            else
            {
                // Nếu file chưa có (mới hoàn toàn), lưu luôn không cần hỏi
                levelEditor.SaveLevel();
            }
        }
        if (GUILayout.Button("DỌN SẠCH SCENE (CLEAR)", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Cảnh báo", "Bạn có chắc chắn muốn xóa toàn bộ object trên Scene?", "Xóa sạch", "Hủy"))
            {
                levelEditor.ClearObjects();
            }
        }
    }

    private void OnSceneGUI()
    {
        if (!levelEditor.isEditing) return;

        //Tools.current = Tool.None;

        //int controlID = GUIUtility.GetControlID(FocusType.Passive);

        //if (Event.current.type == EventType.Layout)
        //{
        //    HandleUtility.AddDefaultControl(controlID);
        //}
        Event e = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        if (e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag))
        {
            if (levelEditor.groundPlane.Raycast(ray, out float enter))
            {
                levelEditor.PlaceObject(ray.GetPoint(enter));
                e.Use();
            }
        }
        else if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && (e.button == 1 || e.button == 2))
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (e.button == 1) levelEditor.RemoveObject(hit.collider.gameObject);
                if (e.button == 2) levelEditor.RotateObject(hit.collider.gameObject);
                e.Use();
            }
        }
    }
}