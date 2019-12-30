using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bead : MonoBehaviour {
    public bool Active = true;

    public Vector3 Position;
    public int ID;

    public Bead N1, U1, N2, U2;
    public int NumOfNbhd; // Nbhdの数

    public bool Joint = false, MidJoint = false;

    // Use this for initialization
    private void Start()
    {
        Position = GetComponent<Transform>().position;
        Position.z = 0f;// 念のため
    }

    // Update is called once per frame
    void Update () {
        //GetComponent<Transform>().position = Position;
        gameObject.transform.position = Position;

        SpriteRenderer SR = GetComponent<SpriteRenderer>();
        if(Joint || MidJoint)
        {
            gameObject.transform.localScale = new Vector3(0.05f, 0.05f, 1f);
            SR.color = Color.green;
        }
        else
        {
            gameObject.transform.localScale = new Vector3(0.018f, 0.018f, 1f);
            SR.color = new Color(0.9f,0.9f,0.9f);
        }
    }

    public int GetRID(Bead bd)
    {
        if (N1 == bd) return 0;
        if (U1 == bd) return 1;
        if (N2 == bd) return 2;
        if (U2 == bd) return 3;
        return -1;
    }

    public void SetNU12(int RID, Bead bd)
    {
        if (RID == 0) N1 = bd;
        else if (RID == 1) U1 = bd;
        else if (RID == 2) N2 = bd;
        else if (RID == 3) U2 = bd;
    }

    public void SetNU12(Bead n1, Bead u1, Bead n2, Bead u2)
    {
        N1 = n1;
        U1 = u1;
        N2 = n2;
        U2 = u2;
    }
    public Bead GetNU12(int RID)
    {
        if (RID == 0) return N1;
        else if (RID == 1) return U1;
        else if (RID == 2) return N2;
        else if (RID == 3) return U2;
        return null;
    }
}
