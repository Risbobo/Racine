using UnityEngine;

namespace Racines
{
    public class RootBuilder : MonoBehaviour
    {
        [SerializeField] private GameObject nodePrefab;

        protected void Start()
        {
            var child = Instantiate(nodePrefab, position: Vector3.zero, rotation: Quaternion.identity, parent: transform);
            Instantiate(nodePrefab, position: Vector3.up, rotation: Quaternion.AngleAxis(45f, Vector3.forward), parent: child.transform);
        }
    }
}