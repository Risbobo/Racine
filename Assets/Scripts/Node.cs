using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Racines
{
    public class Node : MonoBehaviour
    {
        [SerializeField] private int _depth;
        [SerializeField] private float _initialWidth = 1f;
        [SerializeField] private int _maxDepth = 5;
        [SerializeField] private GameObject _shapePrefab;
        [SerializeField] private float _shrinkFactor;
        [SerializeField] private float _timeToGrow = 1;
        [SerializeField] private float _frames = 10;
        
        private RootParams _rootParams;
        private Node _parent;
        private bool _isLeaf;
        [SerializeField] private float _width;

        protected void Start()
        {
            _width = _initialWidth;
            _rootParams = GetComponent<RootParams>();
            StartCoroutine(Sprout());
            _rootParams.Calyptra.Clicked += OnCalyptraClicked;
        }

        private void OnCalyptraClicked()
        {
            _maxDepth += 3;
            //StartCoroutine(Grow());
            Grow();
        }

        private Vector3 GetCalyptraPosition()
        {
            return _rootParams.Calyptra.transform.position;
        }

        private IEnumerator Sprout()
        {
            _rootParams.Calyptra.gameObject.SetActive(false);
            for (float alpha = 0; alpha <= 1; alpha += 1f / _frames)
            {
                transform.localScale = new Vector3(1, alpha, 1);
                yield return new WaitForSeconds(_timeToGrow / _frames);
            }

            Grow();
        }
        
        private void Grow()
        {
            if (_depth < _maxDepth)
            {
                //StartCoroutine(CreateChildren());
                CreateChildren();
                _rootParams.Calyptra.gameObject.SetActive(false);
            }
            else
            {
                _isLeaf = true;
                _rootParams.Calyptra.gameObject.SetActive(true);
            }
        }

        //private IEnumerator CreateChildren()
        private void CreateChildren()
        {
            //yield return new WaitForSeconds(0.5f);
            Instantiate(_shapePrefab).AddComponent<Node>().Initialize(this);
            Instantiate(_shapePrefab).AddComponent<Node>().Initialize(this, -1f);
        }
        
        private void Initialize(Node parent, float factor = 1f)
        {
            _shrinkFactor = parent._shrinkFactor;
            _parent = parent;

            transform.position = parent.GetCalyptraPosition();
            transform.rotation = Quaternion.AngleAxis(factor * Random.Range(15f, 75f), Vector3.forward) * parent.transform.rotation;
            transform.localScale = Vector3.zero;

            _shapePrefab = parent._shapePrefab;
            _maxDepth = parent._maxDepth;
            _initialWidth = parent._initialWidth;
            _depth = parent._depth + 1;

            //StartCoroutine(Sprout());

            parent.GrowWidth();
        }

        private void GrowWidth()
        {
            // To Do : Rendu graphique en fonction de la _width
            var newWidth = Mathf.Sqrt(_width * _width + _initialWidth * _initialWidth);
            
            var scale = transform.localScale;
            transform.localScale = new Vector3((newWidth / _width) * scale.x, scale.y, scale.z);
            _width = newWidth;

            if (_parent != null)
            {
                _parent.GrowWidth();
            }
        }
    }
}