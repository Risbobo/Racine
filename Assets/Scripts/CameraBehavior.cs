using System;
using System.Collections;
using System.Collections.Generic;
using Racines;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    private Camera _thisCamera;

    private Vector3 _dragOrigin;
    [SerializeField] private float _minCamSize = 0.5f;
    [SerializeField] private float _maxCamSize = 10f;
    [SerializeField] private float _zoomScale = 1f;
    
    void Start()
    {
        _thisCamera = GetComponent<Camera>();
    }

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
            _thisCamera.transform.position += difference;
        }
    }

    void Zoom()
    {
        var newSize = _thisCamera.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * _zoomScale;
        _thisCamera.orthographicSize = Mathf.Clamp(newSize, _minCamSize, _maxCamSize);
    }
}
