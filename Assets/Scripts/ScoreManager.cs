using UnityEngine;
using TMPro;

namespace Racines
{
    public class ScoreManager : MonoBehaviour
    {
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI energyText;
        public float energyInitial;

        private static float _scoreValue;
        private static float _energyValue;

        private static ScoreManager _instance;

        public static ScoreManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<ScoreManager>();
                }

                return _instance;
            }
        }

        private void Start()
        {
            _scoreValue = 0;
            _energyValue = energyInitial;

            DrawBoard();
        }

        private void DrawBoard()
        {
            scoreText.text = "Score: " + (int) _scoreValue;
            energyText.text = "Energy: " + (int) _energyValue;
        }

        public void UpdateScore(float scoreIncrement)
        {
            _scoreValue += scoreIncrement;
            DrawBoard();
        }

        public bool UpdateEnergy(float energyIncrement)
        {
            _energyValue += energyIncrement;

            if (_energyValue > 0)
            {
                DrawBoard();
                return true;

            }

            return false;
        }
    }
}