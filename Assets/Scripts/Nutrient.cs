using Racines;
using UnityEngine;
using Random = UnityEngine.Random;

public class Nutrient : MonoBehaviour
{
    [SerializeField] private float _nutrient;

    [SerializeField] private float _absorbFactor = 0.1f;

    //private bool _isDead = false;
    // Start is called before the first frame update
    void Start()
    {
        _nutrient = Random.Range(20f, 100f);
        //_nutrient = 100f;
        transform.localScale = _nutrient / 60f * Vector3.one;
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
    public float IsAbsorbed(float width)
    {
        float absorbedNutrientValue = _absorbFactor * width;

        _nutrient -= absorbedNutrientValue;
        transform.localScale = _nutrient / 60f * Vector3.one;

        if (_nutrient <= 0)
        {
            Destroy(gameObject);
        }

        return absorbedNutrientValue;
    }
}
