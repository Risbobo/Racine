using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Racines
{
    public class Node : MonoBehaviour
    {
        public List<Node> Children { get; } = new List<Node>();
        public float Width => _width;
        
        [SerializeField] private int _depth;
        [SerializeField] private int _maxDepth = 5;
        [SerializeField] private GameObject _shapePrefab;
        [SerializeField] private Calyptra _calyptraPrefab;
        
        private Calyptra _calyptra;
        private Node _parent;
        private float _width;
        private List<Nutriment> _nutrimentsInContact = new List<Nutriment>();
        
        protected void Start()
        {
            _width = RootManager.Instance.initialWidth;
            StartCoroutine(Sprout());
        }

        protected void Update()
        {
            foreach (var nutriment in _nutrimentsInContact)
            {
                nutriment.GetComponent<Nutriment>().isAbsorbed(_width);
            }
        }

        private void OnCalyptraClicked()
        {
            DestroyCalyptra();
            _maxDepth = _depth + RootManager.Instance.depthIncrement;
            Grow(mustHaveChildren: true);
        }

        public Vector3 GetRootPosition()
        {
            return transform.position;
        }

        public Vector3 GetTipPosition()
        {
            return transform.position + transform.rotation * new Vector3(0f, transform.localScale.y, 0f);
        }
        
        /// <summary>
        /// Grow children in a coroutine, scaling them along y
        /// </summary>
        /// <returns></returns>
        private IEnumerator Sprout()
        {
            float elapsedTime = 0f;
            Vector3 initialScale = new Vector3(1f, 0f, 1f);
            float length = Utils.RandomFromGaussion(mean: 1f, sigma: RootManager.Instance.sproutLengthSigma);
            Vector3 finalScale = new Vector3(1f, length, 1f);
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
            }
            else
            {
                CreateCalyptra();
            }
        }

        private void CreateCalyptra()
        {
            _calyptra = Instantiate(_calyptraPrefab, GetTipPosition(), Quaternion.identity);
            _calyptra.Clicked += OnCalyptraClicked;
        }

        private void DestroyCalyptra()
        {
            _calyptra.Clicked -= OnCalyptraClicked;
            Destroy(_calyptra.gameObject);
        }

        private void CreateChildren()
        {
            bool isSplit = Random.Range(0f, 1f) < RootManager.Instance.splitProbability;

            if (isSplit)
            {
                CreateChild(GetFanAngle());
                CreateChild(-GetFanAngle());
            }
            else
            {
                var direction = Random.Range(0, 1) != 0 ? 1 : -1;
                CreateChild(direction * GetFanAngle());
            }
        }

        private void CreateChild(float angle)
        {
            Node child = Instantiate(_shapePrefab).AddComponent<Node>();
            child.Initialize(this, angle);
            Children.Add(child);
        }
        
        private static float GetFanAngle()
        {
            return Random.Range(RootManager.Instance.minFanAngle, RootManager.Instance.maxFanAngle);
        }
        
        private void Initialize(Node parent, float angle)
        {
            _parent = parent;

            transform.position = parent.GetTipPosition();
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward) * parent.transform.rotation;
            
            _shapePrefab = parent._shapePrefab;
            _calyptraPrefab = parent._calyptraPrefab;
            _maxDepth = parent._maxDepth;
            _depth = parent._depth + 1;
            
            StartCoroutine(parent.Widen());
        }
        
        /// <summary>
        /// Recursively widen the parents by scaling them along X
        /// </summary>
        private IEnumerator Widen()
        {
            if (_parent != null)
            {
                StartCoroutine(_parent.Widen());
            }
            var newWidth = Mathf.Sqrt(_width * _width + RootManager.Instance.initialWidth * RootManager.Instance.initialWidth);
            var scale = transform.localScale;
            float elapsedTime = 0f;
            Vector3 initialScale = new Vector3(scale.x, scale.y, scale.z);
            Vector3 finalScale = new Vector3((newWidth / _width) * scale.x, scale.y, scale.z);
            float timeToGrow = RootManager.Instance.timeToGrowSprout;
            while (elapsedTime < timeToGrow)
            {
                transform.localScale = Vector3.Lerp(initialScale, finalScale, elapsedTime / timeToGrow);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            _width = newWidth;
        }

        public void AddNutriment(Nutriment other)
        {
            var newNutriment = other.GetComponent<Nutriment>();
            _nutrimentsInContact.Add(newNutriment);
        }
    }
}