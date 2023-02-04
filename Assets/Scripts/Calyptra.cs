using System;
using UnityEngine;

namespace Racines
{
    public class Calyptra : MonoBehaviour
    {
        private float _hoverScaleFactor = 1.5f;

        public event Action Clicked = delegate { };

        private void OnMouseEnter()
        {
            transform.localScale *= _hoverScaleFactor;
        }

        private void OnMouseExit()
        {
            transform.localScale /= _hoverScaleFactor;
        }

        private void OnMouseDown()
        {
            Clicked();
        }
    }
}