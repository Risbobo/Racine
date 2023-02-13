using Racines;
using UnityEngine;

public class Nutrient : MonoBehaviour
{
    private float _absorbFactor;
    private float _nutrient;
    private float _nutrientSize;

    public void Update()
    {
       
    }

    public bool Initialize(Vector3 position, float nutrientValue, float nutrientSize, float nutrientAbsorbFactor)
    {
        transform.position = position;
        _nutrient = nutrientValue;
        transform.localScale = nutrientSize* Vector3.one;
        _nutrientSize = nutrientSize;
        _absorbFactor = nutrientAbsorbFactor;

        return true;
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
