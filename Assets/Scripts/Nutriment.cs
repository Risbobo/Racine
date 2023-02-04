using System.Collections;
using System.Collections.Generic;
using Racines;
using UnityEngine;
using Random = UnityEngine.Random;

public class Nutriment : MonoBehaviour
{
    [SerializeField] private float _nutriment;

    [SerializeField] private float _absorbFactor = 1f;
    // Start is called before the first frame update
    void Start()
    {
        _nutriment = Random.Range(20f, 100f);
        transform.localScale = _nutriment / 100f * Vector3.one;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        print("OnTriggerActivated");
        if (other.tag == "Root")
        {
            var root = other.GetComponent<Node>();
            root.AddNutriment(gameObject.GetComponent<Nutriment>());
        }
    }
    public void isAbsorbed(float width)
    {
        _nutriment -= _absorbFactor * width;
        transform.localScale = _nutriment / 100f * Vector3.one;
        if (_nutriment <= 0)
        {
            Destroy(gameObject);
        }
    }
}
