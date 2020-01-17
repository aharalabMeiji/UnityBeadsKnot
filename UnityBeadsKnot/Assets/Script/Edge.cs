using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Edge : MonoBehaviour
{

    public int ANodeID;
    public int BNodeID;
    public int ANodeRID;
    public int BNodeRID;
    public int ID;
    public bool Oriented = false;

    //private int BeadsNumber;
    //private List<Bead> Beads; 
    public Knot ParentKnot;

    public Node ANode, BNode;//ANodeID, BNodeID に依存する。
    //private Vector3 V1, V2, V3, V4;

    //private float rate, t1, t2;
    //private int th1, th2;

    public bool Active;

    // Use this for initialization
    void Start()
    {
        Active = true;
    }


    /// <summary>
    //四本の線の長さを変えることで形を整える
    // arcLength に値を代入する。
    /// </summary>
    public void ScalingShapeModifier(Node[] nodes)
    {
        //Node ANode = null, BNode = null;
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i].ID == ANodeID)
            {
                ANode = nodes[i];
            }
            if (nodes[i].ID == BNodeID)
            {
                BNode = nodes[i];
            }
        }
        if(ANode == null || BNode == null) return;
        if (!ANode.Active || !BNode.Active) return;
        float v1X = ANode.Position.x;
        float v1Y = ANode.Position.y;
        Vector3 v2 = ANode.GetCoordEdgeEnd(ANodeRID);
        float v2X = v2.x;
        float v2Y = v2.y;
        Vector3 v3 = BNode.GetCoordEdgeEnd(BNodeRID);
        float v3X = v3.x;
        float v3Y = v3.y;
        float v4X = BNode.Position.x;
        float v4Y = BNode.Position.y;
        float rate = Mathf.Sqrt((v4X - v1X)* (v4X - v1X)+( v4Y - v1Y)* (v4Y - v1Y)) / 250f;// 250=計測したときのV4-V1の長さ
        float t1 = Mathf.Rad2Deg * Util.Atan2Vec(v2X - v1X, v2Y - v1Y, v4X - v1X, v4Y - v1Y);
        float t2 = Mathf.Rad2Deg * Util.Atan2Vec(v2X - v1X, v2Y - v1Y, v3X - v4X, v3Y - v4Y);
        //Debug.Log(rate);
        int th1 = Mathf.FloorToInt(t1 / 10f);
        int th2 = Mathf.FloorToInt(t2 / 10f);
        if (th1 == 36)
        {
            th1 = 0;
            t1 -= 360f;
        }
        if (th2 == 36)
        {
            th2 = 0;
            t2 -= 360f;
        }
        // 端数の処理のため
        t1 -= 10f * th1;
        t2 -= 10f * th2;
        t1 /= 10f;
        t2 /= 10f;

        if (th1 < 0 || 36 <= th1 || th2 < 0 || 36 <= th2) Debug.LogError(th1 + "," + th2);
        float a10 = (float)Constant.len1[th1,th2];
        float a11 = (float)Constant.len1[(th1 + 1) % 36,th2];
        float a12 = (float)Constant.len1[th1,(th2 + 1) % 36];
        float a1 = a10 + t1 * (a11 - a10) + t2 * (a12 - a10);
        float a20 = (float)Constant.len2[th1,th2];
        float a21 = (float)Constant.len2[(th1 + 1) % 36,th2];
        float a22 = (float)Constant.len2[th1,(th2 + 1) % 36];
        float a2 = a20 + t1 * (a21 - a20) + t2 * (a22 - a20);

        //Debug.Log(ANode.R[ANodeRID] + "->" + rate * a1);
        //Debug.Log(BNode.R[BNodeRID] + "->" + rate * a2);

        ANode.R[ANodeRID] = rate * a1;
        BNode.R[BNodeRID] = rate * a2;

    }

    public void AdjustLineRenderer()
    {
        LineRenderer LR = gameObject.GetComponent<LineRenderer>();
        //Debug.Log("AdjustLineRenderer()");
        Node ANode = ParentKnot.GetNodeByID(ANodeID);
        Node BNode = ParentKnot.GetNodeByID(BNodeID);
        if (ANode == null || BNode == null) 
        {
            Debug.LogError("Edge ID " + ID + "(" + ANodeID + "," + ANodeRID + ")(" + BNodeID + "," + BNodeRID + ") has error in NodeID");
            ParentKnot.UnderError = true;
            return;
        }
        if (!ANode.Active || !BNode.Active)
        {
            Debug.LogError("Edge ID " + ID + "(" + ANodeID + "," + ANodeRID + ")(" + BNodeID + "," + BNodeRID + ") has error in Node activity");
            ParentKnot.UnderError = true;
            return;
        }
        Bead ABead = ANode.ThisBead;
        //Bead BBead = BNode.ThisBead;
        List<Vector3> Positions = new List<Vector3>();
        if (ANodeRID == 0 || ANodeRID == 2)
            Positions.Add(ABead.Position * ParentKnot.GlobalRate);
        Bead Prev = ABead;
        Bead Now = ABead.GetNU12(ANodeRID);
        if (Now == null)
        {
            LR.positionCount = 0;// 失敗
            Debug.LogError("Edge ID " + ID + "(" + ANodeID + "," + ANodeRID + ")(" + BNodeID + "," + BNodeRID + ") has error beads sequence");
            Debug.Log("ABead ID = "+ABead.ID+ ", ANodeRID = "+ANodeRID+":ABead.N1 = " + ABead.N1+":ABead.N2 = " + ABead.N2+":ABead.U1 = " + ABead.U1+":ABead.U2 = " + ABead.U2);
            ParentKnot.UnderError = true;
            return;
        }
        Bead Next;
        int MaxRepeat = ParentKnot.AllBeads.Length;
        //以降は基本的にN1,N2しか見ない。
        for (int repeat = 0; repeat < MaxRepeat; repeat++)
        {
            if(Now==null)
            {
                Debug.LogError("Edge ID " + ID + "(" + ANodeID + "," + ANodeRID + ")(" + BNodeID + "," + BNodeRID + ") error in AdjustLineRenderer() : prev = " + Prev.ID+":repeat = "+repeat);
                ParentKnot.UnderError = true;
                return;
            }
            if (Now.N1 == null)
            {
                Debug.LogError("Edge ID " + ID + "(" + ANodeID + "," + ANodeRID + ")(" + BNodeID + "," + BNodeRID + ") error in AdjustLineRenderer() : Now.ID =" + Now.ID );
                ParentKnot.UnderError = true;
                break;
            }
            if (Now.N1 == Prev || Now.N1.ID == Prev.ID)//IDベースですすめる。
            {
                Next = Now.N2;
            }
            else if (Now.N2 == Prev || Now.N2.ID == Prev.ID)
            {
                Next = Now.N1;
            }
            else
            {
                Debug.LogError("error in AdjustLineRenderer() : " + repeat+" "+ Now.N1.ID+" "+ Now.N2.ID);
                ParentKnot.UnderError = true;
                break;
            }
            Positions.Add(Now.Position * ParentKnot.GlobalRate);
            Prev = Now;
            Now = Next;
            if (Now.Joint || Now.MidJoint)
            {
                if (BNodeRID == 0 || BNodeRID == 2)
                {
                    Positions.Add(Now.Position * ParentKnot.GlobalRate);
                }
                break;
            }
        }
        // LineRendererにデータを流し込む。
        LR.positionCount = Positions.Count;
        for (int count = 0; count < Positions.Count; count++)
        {
            LR.SetPosition(count, Positions[count]);
        }

    }
}