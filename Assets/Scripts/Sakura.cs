using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Sakura : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.AngleAxis(Random.Range(0f,360f), Vector3.forward);
        transform.localScale *= Random.Range(0.7f, 1.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
