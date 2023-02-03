using System;
using UnityEngine;

namespace Racines
{
    public class CubeMover : MonoBehaviour
    {
        [SerializeField] private float _omega;
        [SerializeField] private float _amplitude;
        private Vector3 _initialPosition;

        protected void Start()
        {
            _initialPosition = gameObject.transform.localPosition;
        }

        protected void Update()
        {
            gameObject.transform.localPosition = _initialPosition + new Vector3(0f, _amplitude * Mathf.Sin(_omega * Time.time), 0f);
        }
    }
}