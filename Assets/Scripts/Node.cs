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
        public RootManager RootManager => _rootManager;

        

        [SerializeField] private int _depth;
        [SerializeField] private int _maxDepth = 5;
        [SerializeField] private RootManager _rootManager;
        [SerializeField] private GameObject _shapePrefab;
        [SerializeField] private Calyptra _calyptraPrefab;

        private Calyptra _calyptra;
        private Node _parent;
        private Coroutine _sproutRoutine;

        public GameManager _gameManager;
        
        private float _width;
        private float _length;
        [SerializeField] private Vector3 _pullDirection = Vector3.down;
        private float _probabilityToSpawn = 1f;
        private float _pullStrength;


        protected void Start()
        {
            _width = _rootManager.initialWidth;
            _sproutRoutine = StartCoroutine(Sprout());
            _gameManager = GameManager.Instance;
        }

        protected void OnCollisionEnter2D(Collision2D other)
        {
            var calyptra = other.collider.GetComponent<Calyptra>();
            if (calyptra != null)
            {
                return;
            }
            var node = other.collider.GetComponent<Node>();
            if (node != null && (_parent == null || node == _parent || _parent.Children.Contains(node) || _rootManager.selfCollide))
            {
                return;
            }
            StopCoroutine(_sproutRoutine);
        }

        private void OnCalyptraGrowthSignal(GrowthParams growthParams)
        {
            DestroyCalyptra();

            _pullDirection = growthParams.growthDirection;
            _pullStrength = _rootManager.directionalPullStrength;
            _probabilityToSpawn = 1f;
            _maxDepth = _depth + _rootManager.depthIncrement;
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
            float finalLength = Utils.RandomFromGaussion(mean: _rootManager.meanSproutLength,
                                               sigma: _rootManager.sproutLengthSigma);
            float timeToGrow = _rootManager.timeToGrowSprout;
            while (elapsedTime < timeToGrow)
            {
                _length = Mathf.Lerp(0f, finalLength, elapsedTime / timeToGrow);
                elapsedTime += Time.deltaTime;

                // Decrease the energy for each new Node while it grows
                GameManager g = GameManager.Instance;

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
            _gameManager.calyptraList.Remove(_calyptra);
            Destroy(_calyptra.gameObject);
        }

        private void CreateChildren()
        {
            float angle = GetSproutAngle();
            CreateChild(angle, isSplit: false);
            
            bool isSplit = Random.Range(0f, 1f) < _rootManager.splitProbability;

            if (isSplit)
            {
                var direction = Mathf.Sign(Random.Range(-1f, 1f));
                float splitAngle = Random.Range(_rootManager.minFanAngle, _rootManager.maxFanAngle);
                CreateChild(angle + direction * splitAngle, isSplit: true);
            }

        }

        private void CreateChild(float angle, bool isSplit)
        {
            Node child = Instantiate(_shapePrefab, GetComponentInParent<Transform>()).AddComponent<Node>();
            child.Initialize(this, angle, isSplit);
            Children.Add(child);

            // Increase the score for each new Node
            _gameManager.UpdateScore(1);
        }
        
        private void Initialize(Node parent, float angle, bool isSplit)
        {
            _parent = parent;

            _shapePrefab = parent._shapePrefab;
            _calyptraPrefab = parent._calyptraPrefab;
            _rootManager = parent._rootManager;
            _pullDirection = parent._pullDirection;
            _probabilityToSpawn = parent._probabilityToSpawn;
            _maxDepth = parent._maxDepth;
            _depth = parent._depth + 1;

            // Split branches are more short-lived, and don't pull as strongly
            if (isSplit)
            {
                _probabilityToSpawn *= _rootManager.splitSurvivalRatio;
                _pullStrength *= _rootManager.splitPullStrengthRatio;
            }
            
            transform.SetPositionAndRotation(parent.GetTipPosition(), Quaternion.AngleAxis(angle, Vector3.forward) * parent.transform.rotation);
            
            StartCoroutine(parent.Widen());
        }
        
        private float GetSproutAngle()
        {
            var growthDirection = transform.rotation * Vector3.up;

            var direction = (_pullStrength * _pullDirection + (1f - _pullStrength) * growthDirection).normalized;
            var angle = Vector3.SignedAngle(growthDirection, direction, Vector3.forward);
            var sign = Mathf.Sign(angle);
            angle = sign * Mathf.Clamp(Mathf.Abs(angle), 0f, _rootManager.maxSproutAngle);

            return Utils.RandomFromGaussion(angle, _rootManager.sproutAngleSigma);
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
            float newWidth = Mathf.Sqrt(_width * _width + _rootManager.initialWidth * _rootManager.initialWidth);
            float timeToGrow = _rootManager.timeToGrowSprout;
            while (elapsedTime < timeToGrow)
            {
                _width = Mathf.Lerp(oldWidth, newWidth, elapsedTime / timeToGrow);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}