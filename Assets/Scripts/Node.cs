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
        private bool _isLeaf;

        protected void Start()
        {
            _rootParams = GetComponent<RootParams>();
            Grow();
            _rootParams.Calyptra.Clicked += OnCalyptraClicked;
        }

        private void OnCalyptraClicked()
        {
            _maxDepth += 3;
            Grow();
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
            Instantiate(_shapePrefab, transform).AddComponent<Node>().Initialize(this);
            Instantiate(_shapePrefab, transform).AddComponent<Node>().Initialize(this, -1f);
        }
        
        public void Initialize(Node parent, float factor = 1f)
        {
            transform.parent = parent.transform;
            transform.localPosition = new Vector3(0f, 1f, 0f);
            transform.localRotation = Quaternion.AngleAxis(factor * Random.Range(30f, 70f), Vector3.forward);
            transform.localScale = Random.Range(0.5f, 0.9f) * Vector3.one;
            _shapePrefab = parent._shapePrefab;
            _maxDepth = parent._maxDepth;
            _depth = parent._depth + 1;
        }
    }
}