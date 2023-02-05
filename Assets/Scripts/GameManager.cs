using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

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

        [SerializeField] private float _energyDepletionFactor = 1;
        [SerializeField] private float _energyGainFactor = 1;

        private static float _scoreValue;
        private static float _energyValue;

        private static Vector3 _energyBarScale;
        private float _sizeEnergyBar;

        public bool isGameOver = false;

        public List<Calyptra> calyptraList;

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

            if (Input.GetKeyDown(KeyCode.LeftShift) && !isGameOver)
            {
                highlightAllCalyptra();
            }

            else if (Input.GetKeyUp(KeyCode.LeftShift) && !isGameOver)
            {
                deHighlightAllCalyptra();
            }

            else if (Input.GetKeyDown(KeyCode.Q) && !isGameOver)
            {
                GameOver(3);
            }
            else if (isGameOver)
            {
                //If R is pressed: restart the game
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Time.timeScale = 1;
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
            if (energyIncrement > 0)
            {
                _energyValue += energyIncrement * _energyGainFactor;
            }

            else
            {
                _energyValue += energyIncrement * _energyDepletionFactor;
            }

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
                // Pauses the game
                Time.timeScale = 0;
                StartCoroutine(GameOverSequence());
            }
        }

        private IEnumerator GameOverSequence()
        {
            _gameOverPanel.SetActive(true);
            _gameOverText.gameObject.SetActive(true);
            // Real time allows the coroutine to run even when the game is paused
            yield return new WaitForSecondsRealtime(1.0f);

            _gameOverReasonText.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(0.5f);
            _ReplayText.gameObject.SetActive(true);
        }

        // check if there are still active ends in the Game. If not: Game Over
        public void checkCalyptraExists()
        {
            //If calypra list is empty
            if (!calyptraList.Any())
            {
                GameOver(1);
            }
        }

        private void highlightAllCalyptra()
        {
            foreach (var ical in calyptraList)
            {
                ical.highlightCalyptra();
            }
        }

        private void deHighlightAllCalyptra()
        {
            foreach (var ical in calyptraList)
            {
                ical.deHighlightCalyptra();
            }
        }
    }
}