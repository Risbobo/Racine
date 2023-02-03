using UnityEngine;

namespace Racines
{
    public class RootBuilder : MonoBehaviour
    {
        [SerializeField] private Node nodePrefab;

        protected void Start()
        {
            Node root = Instantiate(nodePrefab, transform);
            root.CreateNewChild(5);
        }
    }
}