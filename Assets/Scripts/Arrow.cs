using UnityEngine;

namespace Racines
{
    public class Arrow : MonoBehaviour
    {

        [SerializeField] private GameObject _arrowPrefab;

        private void Start()
        {
            _arrowPrefab.SetActive(false);
        }

        public void ActivateArrow(Vector3 position)
        {
            _arrowPrefab.SetActive(true);
            _arrowPrefab.transform.position = position;
            MutateArrow();
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
            float distance = Vector2.Distance(arrowPoint2D, mousePoint2D);
            _arrowPrefab.transform.localScale = distance/(distance+5)*3 * Vector3.one;


        }

        public void DeactivateArrow()
        {
            _arrowPrefab.transform.localScale = Vector3.one;
            _arrowPrefab.SetActive(false);
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