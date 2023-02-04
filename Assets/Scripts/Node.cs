using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Racines
{
    public class Node : MonoBehaviour
    {
        [SerializeField] private int _depth;
        [SerializeField] private int _maxDepth = 5;
        [SerializeField] private GameObject _shapePrefab;
        
        private RootParams _rootParams;
        private Node _parent;
        private bool _isLeaf;
        
        [SerializeField] private float _width;

        protected void Start()
        {
            _width = RootManager.Instance.initialWidth;
            _rootParams = GetComponent<RootParams>();
            Grow();
            _rootParams.Calyptra.Clicked += OnCalyptraClicked;
        }

        private void OnCalyptraClicked()
        {
            _maxDepth = _depth + RootManager.Instance.depthIncrement;
            Grow();
        }

        private Vector3 GetCalyptraPosition()
        {
            return _rootParams.Calyptra.transform.position;
        }

        private void Grow()
        {
            if (_depth < _maxDepth)
            {
                StartCoroutine(CreateChildren());
                _rootParams.Calyptra.gameObject.SetActive(false);
            }
            else
            {
                _isLeaf = true;
                _rootParams.Calyptra.gameObject.SetActive(true);
            }
        }

        private IEnumerator CreateChildren()
        {
            yield return new WaitForSeconds(0.5f);
            Instantiate(_shapePrefab).AddComponent<Node>().Initialize(this, GetFanAngle());
            Instantiate(_shapePrefab).AddComponent<Node>().Initialize(this, -GetFanAngle());
        }

        private static float GetFanAngle()
        {
            return Random.Range(RootManager.Instance.minFanAngle, RootManager.Instance.maxFanAngle);
        }
        
        private void Initialize(Node parent, float angle)
        {
            _parent = parent;

            transform.position = parent.GetCalyptraPosition();
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward) * parent.transform.rotation;
            
            _shapePrefab = parent._shapePrefab;
            _maxDepth = parent._maxDepth;
            _depth = parent._depth + 1;
            
            parent.Widen();
        }
        
        private void Widen()
        {
            var newWidth = Mathf.Sqrt(_width * _width + RootManager.Instance.initialWidth * RootManager.Instance.initialWidth);
            var scale = transform.localScale;
            transform.localScale = new Vector3((newWidth / _width) * scale.x, scale.y, scale.z);
            _width = newWidth;

            if (_parent != null)
            {
                _parent.Widen();
            }
        }
    }
}