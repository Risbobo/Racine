using Racines;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CameraBehavior : MonoBehaviour
{
    private Camera _thisCamera;

    private Vector3 _dragOrigin;
    [SerializeField] private float _minCamSize = 0.5f;
    [SerializeField] private float _maxCamSize = 10f;
    [SerializeField] private float _zoomScale = 1f;

    //[SerializeField] private float _mapViewOffset = 2f;
    [SerializeField] private GameManager _gameManager;

    //[SerializeField] private Map _map;

    void Start()
    {
        _thisCamera = GetComponent<Camera>();
    }

    /*private void Awake()
    {
        _mapMinX = -_map.Width / 2f;
        _mapMaxX = _map.Width / 2f - _mapViewOffset;
        _mapMaxY = _map.MaxY;
        _mapMinY = _map.MinY;
    }*/

    // Update is called once per frame
    void Update()
    {
        PanCamera();
        Zoom();
    }

    void PanCamera()
    {
        // 0 for left click, 1 for right click
        if (Input.GetMouseButtonDown(1))
        {
            _dragOrigin = _thisCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 difference = _dragOrigin - _thisCamera.ScreenToWorldPoint((Input.mousePosition));
            //_thisCamera.transform.position += difference;
            _thisCamera.transform.position = ClampCamera(_thisCamera.transform.position + difference);
        }
    }

    void Zoom()
    {
        var newSize = _thisCamera.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * _zoomScale;
        _thisCamera.orthographicSize = Mathf.Clamp(newSize, _minCamSize, _maxCamSize);
        _thisCamera.transform.position = ClampCamera(_thisCamera.transform.position);
    }

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = _thisCamera.orthographicSize;
        float camWidth = _thisCamera.orthographicSize * _thisCamera.aspect;

        //float camHeight = 0f;
        //float camWidth = 0f;

        float minX = _gameManager.mapMinX + camWidth;
        float maxX = _gameManager.mapMaxX - camWidth;
        float minY = _gameManager.mapMinY + camHeight;
        float maxY = _gameManager.mapMaxY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }
}
