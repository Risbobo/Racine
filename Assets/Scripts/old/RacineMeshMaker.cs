using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Racines;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class RacineMeshMaker : MonoBehaviour
{
    public Vector3[] NodePoints => GetNodePoints();

    public Vector3[] DebugPos;
    public Vector3 DebugParent;

    public Vector3 Parent => Vector3.zero;
    
    public float[] NodeWidth => GetNodeWidths();

    public int HalfCirclePoints = 10;

    private Node _node;

    // Start is called before the first frame update
    void Start()
    {
        _node = GetComponentInParent<Node>();     

        UpdateMesh();
    }
    
    private Vector3[] GetNodePoints()
    {
        var nodePoints = new List<Vector3>();
        //nodePoints.Add(_node.GetLocalTipPosition());
        nodePoints.AddRange(_node.Children.Select(child => Quaternion.Inverse(_node.transform.rotation)
                                                           * (child.GetTipPosition() - _node.GetRootPosition())));
        return nodePoints.ToArray();
    }
    
    private float[] GetNodeWidths()
    {
        var widths = new List<float>();
        widths.Add(_node.Width);
        widths.AddRange(_node.Children.Select(child => child.Width));
        return widths.ToArray();
    }

    private Vector3 GetParent()
    {
        //if (_node._parent != null)
        //{
        //    return _node._parent.transform.position;
        //}

        //return Parent;

        return _node.GetRootPosition();
    }

    // Update is called once per frame
    void Update()
    {

        UpdateMesh();
    }

    private void UpdateMesh()
    {
        int numNodes = NodePoints.Length;
        int numWidth = NodeWidth.Length;

        if (numNodes != numWidth)
        {
            Debug.LogError("Number of nodes not equal number of width provided");
            return;
        }
        if (numNodes == 3)
        {
            UpdateMeshFork(NodePoints, NodeWidth, Parent);
        }else if(numNodes == 2)
        {
            UpdateMeshLine(NodePoints, NodeWidth, Parent);
        }else if(numNodes == 1)
        {
            UpdateMeshStub(NodePoints, NodeWidth, Parent);
        }

        DebugPos = NodePoints;
        DebugParent = Parent;
    }

    private void UpdateMeshFork(Vector3[] nodePoints, float[] nodeWidth, Vector3 parentNode)
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        Vector3[] Vertices = new Vector3[7];
        Vector2[] UV = new Vector2[7];

        Vector3[] Leg = {nodePoints[1] - nodePoints[0],
                         nodePoints[2] - nodePoints[0] };

        //Vector3[] vertices = mesh.vertices;

        // pour le 0, aller chercher la position du parent
        // pour 1 et 2, calculer la position
        for (int i = 0; i < nodePoints.Length; i++)
        {

            //Orient the root node towards the parent node
            Vector3 perpend = Vector3.Cross(nodePoints[0]-parentNode, Vector3.forward).normalized;

            //orient the leg towards the root node
            if (i > 0)
            {
                perpend = Vector3.Cross(Leg[i - 1], Vector3.forward).normalized;
            }

            Vertices[2 * i] = nodePoints[i] + perpend * nodeWidth[i];
            Vertices[2 * i + 1] = nodePoints[i] - perpend * nodeWidth[i];
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

        //float U = (x0 - x3) * (y0 - y4) - (y0 - y3) * (x0 - x4);
        //U /= div;

        Vertices[6] = new Vector3(x0 + T * (x4 - x0), y0+ T * (y4 - y0), Vertices[0].z);

        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = Vertices;
        mesh.uv = UV;

        int[] Triangles = { 0, 3, 2, 0, 6, 3, 0, 1, 6, 1, 4, 6, 1, 5, 4 };

        mesh.triangles = Triangles;
    }

    private void UpdateMeshLine(Vector3[] NodePoints, float[] NodeWidth, Vector3 ParentNode)
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        Vector3[] Vertices = new Vector3[4];
        Vector2[] UV = new Vector2[4];

        Vector3[] Leg = { NodePoints[1] - NodePoints[0] };

        //Vector3[] vertices = mesh.vertices;

        // pour le 0, aller chercher la position du parent
        // pour 1 et 2, calculer la position
        for (int i = 0; i < NodePoints.Length; i++)
        {

            //Orient the root node towards the parent node
            Vector3 perpend = Vector3.Cross(NodePoints[0] - ParentNode, Vector3.forward).normalized;

            //orient the leg towards the root node
            if (i > 0)
            {
                perpend = Vector3.Cross(Leg[i - 1], Vector3.forward).normalized;
            }

            Vertices[2 * i] = NodePoints[i] + perpend * NodeWidth[i];
            Vertices[2 * i + 1] = NodePoints[i] - perpend * NodeWidth[i];
        }
        

        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = Vertices;
        mesh.uv = UV;

        int[] Triangles = { 0, 1, 2, 1, 3, 2 };

        mesh.triangles = Triangles;
    }

    private void UpdateMeshStub(Vector3[] NodePoints, float[] NodeWidth, Vector3 ParentNode)
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        Vector3[] Vertices = new Vector3[HalfCirclePoints + 1];
        Vector2[] UV = new Vector2[HalfCirclePoints + 1];
        int[] Triangles = new int[3 * (HalfCirclePoints)];

        Vertices[0] = NodePoints[0];


        Vector3 perpend = Vector3.Cross(NodePoints[0] - ParentNode, Vector3.forward).normalized;

        for (int i = 1; i <= HalfCirclePoints; i++)
        {
            Vertices[i] = NodePoints[0] +
                          Quaternion.AngleAxis((i - 1) * 180f / (HalfCirclePoints - 1), Vector3.forward) * perpend *
                          NodeWidth[0];

        }


        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = Vertices;
        mesh.uv = UV;

        for (int i = 0; i < HalfCirclePoints-1; i++)
        {
            Triangles[3 * i] = 0;
            Triangles[3 * i + 1] = 2+i;
            Triangles[3 * i + 2] = 1+i;

        }

            mesh.triangles = Triangles;
    }

}
