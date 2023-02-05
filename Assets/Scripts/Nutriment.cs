using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Racines;
using UnityEngine;
using Random = UnityEngine.Random;

public class Nutriment : MonoBehaviour
{
    [SerializeField] private float _nutriment;

    [SerializeField] private float _absorbFactor = 0.1f;

    private bool _isDead = false;
    // Start is called before the first frame update
    void Start()
    {
        _nutriment = Random.Range(20f, 100f);
        //_nutriment = 100f;
        transform.localScale = _nutriment / 100f * Vector3.one;
    }

    void Update()
    {
        if (_isDead)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var node = other.GetComponent<Node>();
        if (node == null)
        {
            return;
        }
        node.AddNutriment(gameObject.GetComponent<Nutriment>());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var node = other.GetComponent<Node>();
        if (node ==null)
        {
            return;
        }
        node.RemoveNutriment(gameObject.GetComponent<Nutriment>());
    }
    public float isAbsorbed(float width)
    {
        float absorbedNutrimentValue = _absorbFactor * width;

        _nutriment -= absorbedNutrimentValue;
        transform.localScale = _nutriment / 100f * Vector3.one;

        if (_nutriment <= 0)
        {
            _isDead = true;
        }

        return absorbedNutrimentValue;
    }
}
