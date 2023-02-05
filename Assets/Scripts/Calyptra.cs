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
        private SpriteRenderer _spriteRenderer;

        public event Action<GrowthParams> SignalGrowth = delegate { };

        public Arrow _arrow;
        private bool _mouseDown;

        protected void Start()
        {
            _arrow = Arrow.Instance;
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
        }

        private void OnMouseEnter()
        {
            if (!GameManager.Instance.isGameOver)
            {
                highlightCalyptra();
            }
        }

        private void OnMouseExit()
        {
            if (!GameManager.Instance.isGameOver)
            {
                deHighlightCalyptra();
            }
        }

        private void OnMouseDown()
        {
            if (!GameManager.Instance.isGameOver)
            {
                 _mouseDown = true;
                // "Hack" so that if we just click on the Calyptra, it sends the position
                _arrow.ShowArrow(transform.position);
            }
        }

        private void OnMouseDrag()
        {
            if (!GameManager.Instance.isGameOver)
            {
                _arrow.MutateArrow();
            }
        }

        private void OnMouseUp()
        {
            if (!GameManager.Instance.isGameOver)
            {
                _mouseDown = false;
                if (_arrow.IsArrowActive)
                {
                    SignalGrowth(new GrowthParams { growthDirection = _arrow.Direction });
                    _arrow.HideArrow();
                }
            }
        }

        public void highlightCalyptra()
        {
            _arrow.HideArrow();
            SetSpriteAlpha(1f, 0.1f);
            transform.localScale *= HoverScaleFactor;
        }

        public void deHighlightCalyptra()
        {
            if (_mouseDown)
            {
                _arrow.ShowArrow(transform.position);
            }

            SetSpriteAlpha(0f, 0.1f);
            transform.localScale /= HoverScaleFactor;
        }
    }
}