using UnityEngine;
using System.Collections.Generic;

namespace Racines
{
    public class RootLine : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private Node _node;
        private PolygonCollider2D _collider;

        public float intoParent = 0.2f;
        public int curveSegments = 10;

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
                var positions = new Vector3[] { Vector3.Lerp(_node.GetRootPosition(), _node.Parent.GetRootPosition(), intoParent),
                                                _node.GetRootPosition(),
                                                Vector3.Lerp(_node.GetRootPosition(),_node.GetTipPosition(), intoParent),
                                                Vector3.Lerp(_node.GetTipPosition(),_node.GetRootPosition(), intoParent) };

                if (_node.Children.Count == 0 )
                {
                    positions[3] = _node.GetTipPosition();
                }

                var Curve = new Vector3[curveSegments+1];

                for (int i = 0; i < curveSegments; i++)
                {
                    //Quadratic Bezier curve from https://en.wikipedia.org/wiki/B%C3%A9zier_curve
                    float t = (float)i / (float)curveSegments;

                    Curve[i] = positions[1] + (1 - t) * (1 - t) * (positions[0] - positions[1]) + t * t * (positions[2] - positions[1]);
                }

                Curve[curveSegments] = positions[3];

                _lineRenderer.positionCount = curveSegments +1;
                _lineRenderer.SetPositions(Curve);
            }


            _lineRenderer.startWidth = _node.Width;
            _lineRenderer.endWidth = _node.Width;

            if (_node.Children.Count > 0)
            {
                float MaxChildWidth = 0;

                for (int i = 0; i < _node.Children.Count; i++)
                {
                    float w = _node.Children[i].Width;
                    if (MaxChildWidth < w)
                    {
                        MaxChildWidth = w;
                    }
                }
                _lineRenderer.endWidth = MaxChildWidth;
            }

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