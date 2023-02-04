using UnityEngine;

namespace Racines
{
    public class RootManager : MonoBehaviour
    {
        public float minFanAngle;
        public float maxFanAngle;
        public int depthIncrement;
        public float initialWidth;
        
        /// <summary>
        /// The probability that a node does not create children below max depth
        /// </summary>
        public float killProbability;

        /// <summary>
        /// The probability that a node splits into two and not one
        /// </summary>
        /// <returns></returns>
        public float splitProbability;

        /// <summary>
        /// The time is takes to grow one Node
        /// </summary>
        public float timeToGrowSprout;

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