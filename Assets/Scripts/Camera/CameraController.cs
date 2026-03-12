using UnityEngine;

[ExecuteAlways]
public class CameraController : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private Camera _camera;
    
    [Header("Isometric Settings")]
    [SerializeField] private float _zoomMultiplier = 0.7f;
    [SerializeField] private float _orthographicPadding = 2f;
    [SerializeField] private Vector3 _isometricRotation = new Vector3(30f, 45f, 0f);
    [SerializeField] private float _cameraDistance = 20f;

    private void Start()
    {
        Initialize();
        SetupIsometricCamera();
    }

    private void OnValidate()
    {
        Initialize();
        SetupIsometricCamera();
    }

    private void LateUpdate()
    {
        if (!Application.isPlaying)
        {
            SetupIsometricCamera();
        }
    }

    private void Initialize()
    {
        if (_camera == null)
        {
            _camera = GetComponent<Camera>();
            if (_camera == null) _camera = Camera.main;
        }

        if (_gridManager == null)
            _gridManager = FindFirstObjectByType<GridManager>();
    }

    private void SetupIsometricCamera()
    {
        if (_camera == null || _gridManager == null) return;

        // Set camera to orthographic based on requirements
        _camera.orthographic = true;
        
        // Calculate the center of the grid
        Vector3 gridCenter = _gridManager.GetGridCenter();

        // Rotate the camera to an isometric view (quarter view)
        _camera.transform.rotation = Quaternion.Euler(_isometricRotation);

        // Position the camera back along its negative forward vector
        _camera.transform.position = gridCenter - _camera.transform.forward * _cameraDistance;

        // Calculate the required orthographic size to fit the grid
        // Max diagonal size across x and z planes
        float maxGridDimension = Mathf.Max(_gridManager.Width, _gridManager.Height) * _gridManager.CellSize;
        
        // Since we are looking diagonally, we need a bit more space.
        // A simple heuristic for orthographic size to fit a square grid rotated 45 degrees
        float requiredSize = ((maxGridDimension * 0.5f * Mathf.Sqrt(2f)) + _orthographicPadding) * _zoomMultiplier;
        
        _camera.orthographicSize = requiredSize;
    }
}
