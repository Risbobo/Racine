using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine;

namespace Racines
{
    public class NutrientManager : MonoBehaviour
    {
        [SerializeField] private Nutrient _nutrientPrefab;
        [SerializeField] private float _nutrientAbsorbFactor;

        private GameManager _gameManager;

        public int nutrientNum = 15;
        public float nutrientDistributionDensity;
        public float nutrientDistributionDensityHomogeneity;
        public float nutrientSizeHomogeneity;

        void Start()
        {
            _gameManager = GameManager.Instance;
            NutrientGenerators();
        }

        public void NutrientGenerators()
        {
            for (int i = 0; i < nutrientNum; i++)
            {
                // Create new Nutrient
                Nutrient newNutrient = Instantiate(_nutrientPrefab, parent: transform);

                // Set nutrient value and size
                float nutrientValue = Random.Range(20f, 100f);
                float nutrientSize = nutrientValue / 60f;
                newNutrient.Initialize(_gameManager.GetRandomGroundPosition(new Vector2(nutrientSize, nutrientSize)), nutrientValue, nutrientSize, _nutrientAbsorbFactor);

                // Check for collision with rock and other nutrient
                if (Physics2D.IsTouchingLayers(newNutrient.GetComponent<CircleCollider2D>()))
                {
                    print("Collide");
                }

                // Add to the nutrient list
                _gameManager.nutrientList.Add(newNutrient);
            }
        }

    }
}