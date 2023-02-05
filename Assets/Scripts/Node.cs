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
        private List<Nutriment> _nutrimentsInContact = new List<Nutriment>();

        public ScoreManager _scoreManager;
        
        private float _width;
        private float _length;


        protected void Start()
        {
            _width = RootManager.Instance.initialWidth;
            _scoreManager = ScoreManager.Instance;
            
            StartCoroutine(Sprout());
        }

        protected void Update()
        {
            foreach (var nutriment in _nutrimentsInContact)
            {
                float absorbedNutrimentValue = nutriment.GetComponent<Nutriment>().isAbsorbed(_width);

                _scoreManager.UpdateEnergy(absorbedNutrimentValue);
            }
        }

        private void OnCalyptraClicked()
        {
            DestroyCalyptra();


            _maxDepth = _depth + RootManager.Instance.depthIncrement;
            Grow(mustHaveChildren: true);

            // check if there are still active ends in the Game
            //TODO GAMEOVER
            var areActiveEnds = GameObject.FindObjectOfType<Calyptra>();
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

            // Increase the score and decrease the energy for each new Node
            _scoreManager.UpdateScore(1);
            _scoreManager.UpdateEnergy(-1);
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