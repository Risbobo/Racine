using Random = UnityEngine.Random;
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
            NutrientGenerator(nutrientNum);
        }

        public void NutrientGenerator(int limitNutrientNum)
        {
            int nutrientNumCurrent = 0;

            while (nutrientNumCurrent < limitNutrientNum)
            {
                // Determine nutrient value, size and position
                float nutrientValue = Random.Range(20f, 100f);
                float nutrientSize = nutrientValue / 60f;
                Vector3 nutrientPosition = _gameManager.GetRandomGroundPosition(new Vector2(nutrientSize, nutrientSize));
                
                // Check for collision with other collider objects (rocks, other nutrient, etc)
                if (Physics2D.OverlapCircle(nutrientPosition, 2*nutrientSize) == null)
                {
                    // Create new Nutrient
                    Nutrient newNutrient = Instantiate(_nutrientPrefab, parent: transform);

                    newNutrient.Initialize(nutrientPosition, nutrientValue, nutrientSize, _nutrientAbsorbFactor);

                    // Add to the nutrient list
                    _gameManager.nutrientList.Add(newNutrient);

                    nutrientNumCurrent += 1;
                }
            }
        }
    }
}