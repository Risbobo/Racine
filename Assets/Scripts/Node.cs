using System.Collections;
using UnityEngine;

namespace Racines
{
    public class Node : MonoBehaviour
    {
        private int _depth;
        [SerializeField] private int _maxDepth = 5;
        [SerializeField] private GameObject _shapePrefab;
        private bool _isLeaf;

        protected void Start()
        {
            if (_depth < _maxDepth)
            {
                StartCoroutine(CreateChildren());
            }
            else
            {
                _isLeaf = true;
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