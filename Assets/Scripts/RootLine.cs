using UnityEngine;
using System.Collections.Generic;

namespace Racines
{
    public class RootLine : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private Node _node;
        private PolygonCollider2D _collider;

        public float intoParent = 0.1f;

        protected void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _node = GetComponentInParent<Node>();
            _collider = GetComponentInParent<PolygonCollider2D>();
        }

        protected void Update()
        {


            if (_node.Parent == null)
            {
                var positions = new Vector3[] { _node.GetRootPosition(), _node.GetTipPosition() };
                _lineRenderer.SetPositions(positions);
            }
            else
            {
                var positions = new Vector3[] { _node.Parent.GetRootPosition(), 
                                                _node.GetRootPosition(), 
                                                _node.GetTipPosition() };
                _lineRenderer.positionCount = 3;
                _lineRenderer.SetPositions(positions);
            }


            _lineRenderer.startWidth = _node.Width;
            _lineRenderer.endWidth = _node.Width;

            Vector2[] colliderPath = new Vector2[4] {
            new Vector2(-_node.Width/2f, 0),
            new Vector2(+_node.Width/2f, 0),
            new Vector2(+_node.Width/2f, _node.Length),
            new Vector2(-_node.Width/2f, _node.Length)
            };

            _collider.pathCount = 1;
            _collider.SetPath(0, colliderPath);

        }
    }
}