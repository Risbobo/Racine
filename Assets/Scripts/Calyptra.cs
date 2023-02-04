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

        protected void Start()
        {
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
            SetSpriteAlpha(1f, 0.1f);
            transform.localScale *= HoverScaleFactor;
        }

        private void OnMouseExit()
        {
            SetSpriteAlpha(0f, 0.1f);
            transform.localScale /= HoverScaleFactor;
        }

        private void OnMouseDown()
        {
            Clicked();
        }
    }
}