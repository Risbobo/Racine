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

        private void Update()
        {
            //Mouse pressed: Activate the arrow
            if (Input.GetMouseButtonDown(0))
            {
                _arrowPrefab.SetActive(true);
                Vector3 mouseWorldPosition = GetMouseWorldPosition();

                _arrowPrefab.transform.position = mouseWorldPosition;
            }

            //Mouse held: rotate the arrow with the mouse
            if (Input.GetMouseButton(0))
            {
                Vector3 mouseWorldPosition3D = GetMouseWorldPosition();
                Vector3 arrowPosition3D = _arrowPrefab.transform.position;

                Vector2 arrowPoint2D = new Vector2(arrowPosition3D.x, arrowPosition3D.y);
                Vector2 mousePoint2D = new Vector2(mouseWorldPosition3D.x, mouseWorldPosition3D.y);

                float angle = Mathf.Atan2(mousePoint2D.y - arrowPoint2D.y, mousePoint2D.x - arrowPoint2D.x) * 180 / Mathf.PI;
                _arrowPrefab.transform.rotation = Quaternion.Euler(0f, 0f, angle-90);

            }

            //Mouse released
            if (Input.GetMouseButtonUp(0))
            {
                _arrowPrefab.SetActive(false);
            }
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