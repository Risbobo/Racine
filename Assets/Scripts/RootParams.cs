using UnityEngine;

namespace Racines
{
    public class RootParams : MonoBehaviour
    {
        [SerializeField] Calyptra _calyptra;
        
        public Calyptra Calyptra => _calyptra;
    }
}