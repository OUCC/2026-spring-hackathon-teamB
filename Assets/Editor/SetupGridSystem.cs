using UnityEditor;
using UnityEngine;

public class SetupGridSystem : EditorWindow
{
    [MenuItem("TeamB/Add Grid System to Active Scene")]
    public static void RunSetup()
    {
        // 1. Create folders
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            AssetDatabase.CreateFolder("Assets", "Materials");
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/PlaceableItems"))
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "PlaceableItems");

        // 2. Create Materials
        Material defaultMat = CreateMaterial("Assets/Materials/GridDefault.mat", Color.white);
        Material hoverMat = CreateMaterial("Assets/Materials/GridHover.mat", Color.cyan);
        Material occupiedMat = CreateMaterial("Assets/Materials/GridOccupied.mat", Color.red);
        Material turretMat = CreateMaterial("Assets/Materials/TurretMat.mat", Color.blue);

        // 3. Create GridCell Prefab
        string cellPath = "Assets/Prefabs/GridCell.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(cellPath) != null)
        {
            AssetDatabase.DeleteAsset(cellPath);
        }

        GameObject cellObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cellObj.name = "GridCellPrefab";
        cellObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        
        GridCell cellScript = cellObj.AddComponent<GridCell>();
        SerializedObject cellSO = new SerializedObject(cellScript);
        cellSO.FindProperty("_defaultColor").colorValue = Color.white;
        cellSO.FindProperty("_hoverColor").colorValue = Color.cyan;
        cellSO.FindProperty("_occupiedHoverColor").colorValue = Color.red;
        cellSO.ApplyModifiedProperties();

        cellObj.GetComponent<Renderer>().sharedMaterial = defaultMat;
        var col = cellObj.GetComponent<BoxCollider>();
        col.size = new Vector3(1f, 1f, 1f);

        GameObject cellPrefab = PrefabUtility.SaveAsPrefabAsset(cellObj, cellPath);
        DestroyImmediate(cellObj);

        // 4. Create Turret Prefab (Dummy object to place)
        string turretPath = "Assets/Prefabs/PlaceableTurret.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(turretPath) != null)
        {
            AssetDatabase.DeleteAsset(turretPath);
        }

        GameObject turretObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        turretObj.name = "TurretPrefab";
        turretObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        turretObj.GetComponent<Renderer>().sharedMaterial = turretMat;
        GameObject turretPrefab = PrefabUtility.SaveAsPrefabAsset(turretObj, turretPath);
        DestroyImmediate(turretObj);

        // 5. Create a default PlaceableItemSO
        string soPath = "Assets/ScriptableObjects/PlaceableItems/TurretItem.asset";
        PlaceableItemSO turretSO = AssetDatabase.LoadAssetAtPath<PlaceableItemSO>(soPath);
        if (turretSO == null)
        {
            turretSO = ScriptableObject.CreateInstance<PlaceableItemSO>();
            turretSO.ItemName = "Turret";
            turretSO.ItemID = "turret_01";
            turretSO.Prefab = turretPrefab;
            AssetDatabase.CreateAsset(turretSO, soPath);
        }
        else
        {
            turretSO.Prefab = turretPrefab;
            EditorUtility.SetDirty(turretSO);
        }

        // 6. Setup Camera in current scene
        Camera cam = Camera.main;
        GameObject camObj;
        if (cam == null)
        {
            camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            cam = camObj.AddComponent<Camera>();
        }
        else
        {
            camObj = cam.gameObject;
        }

        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
        
        CameraController camController = camObj.GetComponent<CameraController>();
        if (camController == null)
            camController = camObj.AddComponent<CameraController>();

        // 6. Setup Game Manager in current scene
        GameObject gmObj = GameObject.Find("GameManager");
        if (gmObj == null)
        {
            gmObj = new GameObject("GameManager");
        }
        
        GridManager gridManager = gmObj.GetComponent<GridManager>();
        if (gridManager == null)
            gridManager = gmObj.AddComponent<GridManager>();
            
        SerializedObject gmSO = new SerializedObject(gridManager);
        gmSO.FindProperty("_width").intValue = 10;
        gmSO.FindProperty("_height").intValue = 10;
        gmSO.FindProperty("_cellSize").floatValue = 1f;
        gmSO.FindProperty("_outlineThickness").floatValue = 0.5f;
        gmSO.FindProperty("_cellPrefab").objectReferenceValue = cellPrefab.GetComponent<GridCell>();
        gmSO.ApplyModifiedProperties();

        PlacementSystem placementSystem = gmObj.GetComponent<PlacementSystem>();
        if (placementSystem == null)
            placementSystem = gmObj.AddComponent<PlacementSystem>();
            
        SerializedObject psSO = new SerializedObject(placementSystem);
        psSO.FindProperty("_currentItem").objectReferenceValue = turretSO;
        psSO.FindProperty("_mainCamera").objectReferenceValue = cam;
        psSO.FindProperty("_gridManager").objectReferenceValue = gridManager;
        psSO.ApplyModifiedProperties();

        // Assign CameraController references
        SerializedObject camSO = new SerializedObject(camController);
        camSO.FindProperty("_gridManager").objectReferenceValue = gridManager;
        camSO.FindProperty("_camera").objectReferenceValue = cam;
        camSO.ApplyModifiedProperties();

        AssetDatabase.SaveAssets();

        Debug.Log("Grid System added to Active Scene successfully!");
    }

    private static Material CreateMaterial(string path, Color color)
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        Shader outlineShader = Shader.Find("Custom/GridOutlineShader");
        
        if (outlineShader == null) outlineShader = Shader.Find("Universal Render Pipeline/Lit");
        if (outlineShader == null) outlineShader = Shader.Find("Standard");

        if (mat == null)
        {
            mat = new Material(outlineShader);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            else mat.color = color;
            AssetDatabase.CreateAsset(mat, path);
        }
        else
        {
            mat.shader = outlineShader;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            else mat.color = color;
            EditorUtility.SetDirty(mat);
        }
        return mat;
    }
}
