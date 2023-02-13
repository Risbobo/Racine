using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Racines
{
    public class GrowthParams
    {
        public Vector3 growthDirection;
    }
    
    [RequireComponent(typeof(SpriteRenderer))]
    public class Calyptra : MonoBehaviour
    {

        [SerializeField] private float HighlitedAlpha = 1f;
        [SerializeField] private float UnhighlitedAlpha = 0f;
        [SerializeField] private float AnimationTime = 0.1f;

        private const float HoverScaleFactor = 1.5f;

        private Vector3 _normalScaleVector;
        private Vector3 _hoverScaleVector;

        private SpriteRenderer _spriteRenderer;

        public event Action<GrowthParams> SignalGrowth = delegate { };

        public Arrow _arrow;
        private bool _mouseDown;

        private bool _Interactable = false;

        protected void Start()
        {
            _arrow = Arrow.Instance;
            _normalScaleVector = transform.localScale;
            _hoverScaleVector = HoverScaleFactor * transform.localScale;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            SetSpriteAlpha(UnhighlitedAlpha, 0f);
            

            if (transform.tag == "Root")
            {
                _Interactable = true;
                GameManager.Instance.calyptraList.Add(this);
            } else
            {
                GameManager.Instance.TreeCalyptraList.Add(this);
            }

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
            if (_Interactable && !GameManager.Instance.isGameOver && !_arrow.IsArrowActive)
            {
                HighlightCalyptra();
            }
        }


        private void OnMouseExit()
        {
            if (_Interactable && !GameManager.Instance.isGameOver && !_arrow.IsArrowActive)
            {
                DeHighlightCalyptra();
            }
        }

        private void OnMouseDown()
        {
            if (_Interactable && !GameManager.Instance.isGameOver && !_arrow.IsArrowActive)
            {
                 _mouseDown = true;
                // "Hack" so that if we just click on the Calyptra, it sends the position
                _arrow.ShowArrow(transform.position);
            }
        }

        private void OnMouseDrag()
        {
            if (_Interactable && !GameManager.Instance.isGameOver && _arrow.IsArrowActive)
            {
                _arrow.MutateArrow();
            }
        }

        private void OnMouseUp()
        {
            if (_Interactable && !GameManager.Instance.isGameOver && _arrow.IsArrowActive)
            {
                _mouseDown = false;
                SignalGrowth(new GrowthParams { growthDirection = _arrow.Direction });

                // Signat growth to a random tree calyptra (up direction)

                int selectCal = Random.Range(0, GameManager.Instance.TreeCalyptraList.Count);

                GameManager.Instance.TreeCalyptraList[selectCal].SignalGrowth(new GrowthParams { growthDirection = Vector3.up + Random.Range(-.1f,.1f)*Vector3.left });

                _arrow.HideArrow();
            }
        }

        public void HighlightCalyptra()
        {
            if (_Interactable)
            {
                _arrow.HideArrow();
                SetSpriteAlpha(HighlitedAlpha, AnimationTime);
                transform.localScale = _hoverScaleVector;
            }
        }

        public void DeHighlightCalyptra()
        {

            if (_Interactable)
            {
                if (_mouseDown)
                {
                    _arrow.ShowArrow(transform.position);
                }

                SetSpriteAlpha(UnhighlitedAlpha, AnimationTime);
                transform.localScale = _normalScaleVector;
            }
        }
    }
}