using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racines
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Calyptra : MonoBehaviour
    {
        private const float HoverScaleFactor = 1.5f;
        private SpriteRenderer _spriteRenderer;

        public event Action Clicked = delegate { };

        public Arrow _arrow;
        private bool _mouseDown;

        protected void Start()
        {
            _arrow = Arrow.Instance;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            SetSpriteAlpha(0f, 0f);
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
            _arrow.HideArrow();
            SetSpriteAlpha(1f, 0.1f);
            transform.localScale *= HoverScaleFactor;
        }

        private void OnMouseExit()
        {
            if (_mouseDown)
            {
                _arrow.ShowArrow(transform.position);
            }
            
            SetSpriteAlpha(0f, 0.1f);
            transform.localScale /= HoverScaleFactor;
        }

        private void OnMouseDown()
        {
            _mouseDown = true;
            // "Hack" so that if we just click on the Calyptra, it sends the position
            _arrow.ShowArrow(transform.position);
        }

        private void OnMouseDrag()
        {
            _arrow.MutateArrow();
        }

        private void OnMouseUp()
        {
            _mouseDown = false;
            if (_arrow.IsArrowActive)
            {
                _arrow.HideArrow();
                Clicked(); 
            }
        }
    }
}