using UnityEngine;

namespace Racines
{
    public class Arrow : MonoBehaviour
    {

        [SerializeField] private GameObject _arrowPrefab;
        [SerializeField] private GameObject _shaft;
        [SerializeField] private GameObject _arrowhead;
        [SerializeField] private float _maxSizeArrow = 2.5f;

        private Vector3 _scaleShaftOrigin;
        
        private static Arrow _instance;

        public bool IsArrowActive => _arrowPrefab.activeSelf;

        private float ArrowLength { get; set; }

        public static Arrow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<Arrow>();
                }

                return _instance;
            }
        }

        private void Start()
        {
            _scaleShaftOrigin = _shaft.transform.localScale;
            _arrowPrefab.SetActive(false);
        }

        public void ShowArrow(Vector3 position)
        {
            _arrowPrefab.SetActive(true);
            _arrowPrefab.transform.position = position;
            MutateArrow();
        }
        
        public void HideArrow()
        {
            _arrowPrefab.SetActive(false);
        }


        //Modify the arrow (rotation, length) depending on the mouse position
        public void MutateArrow()
        {
            //Rotate the arrow to follow the mouse
            Vector3 mouseWorldPosition3D = GetMouseWorldPosition();
            Vector3 arrowPosition3D = _arrowPrefab.transform.position;

            Vector2 arrowPoint2D = new Vector2(arrowPosition3D.x, arrowPosition3D.y);
            Vector2 mousePoint2D = new Vector2(mouseWorldPosition3D.x, mouseWorldPosition3D.y);

            _arrowPrefab.transform.right = mousePoint2D - arrowPoint2D;

            //scale up the Arrow in function of the distance of the mouse
            ArrowLength = Vector2.Distance(arrowPoint2D, mousePoint2D);
            if (ArrowLength > _maxSizeArrow)
            {
                ArrowLength = _maxSizeArrow;
            }

            _shaft.transform.localScale = new Vector3(_scaleShaftOrigin.x, ArrowLength, _scaleShaftOrigin.z);
            _arrowhead.transform.position = Vector2.MoveTowards(arrowPoint2D, mousePoint2D, _maxSizeArrow);
        }

        //Get the mouse position from the screen to the Game POV
        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = Camera.main.nearClipPlane;
            return Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        }

    }
}