using UnityEngine;

namespace Racines
{
    public static class Utils
    {
        /// <summary>
        /// Gets a sample from a normal distribution based on the Marsaglia polar method
        /// https://en.wikipedia.org/wiki/Marsaglia_polar_method
        /// </summary>
        public static float RandomFromGaussion(float mean = 0f, float sigma = 1f)
        {
            float u, v, s;
            do
            {
                u = 2f * Random.Range(0f, 1f) - 1f;
                v = 2f * Random.Range(0f, 1f) - 1f;
                s = u * u + v * v;
            } while (s >= 1.0f || s == 0f);
            
            s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);
 
            return mean + sigma * u * s;
        }
        
    }
}