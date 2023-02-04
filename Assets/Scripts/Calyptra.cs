using System;
using UnityEngine;

namespace Racines
{
    public class Calyptra : MonoBehaviour
    {
        private float _hoverScaleFactor = 1.5f;

        private void OnMouseEnter()
        {
            transform.localScale *= _hoverScaleFactor;
        }

        private void OnMouseExit()
        {
            transform.localScale /= _hoverScaleFactor;
        }
    }
}