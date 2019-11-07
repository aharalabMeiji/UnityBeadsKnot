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

    private int BeadsNumber;
    private List<Bead> Beads; 

    public Node ANode, BNode;//ANodeID, BNodeID に依存する。
    private Vector3 V1, V2, V3, V4;

    private float rate, t1, t2;
    private int th1, th2;

    public bool inUse;

    // Use this for initialization
    void Start()
    {
        BeadsNumber = 0;
        Beads = new List<Bead>();
        inUse = true;
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log(""+ID+" "+ANodeID+" "+BNodeID);
        //両端の座標を取得
        //V1 = ANode.GetV1();
        //V2 = ANode.GetV2(ANodeRID);
        //V3 = BNode.GetV2(BNodeRID);
        //V4 = BNode.GetV1();

        //if (Master.ModifyNode)
        //{
        //    // Rの調節を行う。
        //    SetGoodRLength();
        //    SetLineRendererPosition();
        //    //SetBeadsCoordinates();
        //    //Master.ModifyNode = false;
        //    //Master.ModifyBeads = true;
        //} else if (Master.ModifyBeads)
        //{

        //}
    }

    //bool GetValid()
    //{
    //    if (0 <= ANodeID && ANodeID < Master.GetNodeLength() && 0 <= BNodeID && BNodeID < Master.GetNodeLength())
    //    {
    //        if (0 <= ANodeRID && ANodeRID < 4 && 0 <= BNodeRID && BNodeRID < 4)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //void SetLineRendererPosition()
    //{
    //    LineRenderer lrd = GetComponent<LineRenderer>();
    //    lrd.positionCount = 21;
    //    for (int i = 0; i <= 20; i++)
    //    {
    //        float t = 0.05f * i;
    //        float s = 1f - t;
    //        Vector3 v = s * s * s * V1 + 3 * s * s * t * V2 + 3 * s * t * t * V3 + t * t * t * V4;
    //        lrd.SetPosition(i, v);
    //    }
    //    lrd.loop = false;
    //}

    //// Edgeの弧長を計算する。
    //float GetArcLengthOfEdge()
    //{
    //    float ArcLength = 0f;
    //    LineRenderer lrd = GetComponent<LineRenderer>();
    //    for(int i=0; i<lrd.positionCount-1; i++)
    //    {
    //        Vector3 vec = lrd.GetPosition(i+1) - lrd.GetPosition(i);
    //        float dist = vec.magnitude;
    //        ArcLength += dist;
    //    }
    //    return ArcLength;
    //}

    //// Edgeに必要なBeadsの個数を数える
    //int GetNumberOfBeadsOfEdge()
    //{
    //    return Mathf.FloorToInt(GetArcLengthOfEdge() / CONST.BeadsDistance) - 1;
    //}

    //// Beadsの位置を計算する。
    //void SetBeadsCoordinates()
    //{
    //    int LengthOfBeads = Beads.Count;
    //    int RealBeadsNumber = GetNumberOfBeadsOfEdge();
    //    Vector3 vInf = new Vector3(9999f, 9999f, 9999f);
    //    if (BeadsNumber >= RealBeadsNumber)
    //    {
    //        //減らすときはそのままにしておいて、
    //        //BeadsNumberで処理する。
    //        BeadsNumber = RealBeadsNumber;
    //    }
    //    else if (BeadsNumber < RealBeadsNumber)
    //    {
    //        for (int i = 0; i < RealBeadsNumber - BeadsNumber; i++)
    //        {
    //            // プレハブからobjectをinstanciateする必要がある。
    //            GameObject prefab = Resources.Load<GameObject>("Prefabs/Bead");
    //            GameObject obj = Bead.Instantiate<GameObject>(prefab, vInf, Quaternion.identity);
    //            Bead bead = obj.GetComponent<Bead>();
    //            Beads.Add(bead);
    //        }
    //    }
    //    BeadsNumber = RealBeadsNumber;
    //    //個数が足りたので、座標データを入力する。
    //    int k = 0 ;
    //    float arcLength = 0f;
    //    Vector3 v0 = V1, v1=Vector3.zero;
    //    for (int i = 0; i <= 100; i++)
    //    {
    //        float t = 0.01f * i;
    //        float s = 1f - t;
    //        v1 = s * s * s * V1 + 3 * s * s * t * V2 + 3 * s * t * t * V3 + t * t * t * V4;
    //        Vector3 vv = v1 - v0;
    //        arcLength += vv.magnitude;
    //        if (arcLength >= (k+1)* CONST.BeadsDistance)
    //        {
    //            Beads[k].SetPosition(v1);
    //            k++;
    //            if (k == BeadsNumber)
    //            {
    //                for(k= BeadsNumber; k<Beads.Count; k++)
    //                {
    //                    Beads[k].SetPosition(vInf);
    //                }
    //                break;
    //            }
    //        }
    //        v0 = v1;
    //    }
    //}

    //public void SetEdgePara(int ID1, int ID2, int RID1, int RID2)
    //{
    //    ANodeID = ID1;
    //    BNodeID = ID2;
    //    ANodeRID = RID1;
    //    BNodeRID = RID2;
    //    ANode = Master.GetNodeAt(ANodeID);
    //    BNode = Master.GetNodeAt(BNodeID);
    //}

    //static float Atan2(Vector3 v1, Vector3 v2)
    //{
    //    float theta = Vector3.Angle(v1, v2);
    //    float ret;
    //    if (v1.x * v2.y - v1.y * v2.x >= 0) ret = theta; 
    //    else ret = 360f - theta;
    //    return ret;
    //}

    /// <summary>
    ///     //四本の線の長さを変えることで形を整える
    /// </summary>
    //void SetGoodRLength()
    //{
    //    if (!GetValid()) return;

    //    rate = ((V4 - V1).magnitude)/250f;// 250=計測したときのV4-V1の長さ
    //    t1 = Atan2(V2 - V1, V4 - V1);
    //    t2 = Atan2(V2 - V1, V3 - V4);
    //    th1 = Mathf.FloorToInt(t1 / 10f);
    //    th2 = Mathf.FloorToInt(t2 / 10f);
    //    //Debug.Log("" + rate + " " + t1 + " " + t2 + " " + th1 + " " + th2);
    //    if (th1 == 36)
    //    {
    //        th1 = 0;
    //        t1 = 0;
    //    }
    //    if (th2 == 36)
    //    {
    //        th2 = 0;
    //        t2 = 0;
    //    }
    //    t1 -= 10f * th1;
    //    t2 -= 10f * th2;
    //    t1 /= 10f;
    //    t2 /= 10f;
    //    //Debug.Log("" + t1 + " " + t2 );
    //    //SetGoodRLength();
    //    float a10 = (float)Constant.angle1[th1, th2];
    //    float a11 = (float)Constant.angle1[(th1 + 1) % 36, th2];
    //    float a12 = (float)Constant.angle1[th1, (th2 + 1) % 36];
    //    float a1 = a10 + t1 * (a11 - a10) + t2 * (a12 - a10);
    //    float a20 = (float)Constant.angle2[th1, th2];
    //    float a21 = (float)Constant.angle2[(th1 + 1) % 36, th2];
    //    float a22 = (float)Constant.angle2[th1, (th2 + 1) % 36];
    //    float a2 = a20 + t1 * (a21 - a20) + t2 * (a22 - a20);
    //    float a30 = (float)Constant.angleRange[th1, th2];
    //    float a31 = (float)Constant.angleRange[(th1 + 1) % 36, th2];
    //    float a32 = (float)Constant.angleRange[th1, (th2 + 1) % 36];
    //    float a3 = a30 + t1 * (a31 - a30) + t2 * (a32 - a30);

    //    ANode.R[ANodeRID] = rate * a1;
    //    BNode.R[BNodeRID] = rate * a2;
    //}


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
        if (!ANode.inUse || !BNode.inUse) return;
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
        float rate = ((v4X - v1X)* (v4X - v1X)+( v4Y - v1Y)* (v4Y - v1Y)) / 250f;// 250=計測したときのV4-V1の長さ
        float t1 = Mathf.Rad2Deg * Util.Atan2Vec(v2X - v1X, v2Y - v1Y, v4X - v1X, v4Y - v1Y);
        float t2 = Mathf.Rad2Deg * Util.Atan2Vec(v2X - v1X, v2Y - v1Y, v3X - v4X, v3Y - v4Y);

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

        float a10 = (float)Constant.len1[th1,th2];
        float a11 = (float)Constant.len1[(th1 + 1) % 36,th2];
        float a12 = (float)Constant.len1[th1,(th2 + 1) % 36];
        float a1 = a10 + t1 * (a11 - a10) + t2 * (a12 - a10);
        float a20 = (float)Constant.len2[th1,th2];
        float a21 = (float)Constant.len2[(th1 + 1) % 36,th2];
        float a22 = (float)Constant.len2[th1,(th2 + 1) % 36];
        float a2 = a20 + t1 * (a21 - a20) + t2 * (a22 - a20);
        ANode.R[ANodeRID] = rate * a1;
        BNode.R[BNodeRID] = rate * a2;
    }


}