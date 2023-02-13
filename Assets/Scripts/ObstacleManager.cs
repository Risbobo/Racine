using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Racines
{
    public class ObstacleManager : MonoBehaviour
    {
        private GameManager _gameManager;

        public int obstacleNum = 15;

        void Start()
        {
            _gameManager = GameManager.Instance;
            ObstacleGenerator(obstacleNum);
        }

        public void ObstacleGenerator(int limitObstacletNum)
        {
            int obstacleNumCurrent = 0;

            while (obstacleNumCurrent < limitObstacletNum)
            {
                // Determine obstacle size, roation and position
                float obstacleSize = Random.Range(20f, 100f);
                float obstacleRotation = Random.Range(0f, 360f);
                Vector3 obstaclePosition = _gameManager.GetRandomGroundPosition(new Vector2(obstacleSize, obstacleSize));

                //Rotate(Vector3.forward, obstacleRotation);

                //// Check for collision with other collider objects (rocks, other nutrient, etc)
                //if (Physics2D.OverlapCircle(nutrientPosition, 2 * nutrientSize) == null)
                //{
                //    // Create new Nutrient
                //    Nutrient newNutrient = Instantiate(_nutrientPrefab, parent: transform);

                //    newNutrient.Initialize(nutrientPosition, nutrientValue, nutrientSize, _nutrientAbsorbFactor);

                //    // Add to the nutrient list
                //    _gameManager.nutrientList.Add(newNutrient);

                //    nutrientNumCurrent += 1;
                //}
            }
        }

    }
}