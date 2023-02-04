using UnityEngine;

namespace Racines
{
    public class RootLine : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private Node _node;

        protected void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _node = GetComponentInParent<Node>();
        }

        protected void Update()
        {
            var positions = new Vector3[] { _node.GetRootPosition(), _node.GetTipPosition() };
            _lineRenderer.SetPositions(positions);
            _lineRenderer.startWidth = _node.Width;
            _lineRenderer.endWidth = _node.Width;
        }
    }
}