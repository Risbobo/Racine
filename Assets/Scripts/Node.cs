using UnityEngine;

namespace Racines
{
    public class Node : MonoBehaviour
    {
        public void CreateNewChild(int depth, float factor = 1f)
        {
            if (depth < 0)
            {
                return;
            }

            var child = Instantiate(gameObject, transform);
            child.transform.localPosition = new Vector3(0f, 1f, 0f);
            child.transform.localRotation = Quaternion.AngleAxis(factor * Random.Range(30f, 70f), Vector3.forward);
            child.transform.localScale = Random.Range(0.5f, 0.9f) * Vector3.one;

            var childNode = child.GetComponent<Node>();
            childNode.CreateNewChild(depth - 1);
        }
    }
}