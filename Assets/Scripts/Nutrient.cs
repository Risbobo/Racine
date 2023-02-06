using Racines;
using UnityEngine;

public class Nutrient : MonoBehaviour
{
    [SerializeField] private float _nutrient;
    [SerializeField] private float _absorbFactor = 0.1f;

    void Start()
    {
        if (GetComponent<CircleCollider2D>().IsTouchingLayers())
                {
                    print("Aouch start");
        }
    }

    public void Initialize(float nutrientValue, float nutrientSize)
    {
        _nutrient = nutrientValue;
        transform.localScale = nutrientSize* Vector3.one;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var node = other.GetComponent<Node>();

        if (node == null)
        {
            return;
        }

        var rate = node.Width;
        var energy = IsAbsorbed(rate);
        GameManager.Instance.UpdateEnergy(energy);
    }

    private float IsAbsorbed(float width)
    {
        float absorbedNutrientValue = _absorbFactor * width;

        _nutrient -= absorbedNutrientValue;
        transform.localScale = _nutrient / 60f * Vector3.one;

        if (_nutrient <= 0)
        {
            absorbedNutrientValue += _nutrient; // To avoid getting more nutrient than is possible (_nutrient < 0 case)
            Destroy(gameObject);
        }

        return absorbedNutrientValue;
    }
}
