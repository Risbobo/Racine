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
        public float Length => _length;

        public Node Parent => _parent;

        [SerializeField] private int _depth;
        [SerializeField] private int _maxDepth = 5;
        [SerializeField] private GameObject _shapePrefab;
        [SerializeField] private Calyptra _calyptraPrefab;
        
        private Calyptra _calyptra;
        private Node _parent;
        private Coroutine _sproutRoutine;
        private List<Nutriment> _nutrimentsInContact = new List<Nutriment>();
        
        private float _width;
        private float _length;
        [SerializeField] private Vector3 _growthDirection;
        private float _probabilityToSpawn = 1f;


        protected void Start()
        {
            _width = RootManager.Instance.initialWidth;
            _sproutRoutine = StartCoroutine(Sprout());
        }

        protected void Update()
        {
            foreach (var nutriment in _nutrimentsInContact)
            {
                nutriment.GetComponent<Nutriment>().isAbsorbed(_width);
            }
        }

        protected void OnCollisionEnter2D(Collision2D other)
        {
            var calyptra = other.collider.GetComponent<Calyptra>();
            if (calyptra != null)
            {
                return;
            }
            var node = other.collider.GetComponent<Node>();
            if (node != null && (_parent == null || node == _parent || _parent.Children.Contains(node) || RootManager.Instance.selfCollide))
            {
                return;
            }
            StopCoroutine(_sproutRoutine);
        }

        private void OnCalyptraGrowthSignal(GrowthParams growthParams)
        {
            DestroyCalyptra();

            _growthDirection = growthParams.growthDirection;
            _probabilityToSpawn = 1f;
            _maxDepth = _depth + RootManager.Instance.depthIncrement;
            Grow();
        }

        public Vector3 GetRootPosition()
        {
            return transform.position;
        }

        public Vector3 GetTipPosition()
        {
            return transform.position + transform.rotation * new Vector3(0f, _length, 0f);
        }

        /// <summary>
        /// Grow children in a coroutine, scaling them along y
        /// </summary>
        /// <returns></returns>
        private IEnumerator Sprout()
        {
            float elapsedTime = 0f;
            float finalLength = Utils.RandomFromGaussion(mean: RootManager.Instance.meanSproutLength,
                                               sigma: RootManager.Instance.sproutLengthSigma);
            float timeToGrow = RootManager.Instance.timeToGrowSprout;
            while (elapsedTime < timeToGrow)
            {
                _length = Mathf.Lerp(0f, finalLength, elapsedTime / timeToGrow);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            Grow();
        }

        private void Grow()
        {
            bool mustSpawn = Random.Range(0f, 1f) <= _probabilityToSpawn;
            if (_depth < _maxDepth && mustSpawn)
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
            _calyptra = Instantiate(_calyptraPrefab, GetTipPosition() + Vector3.back * 0.2f, Quaternion.identity);
            _calyptra.SignalGrowth += OnCalyptraGrowthSignal;
        }

        private void DestroyCalyptra()
        {
            _calyptra.SignalGrowth -= OnCalyptraGrowthSignal;
            Destroy(_calyptra.gameObject);
        }

        private void CreateChildren()
        {
            float angle = Random.Range(-RootManager.Instance.maxFirstAngle, RootManager.Instance.maxFirstAngle);
            CreateChild(angle, isSplit: false);
            
            bool isSplit = Random.Range(0f, 1f) < RootManager.Instance.splitProbability;

            if (isSplit)
            {
                var direction = Mathf.Sign(Random.Range(-1f, 1f));
                float splitAngle = Random.Range(RootManager.Instance.minFanAngle, RootManager.Instance.maxFanAngle);
                CreateChild(angle + direction * splitAngle, isSplit: true);
            }
        }

        private void CreateChild(float angle, bool isSplit)
        {
            Node child = Instantiate(_shapePrefab).AddComponent<Node>();
            child.Initialize(this, angle, isSplit);
            Children.Add(child);
        }
        
        private static float GetFanAngle()
        {
            return Random.Range(RootManager.Instance.minFanAngle, RootManager.Instance.maxFanAngle);
        }
        
        private void Initialize(Node parent, float angle, bool isSplit)
        {
            _parent = parent;

            _shapePrefab = parent._shapePrefab;
            _calyptraPrefab = parent._calyptraPrefab;
            _growthDirection = parent._growthDirection;
            _probabilityToSpawn = parent._probabilityToSpawn;
            _maxDepth = parent._maxDepth;
            _depth = parent._depth + 1;

            // Split branches are more short-lived
            if (isSplit)
            {
                _probabilityToSpawn *= RootManager.Instance.splitSurvivalRatio;
            }
            
            transform.position = parent.GetTipPosition();
            transform.rotation = GetRotation(angle);
            
            StartCoroutine(parent.Widen());
        }
        
        private Quaternion GetRotation(float angle)
        {
            var rotationWithoutPull = Quaternion.AngleAxis(angle, Vector3.forward) * _parent.transform.rotation;
            var directionWithoutPull = rotationWithoutPull * Vector3.up;
            float alpha = RootManager.Instance.directionalPullStrength;
            var direction = alpha * _growthDirection + (1 - alpha) * directionWithoutPull;
            return Quaternion.FromToRotation(Vector3.up, direction);
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
            
            float elapsedTime = 0f;
            float oldWidth = _width;
            float newWidth = Mathf.Sqrt(_width * _width + RootManager.Instance.initialWidth * RootManager.Instance.initialWidth);
            float timeToGrow = RootManager.Instance.timeToGrowSprout;
            while (elapsedTime < timeToGrow)
            {
                _width = Mathf.Lerp(oldWidth, newWidth, elapsedTime / timeToGrow);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        public void AddNutriment(Nutriment other)
        {
            var newNutriment = other.GetComponent<Nutriment>();
            _nutrimentsInContact.Add(newNutriment);
        }
        
        public void RemoveNutriment(Nutriment other)
        {
            var oldNutriment = other.GetComponent<Nutriment>();
            _nutrimentsInContact.Remove(oldNutriment);
        }
    }
}