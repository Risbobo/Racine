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
        
        [SerializeField] private float _width;

        protected void Start()
        {
            _width = RootManager.Instance.initialWidth;
            _rootParams = GetComponent<RootParams>();
            // Disable calyptra so that it only appears on leaves
            _rootParams.Calyptra.gameObject.SetActive(false);
            
            StartCoroutine(Sprout());
            _rootParams.Calyptra.Clicked += OnCalyptraClicked;
        }

        private void OnCalyptraClicked()
        {
            _maxDepth = _depth + RootManager.Instance.depthIncrement;
            Grow(mustHaveChildren: true);
        }

        private Vector3 GetCalyptraPosition()
        {
            return _rootParams.Calyptra.transform.position;
        }
        
        /// <summary>
        /// Grow children in a coroutine, scaling them along y
        /// </summary>
        /// <returns></returns>
        private IEnumerator Sprout()
        {
            float elapsedTime = 0f;
            Vector3 initialScale = new Vector3(1f, 0f, 1f);
            Vector3 finalScale = Vector3.one;
            float timeToGrow = RootManager.Instance.timeToGrowSprout;
            while (elapsedTime < timeToGrow)
            {
                transform.localScale = Vector3.Lerp(initialScale, finalScale, elapsedTime / timeToGrow);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            Grow();
        }

        private void Grow(bool mustHaveChildren = false)
        {
            bool hasNotBeenKilled = Random.Range(0f, 1f) > RootManager.Instance.killProbability;
            if (_depth < _maxDepth && (hasNotBeenKilled || mustHaveChildren))
            {
                CreateChildren();
                _rootParams.Calyptra.gameObject.SetActive(false);
            }
            else
            {
                _rootParams.Calyptra.gameObject.SetActive(true);
            }
        }

        private void CreateChildren()
        {
            bool isSplit = Random.Range(0f, 1f) < RootManager.Instance.splitProbability;

            if (isSplit)
            {
                Instantiate(_shapePrefab).AddComponent<Node>().Initialize(this, GetFanAngle());
                Instantiate(_shapePrefab).AddComponent<Node>().Initialize(this, -GetFanAngle());
            }
            else
            {
                var direction = Random.Range(0, 1) != 0 ? 1 : -1;
                Instantiate(_shapePrefab).AddComponent<Node>().Initialize(this, direction * GetFanAngle());
            }
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
        
        /// <summary>
        /// Recursively widen the parents by scaling them along X
        /// </summary>
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