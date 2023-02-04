using UnityEngine;

namespace Racines
{
    public class RootManager : MonoBehaviour
    {
        public float minFanAngle;
        public float maxFanAngle;
        public int depthIncrement;
        public float initialWidth;

        private static RootManager _instance;

        public static RootManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<RootManager>();
                }

                return _instance;
            }
        }
    }
}