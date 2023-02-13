using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racines
{
    public class GrowthParams
    {
        public Vector3 growthDirection;
    }
    
    [RequireComponent(typeof(SpriteRenderer))]
    public class Calyptra : MonoBehaviour
    {
        private const float HoverScaleFactor = 1.5f;

        private Vector3 _normalScaleVector;
        private Vector3 _hoverScaleVector;

        private SpriteRenderer _spriteRenderer;

        public event Action<GrowthParams> SignalGrowth = delegate { };

        public Arrow _arrow;
        private bool _mouseDown;

        protected void Start()
        {
            _arrow = Arrow.Instance;
            _normalScaleVector = transform.localScale;
            _hoverScaleVector = HoverScaleFactor * transform.localScale;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            SetSpriteAlpha(0f, 0f);
            GameManager.Instance.calyptraList.Add(this);
        }

        private void SetSpriteAlpha(float alpha, float animationTime)
        {
            if (animationTime == 0f)
            {
                var color = _spriteRenderer.color;
                color.a = alpha;
                _spriteRenderer.color = color;
                return;
            }

            StartCoroutine(AlphaCoroutine(alpha, animationTime));
        }

        private IEnumerator AlphaCoroutine(float targetAlpha, float animationTime)
        {
            var color = _spriteRenderer.color;
            float initAlpha = color.a;
            float timeElapsed = 0f;
            while (timeElapsed < animationTime)
            {
                color.a = Mathf.Lerp(initAlpha, targetAlpha, timeElapsed / animationTime);
                _spriteRenderer.color = color;
                timeElapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            color.a = targetAlpha;
        }

        private void OnMouseEnter()
        {
            if (!GameManager.Instance.isGameOver && !_arrow.IsArrowActive)
            {
                HighlightCalyptra();
            }
        }

        private void OnMouseOver()
        {
            if (!GameManager.Instance.isGameOver && !_arrow.IsArrowActive)
            {
                HighlightCalyptra();
            }
        }

        private void OnMouseExit()
        {
            if (!GameManager.Instance.isGameOver && !_arrow.IsArrowActive)
            {
                DeHighlightCalyptra();
            }
        }

        private void OnMouseDown()
        {
            if (!GameManager.Instance.isGameOver && !_arrow.IsArrowActive)
            {
                 _mouseDown = true;
                // "Hack" so that if we just click on the Calyptra, it sends the position
                _arrow.ShowArrow(transform.position);
            }
        }

        private void OnMouseDrag()
        {
            if (!GameManager.Instance.isGameOver && _arrow.IsArrowActive)
            {
                _arrow.MutateArrow();
            }
        }

        private void OnMouseUp()
        {
            if (!GameManager.Instance.isGameOver && _arrow.IsArrowActive)
            {
                _mouseDown = false;
                SignalGrowth(new GrowthParams { growthDirection = _arrow.Direction });
                _arrow.HideArrow();
            }
        }

        public void HighlightCalyptra()
        {
            _arrow.HideArrow();
            SetSpriteAlpha(1f, 0.1f);
            transform.localScale = _hoverScaleVector;
        }

        public void DeHighlightCalyptra()
        {
            if (_mouseDown)
            {
                _arrow.ShowArrow(transform.position);
            }

            SetSpriteAlpha(0f, 0.1f);
            transform.localScale = _normalScaleVector;
        }
    }
}