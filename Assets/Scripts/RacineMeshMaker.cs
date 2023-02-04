using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacineMeshMaker : MonoBehaviour
{
    Vector3[] Vertices = new Vector3[7];
    Vector2[] UV = new Vector2[7];
    int[] Triangles = new int[15];
    
    // Start is called before the first frame update
    void Start()
    {

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();

        // create an ensemble of root nodes and create the branches with width
        float[] width = { 0.4f, 0.2f, 0.1f };

        Vector3[] NodePoints = { new Vector3(0f, 0f, 0f), 
                            new Vector3(-1f, -1f, 0f), 
                            new Vector3(1.4f, -2f, 0f)
                            };

        Vector3[] Leg = {NodePoints[1] - NodePoints[0],
                         NodePoints[2] - NodePoints[0] };

        // pour le 0, aller chercher la position du parent
        // pour 1 et 2, calculer la position
        for (int i = 0; i < NodePoints.Length; i++)
        {

            Vector3 orientation = Vector3.left;

            if (i > 0)
            {
                orientation = Vector3.Cross(Leg[i - 1], Vector3.forward).normalized;
            }

            Vertices[2 * i] = NodePoints[i] + orientation * width[i];
            Vertices[2 * i+1] = NodePoints[i] - orientation * width[i];
        }
        //calcul du bas du pantalon
        float x0 = Vertices[0].x;
        float y0 = Vertices[0].y;

        float x1 = Vertices[1].x;
        float y1 = Vertices[1].y;

        float x3 = Vertices[3].x;
        float y3 = Vertices[3].y;

        float x4 = Vertices[4].x;
        float y4 = Vertices[4].y;

        float T = (x0 - x3) * (y3 - y1) - (y0 - y3) * (x3 - x1);
        float div = (x0 - x4) * (y3 - y1) - (y0 - y4) * (x3 - x1);
        T /= div;

        float U = (x0 - x3) * (y0 - y4) - (y0 - y3) * (x0 - x4);
        U /= div;

        Vertices[6] = new Vector3(x0 + T * (x4 - x0), y1 + T * (y4 - y0), 0f);

        //print les coord des points
        for (int i = 0; i < Vertices.Length; i++)
        {
            Debug.Log(Vertices[i]);
        }

        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = Vertices;

        int[] Triangles = { 0, 3, 2, 0, 6, 3, 0, 1, 6, 1, 4, 6, 1, 5, 4 };

        mesh.triangles = Triangles;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
