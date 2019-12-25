using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spot : MonoBehaviour
{
    Mesh SpotMesh;
    List<Vector3> Vertices;
    List<int> Triangles;
    // Start is called before the first frame update
    void Start()
    {
        Vertices = new List<Vector3>();
        Vertices.Add(Vector3.zero);
        int divNumber = 12;
        for(int i=0; i< divNumber; i++)
        {
            Vertices.Add(new Vector3(Mathf.Cos(2f * Mathf.PI * i / divNumber), Mathf.Sin(2f * Mathf.PI * i / divNumber)));
        }
        Triangles = new List<int>();
        for (int i = 0; i < divNumber; i++)
        {
            Triangles.Add(0);
            Triangles.Add((i+1)%divNumber+1);
            Triangles.Add(i + 1);
        }
        SpotMesh = new Mesh();
        SpotMesh.SetVertices(Vertices);
        SpotMesh.SetTriangles(Triangles,0);
        GetComponent<MeshFilter>().mesh = SpotMesh;


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
