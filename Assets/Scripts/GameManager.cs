using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Racines
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject _energyBar;
        [SerializeField] private TextMeshProUGUI _gameOverText;
        [SerializeField] private TextMeshProUGUI _gameOverReasonText;
        [SerializeField] private TextMeshProUGUI _ReplayText;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private float _energyInitial = 100;

        private static float _scoreValue;
        private static float _energyValue;

        private static Vector3 _energyBarScale;
        private float _sizeEnergyBar;

        private bool isGameOver = false;

        private static GameManager _instance;

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<GameManager>();
                }

                return _instance;
            }
        }

        private void Start()
        {
            _scoreValue = 0;
            _energyValue = _energyInitial;
            _sizeEnergyBar = _energyInitial;
            _gameOverPanel.SetActive(false);
            _gameOverReasonText.gameObject.SetActive(false);
            _ReplayText.gameObject.SetActive(false);

            _energyBarScale = _energyBar.transform.localScale;

            DrawBoard();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q) && !isGameOver)
            {
                GameOver(3);
            }
            if (isGameOver)
            {
                //If R is pressed: restart the game
                if (Input.GetKeyDown(KeyCode.R))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                //If Q is pressed: exit the game
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    Application.Quit();
                }
            }
        }

        private void DrawBoard()
        {
            _energyBar.transform.localScale = new Vector3(_sizeEnergyBar, _energyBarScale.y, _energyBarScale.z);
        }

        public void UpdateScore(float scoreIncrement)
        {
            _scoreValue += scoreIncrement;
            DrawBoard();
        }

        public void UpdateEnergy(float energyIncrement)
        {
            _energyValue += energyIncrement;

            if (_energyValue > 0)
            {
                _sizeEnergyBar = _sizeEnergyBar  + energyIncrement;
                DrawBoard();
            }
            else
            {
                _energyValue = 0;
                _sizeEnergyBar = 0;
                DrawBoard();
                GameOver(0);
            }

        }

        public void GameOver(int codeGameOver)
        {
            if (!isGameOver)
            {
                switch (codeGameOver)
                {
                    case 0:
                        _gameOverReasonText.text = "No energy left";
                        break;

                    case 1:
                        _gameOverReasonText.text = "No active Calyptra left";
                        break;

                    default:
                        _gameOverReasonText.text = "You just suck I guess...";
                        break;
                }

                isGameOver = true;

                StartCoroutine(GameOverSequence());
            }
        }

        private IEnumerator GameOverSequence()
        {
            _gameOverPanel.SetActive(true);
            _gameOverText.gameObject.SetActive(true);

            yield return new WaitForSeconds(1.0f);

            _gameOverReasonText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _ReplayText.gameObject.SetActive(true);
        }
    }
}