using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    public Vector3 Position;
    public float Theta;
    public float[] R;
    public int[] EdgeID;//Edge型？

    public Bead ThisBead;// Bead型
    public bool inUse;
    public int ID;

    public bool Joint = true, MidJoint = false, BandJoint = false;

    // Use this for initialization
    void Start() {
        //R = new float[4];	
        inUse = true;
    }


    public Vector3 GetCoordEdgeEnd(int RID)
    {
        Vector3 u = new Vector3(Mathf.Cos(Theta + RID * Mathf.PI * 0.5f), Mathf.Sin(Theta + RID * Mathf.PI * 0.5f), 0f);
        return Position + R[RID] * u;
    }



    public void SetNodeCoord(float x, float y, float t)
    {
        Position.x = x;
        Position.y = y;
        Position.z = 0f;
        Theta = t;
    }

    public void SetPosition(Vector3 v)
    {
        Position = v;
    }

    void Update()
    {
        Position = ThisBead.Position;
        gameObject.transform.position = Position;
    }

}
