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
        public float mapMinX, mapMaxX, mapMinY, mapMaxY;

        [SerializeField] private GameObject _energyBar;
        [SerializeField] private TextMeshProUGUI _gameOverText;
        [SerializeField] private TextMeshProUGUI _gameOverReasonText;
        [SerializeField] private TextMeshProUGUI _ReplayText;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private SpriteRenderer _sceneGround;
        [SerializeField] private float _energyInitial = 100;

        [SerializeField] private float _energyDepletionFactor = 1;
        [SerializeField] private float _energyGainFactor = 1;

        public float nodeGrowCost = 0.05f;

        public float scoreValue;
        public float energyValue;

        private Vector3 _energyBarScale;
        private float _sizeEnergyBar;

        public bool isGameOver = false;

        public List<Calyptra> calyptraList;
        public List<Nutrient> nutrientList;

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
            scoreValue = 0;
            energyValue = _energyInitial;
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
                HighlightAllCalyptra();
            }

            else if (Input.GetKeyUp(KeyCode.LeftShift) && !isGameOver)
            {
                DeHighlightAllCalyptra();
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
            scoreValue += scoreIncrement;
        }

        public void UpdateEnergy(float energyIncrement)
        {
            // Calculate the new energy levels
            if (energyIncrement > 0)
            {
                energyValue += energyIncrement * _energyGainFactor;
            }
            else
            {
                energyValue += energyIncrement * _energyDepletionFactor;
            }
            // Update the size of the energy bar
            if (energyValue > 0)
            {
                _sizeEnergyBar += energyIncrement;
                DrawBoard();
            }
            else
            {
                energyValue = 0;
                _sizeEnergyBar = 0;
                DrawBoard();
                GameOver(0);
            }
        }

        public void GameOver(int codeGameOver)
        {
            if (!isGameOver)
            {

                _gameOverReasonText.text = codeGameOver switch
                {
                    0 => "No energy left",
                    1 => "No active Calyptra left",
                    _ => "You just suck I guess...",
                };

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
        public void CheckCalyptraExists()
        {
            //If calypra list is empty
            if (!calyptraList.Any())
            {
                GameOver(1);
            }
        }

        private void HighlightAllCalyptra()
        {
            foreach (var ical in calyptraList)
            {
                ical.HighlightCalyptra();
            }
        }

        private void DeHighlightAllCalyptra()
        {
            foreach (var ical in calyptraList)
            {
                ical.DeHighlightCalyptra();
            }
        }

        public Vector2 GetRandomGroundPosition(Vector2 boundaries = default)
        {
            Vector3 groundCenter = _sceneGround.bounds.center;
            Vector3 groundExtents = _sceneGround.bounds.extents;

            float maxGroundX = groundCenter.x + groundExtents.x - boundaries.x;
            float minGroundX = groundCenter.x - groundExtents.x + boundaries.x;

            float maxGroundY = groundCenter.y + groundExtents.y - boundaries.y;
            float minGroundY = groundCenter.y - groundExtents.y + boundaries.y;

            float x = Random.Range(minGroundX, maxGroundX);
            float y = Random.Range(minGroundY, maxGroundY);

            return new Vector2(x, y);
        }
    }
}