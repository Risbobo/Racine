using UnityEngine;

namespace Racines
{
    public class RootBuilder : MonoBehaviour
    {
        [SerializeField] private GameObject nodePrefab;

        protected void Start()
        {
            Node root = Instantiate(nodePrefab, transform).AddComponent<Node>();
        }
    }
}