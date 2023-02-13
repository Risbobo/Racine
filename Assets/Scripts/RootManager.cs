using UnityEngine;

namespace Racines
{
    public class RootManager : MonoBehaviour
    {
        public float maxSproutAngle;
        public float sproutAngleSigma;
        
        public float minFanAngle;
        public float maxFanAngle;
        public int depthIncrement;
        public float initialWidth;
        
        /// The probability that a node splits into two and not one
        /// </summary>
        /// <returns></returns>
        public float splitProbability;

        /// <summary>
        /// The time is takes to grow one Node
        /// </summary>
        public float timeToGrowSprout;

        public float meanSproutLength;
        public float sproutLengthSigma;
        public bool selfCollide;
        public float directionalPullStrength;

        public float splitSurvivalRatio;

        public float RootCurveLengthPercernt;

        public int RootCurveSegments;
        public float splitPullStrengthRatio;

        public float energyGainFactor;
        public float energyDepletionFactor;

        public Material lineMaterial;
    }
}