﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dowker //: MonoBehaviour
{
    //このような感じでドウカーコードを準備しておく．
    //int[] dowker ={6, 10, 12, 2, 4, 8};
    //int dowker[]={4,8,14,16,2,18,20,22,10,12,6};
    //int dowker[]={4, 10, -12, 14, 22, 2, 18, 20, 8, 6, 16};
    //int dowker[]={6, 8, 16, 14, 4, 18, 20, 2, 22, 12, 10};
    int[] dowker= {6, 10, 16, 18, 14, 2, 20, 4, 22, 12, 8};
    //int[] dowker = {6, 12, 16, 18, 14, 4, 20, 22, 2, 8, 10};
    //int[] dowker = {40, 24, 10, 30, 22, 52, 32, 64, 46, 12, 6 ,42, 60, 2, 8, 50, 66, 16, 62, 58, 28, 4, 54, 34, 14, 20, 68, 36, 72, 26, 70, 56, 48, 18, 44, 38};


    Knot thisKnot;
    
    //int[] dowker;
    int dowkerCount;
    List<DNode> nodes;
    List<DEdge> edges;
    int[] outer;
    int outerCount = 0;

    public Dowker()
    {
        thisKnot = MonoBehaviour.FindObjectOfType<Knot>();// Knotが一つであるという前提。
    }

    public void MakeKnotFromDowkerCode()
    {
        dowkerCount = 0;
        nodes = new List<DNode>();
        edges = new List<DEdge>();
        int len = dowker.Length;
        for (int i = 0; i < len; i++)
        {
            nodes.Add(new DNode(400 + 200 * Mathf.Cos(Mathf.PI * 2 * i / len), 400 - 200 * Mathf.Sin(Mathf.PI * 2 * i / len), 2 * i + 1, Mathf.Abs(dowker[i]), i, 0, (dowker[i] > 0)));// center
            nodes.Add(new DNode(400 + 200 * Mathf.Cos(Mathf.PI * 2 * i / len) + 30, 400 - 200 * Mathf.Sin(Mathf.PI * 2 * i / len), 2 * i + 1, Mathf.Abs(dowker[i]), i, 1, (dowker[i] > 0)));// right
            nodes.Add(new DNode(400 + 200 * Mathf.Cos(Mathf.PI * 2 * i / len), 400 - 200 * Mathf.Sin(Mathf.PI * 2 * i / len) - 30, 2 * i + 1, Mathf.Abs(dowker[i]), i, 2, (dowker[i] > 0)));// top
            nodes.Add(new DNode(400 + 200 * Mathf.Cos(Mathf.PI * 2 * i / len) - 30, 400 - 200 * Mathf.Sin(Mathf.PI * 2 * i / len), 2 * i + 1, Mathf.Abs(dowker[i]), i, 3, (dowker[i] > 0)));// left
            nodes.Add(new DNode(400 + 200 * Mathf.Cos(Mathf.PI * 2 * i / len), 400 - 200 * Mathf.Sin(Mathf.PI * 2 * i / len) + 30, 2 * i + 1, Mathf.Abs(dowker[i]), i, 4, (dowker[i] > 0)));// bottom
            edges.Add(new DEdge(5 * i, 5 * i + 1, true));
            edges.Add(new DEdge(5 * i, 5 * i + 2, true));
            edges.Add(new DEdge(5 * i, 5 * i + 3, true));
            edges.Add(new DEdge(5 * i, 5 * i + 4, true));
            edges.Add(new DEdge(5 * i + 1, 5 * i + 2, false));
            edges.Add(new DEdge(5 * i + 2, 5 * i + 3, false));
            edges.Add(new DEdge(5 * i + 3, 5 * i + 4, false));
            edges.Add(new DEdge(5 * i + 4, 5 * i + 1, false));
        }
        // a,b は１始まり
        for (int i = 0; i < len; i++)
        {
            for (int j = i + 1; j < len; j++)
            {
                DNode n1 = nodes[5 * i];
                DNode n2 = nodes[5 * j];
                if (n2.b == n1.a - 1 || n2.b == n1.a + 2 * len - 1)
                {
                    edges.Add(new DEdge(5 * i + 1, 5 * j + 4, true));
                }
                else if (n2.b == n1.a + 1)
                {
                    edges.Add(new DEdge(5 * i + 3, 5 * j + 2, true));
                }
                if (n1.b == n2.a - 1 || n1.b == n2.a + 2 * len - 1)
                {
                    edges.Add(new DEdge(5 * i + 4, 5 * j + 1, true));
                }
                else if (n1.b == n2.a + 1)
                {
                    edges.Add(new DEdge(5 * i + 2, 5 * j + 3, true));
                }
            }
        }
        outer = new int[20];
        //一度，三角形の外周で計算する．
        FindTriangle();
        DowkerModify();
        // 改めて外周を探しなおす．
        FindOuter();
        DowkerModify();
        //この点データでdata_graphを構成する．
        for (int i = 0; i < nodes.Count; i++)
        {
            //Debug.Log(nodes[i].ToString());
        }
        for (int i = 0; i < edges.Count; i++)
        {
            //Debug.Log(edges[i].ToString());
        }
        //Debug.Log("Dowker test");
        OutputData();
    }

    class DNode
    {
        public int a, b;
        public float x, y;
        public int nodeID;//奇数
        public int branchID;// 1: 奇数-1, 2:偶数-1, 3:奇数+1, 4:偶数+1
        public bool ou;// true: 奇数が上， false: 偶数が上
        public DNode(float _x, float _y, int _i, int _j, int _nID, int _bID, bool _ou)
        {
            x = _x;
            y = _y;
            a = _i;
            b = _j;
            nodeID = _nID;
            branchID = _bID;
            ou = _ou;
        }
        override public string ToString()
        {
            return "(" + x + "," + y + "),(" + a + "," + b + "),(" + nodeID + "," + branchID + ")," + ou;
        }
    }

    DNode GetDNode(int _nID, int _bID)
    {
        return nodes[_nID * 5 + _bID];
    }

    class DEdge
    {
        public int s, t;
        public bool visible;
        public DEdge(int _s, int _t, bool _v)
        {
            s = _s;
            t = _t;
            visible = _v;
        }
        public override string ToString()
        {
            return "("+ s + "," + t + ")," + visible;
        }
    }

    void FindTriangle()
    {
        int nSize = nodes.Count;
        outer = new int[nSize];
        for (int a = 0; a < nSize; a++)
        {
            outer[a] = -1;
        }
        int forSize = nSize / 5;
        for (int a = 0; a < forSize; a++)
        {
            for (int b = a + 1; b < forSize; b++)
            {
                for (int c = b + 1; c < forSize; c++)
                {
                    int eAB_a = -1, eAB_b = -1;
                    int eBC_b = -1, eBC_c = -1;
                    int eCA_c = -1, eCA_a = -1;
                    for (int e = 0; e < edges.Count; e++)
                    {
                        DEdge ee = edges[e];
                        int s = (int)(ee.s / 5);
                        int t = (int)(ee.t / 5);
                        if (s == a && t == b)
                        {
                            eAB_a = ee.s;
                            eAB_b = ee.t;
                        }
                        else if (s == b && t == a)
                        {
                            eAB_b = ee.s;
                            eAB_a = ee.t;
                        }
                        else if (s == b && t == c)
                        {
                            eBC_b = ee.s;
                            eBC_c = ee.t;
                        }
                        else if (s == c && t == b)
                        {
                            eBC_c = ee.s;
                            eBC_b = ee.t;
                        }
                        else if (s == c && t == a)
                        {
                            eCA_c = ee.s;
                            eCA_a = ee.t;
                        }
                        else if (s == a && t == c)
                        {
                            eCA_a = ee.s;
                            eCA_c = ee.t;
                        }
                    }
                    if (eAB_a != -1 && eBC_b != -1 && eCA_c != -1)
                    {
                        Debug.Log("三角形見つけた！");
                        Debug.Log(eAB_a + "," + eAB_b + "," + eBC_b + "," + eBC_c + "," + eCA_c + "," + eCA_a);
                        outer[0] = eCA_a;
                        outer[1] = eAB_a;
                        outer[2] = eAB_b;
                        outer[3] = eBC_b;
                        outer[4] = eBC_c;
                        outer[5] = eCA_c;
                        outerCount = 6;
                        return;
                    }
                }
            }
        }
    }

    int FindNext(int p, int q)
    {
        if (q % 5 == 0)
        {
            float xp = nodes[p].x - nodes[q].x;
            float yp = nodes[p].y - nodes[q].y;
            int p1 = p + 1;
            if (p1 - q >= 5)
            {
                p1 -= 4;
            }
            float xp1 = nodes[p1].x - nodes[q].x;
            float yp1 = nodes[p1].y - nodes[q].y;
            int p3 = p + 3;
            if (p3 - q >= 5)
            {
                p3 -= 4;
            }
            float xp3 = nodes[p3].x - nodes[q].x;
            float yp3 = nodes[p3].y - nodes[q].y;
            float ax = xp1 - xp;
            float ay = yp1 - yp;
            float bx = xp3 - xp;
            float by = yp3 - yp;
            float orientation = ax * by - ay * bx;
            if (orientation > 0)
            {
                return p3;
            }
            else
            {
                return p1;
            }
        }
        else
        {
            for (int e = 0; e < edges.Count; e++)
            {
                DEdge ee = edges[e];
                if (ee.visible)
                {
                    if (ee.s == q && ee.t != p)
                    {
                        return ee.t;
                    }
                    if (ee.t == q && ee.s != p)
                    {
                        return ee.s;
                    }
                }
            }
        }
        return -1;
    }

    void FindOuter()
    {
        int[] outer_sub = new int[nodes.Count];
        int outer_sub_count;
        for (int pp = 0; pp < nodes.Count; pp += 5)
        {
            for (int qq = pp + 1; qq < pp + 5; qq++)
            {
                int p = pp;
                int q = qq;
                outer_sub_count = 0;
                outer_sub[outer_sub_count] = q;
                outer_sub_count++;
                for (int repeat = 0; repeat < nodes.Count; repeat++)
                {
                    int r = FindNext(p, q);
                    if (r == pp)
                    {
                        break;
                    }
                    else
                    {
                        p = q;
                        q = r;
                        if (q % 5 != 0)
                        {
                            outer_sub[outer_sub_count] = q;
                            outer_sub_count++;
                        }
                    }
                }
                if (outer_sub_count > outerCount)
                {//  マックスの場合がいいとは限らない．
                    string log = "";
                    for (int k = 0; k < outer_sub_count; k++)
                    {
                        outer[k] = outer_sub[k];
                        log += (outer_sub[k] + " ");
                    }
                    outerCount = outer_sub_count;
                    log += ("(" + outer_sub_count + ")");
                    Debug.Log(log);
                }
            }
        }
        return;
    }

    void DowkerModify()
    {
        int len = nodes.Count;
        //外周は固定する
        for (int a = 0; a < outerCount; a++)
        {
            nodes[outer[a]].x = 400 + 300 * Mathf.Cos(Mathf.PI * 2 * a / outerCount);
            nodes[outer[a]].y = 400 - 300 * Mathf.Sin(Mathf.PI * 2 * a / outerCount);
        }
        float[] dx = new float[len];
        float[] dy = new float[len];
        for (int repeat = 0; repeat < 2000; repeat++)
        {
            for (int n = 0; n < len; n++)
            {
                dx[n] = dy[n] = 0f;
            }
            for (int n = 0; n < len; n++)
            {
                DNode nn = nodes[n];
                float zx = 0;
                float zy = 0;
                int count = 0;
                for (int e = 0; e < edges.Count; e++)
                {
                    DEdge ee = edges[e];
                    if (ee.s == n)
                    {
                        zx += nodes[ee.t].x;
                        zy += nodes[ee.t].y;
                        count++;
                    }
                    else if (ee.t == n)
                    {
                        zx += nodes[ee.s].x;
                        zy += nodes[ee.s].y;
                        count++;
                    }
                }
                zx /= count;
                zy /= count;
                float ax = zx - nn.x;
                float ay = zy - nn.y;
                ax *= 0.1f;
                ay *= 0.1f;
                dx[n] = ax;
                dy[n] = ay;
            }
            for (int n = 0; n < len; n++)
            {
                bool OK = true;
                for (int a = 0; a < outerCount; a++)
                {
                    if (n == outer[a])
                    {
                        OK = false;
                        break;
                    }
                }
                if (OK)
                {
                    DNode nn = nodes[n];
                    nn.x += dx[n];
                    nn.y += dy[n];
                }
            }
        }
    }


    void OutputData()
    {
        int nodeNumber = nodes.Count;
        thisKnot.ClearAll();
        for (int n = 0; n < nodeNumber; n++)
        {
            DNode nn = nodes[n];
            if (n % 5 == 0)
            {// ジョイント
                Node nd = thisKnot.AddNode(new Vector3((nn.x - 400f) * 0.01f, (nn.y - 400f) * 0.01f), n);// ID = n
                Bead bd = thisKnot.AddBead(new Vector3((nn.x - 400f) * 0.01f, (nn.y - 400f) * 0.01f), n);// ID = n
                if (nn.ou)
                {
                    nd.Theta = Mathf.Atan2(nodes[n + 1].y - nodes[n].y, nodes[n + 1].x - nodes[n].x);
                }
                else
                {
                    nd.Theta = Mathf.Atan2(nodes[n + 2].y - nodes[n].y, nodes[n + 1].x - nodes[n].x);
                }
                nd.R[0] = nd.R[1] = nd.R[2] = nd.R[3] = 0.1f;
                nd.ID = n;
                nd.Joint = true;
                nd.MidJoint = false;
                bd.NumOfNbhd = 4;//あとで設定
                bd.N1 = bd.N2 = null;//あとで設定
                bd.U1 = bd.U2 = null;//あとで設定
                bd.ID = n;
                bd.Joint = true;
                bd.MidJoint = false;
                nd.ThisBead = bd;
            }
            else
            {//非ジョイント
                Bead bd = thisKnot.AddBead(new Vector3((nn.x - 400f) * 0.01f, (nn.y - 400f) * 0.01f), n);// ID = n
                bd.NumOfNbhd = 2;//あとで設定
                bd.N1 = bd.N2 = null;//あとで設定
                bd.U1 = bd.U2 = null;//あとで設定
                bd.ID = n;
                bd.Joint = false;
                bd.MidJoint = false;
            }
        }
        int BeadLastID = thisKnot.GetMaxIDOfBead();
        // edges
        int A = -1, AR = -1, B = -1, BR = -1;
        for (int e = 0; e < edges.Count; e++)
        {
            DEdge ee = edges[e];
            if (ee.visible)
            {
                A = AR = B = BR = -1;
                if (ee.s % 5 != 0 && ee.t %5 != 0)
                {
                    for (int es = 0; es < edges.Count; es++)
                    {
                        DEdge ees = edges[es];
                        // ee.sからたどれるジョイントを見つける
                        if (ees.visible && ees.s % 5 == 0 && ees.t == ee.s)
                        {
                            int s0 = ees.s;
                            float aX = nodes[s0 + 2].x - nodes[s0 + 1].x;
                            float aY = nodes[s0 + 2].y - nodes[s0 + 1].y;
                            float bX = nodes[s0 + 4].x - nodes[s0 + 1].x;
                            float bY = nodes[s0 + 4].y - nodes[s0 + 1].y;
                            float orientation = aX * bY - aY * bX;
                            int t = ees.t - ees.s;
                            if (orientation > 0)
                            {
                                if (nodes[ees.s].ou)
                                {
                                    t = (t + 7) % 4;
                                }
                                else
                                {
                                    t = (t + 6) % 4;
                                }
                            }
                            else
                            {
                                if (nodes[ees.s].ou)
                                {
                                    t = (5 - t) % 4;
                                }
                                else
                                {
                                    t = (6 - t) % 4;
                                }
                            }
                            A = ees.s;// ee.sからたどれるジョイント
                            AR = t;
                        }
                        //　ee.tからたどれるジョイント
                        if (ees.visible && ees.s % 5 == 0 && ees.t == ee.t)
                        {
                            int s0 = ees.s;
                            float aX = nodes[s0 + 2].x - nodes[s0 + 1].x;
                            float aY = nodes[s0 + 2].y - nodes[s0 + 1].y;
                            float bX = nodes[s0 + 4].x - nodes[s0 + 1].x;
                            float bY = nodes[s0 + 4].y - nodes[s0 + 1].y;
                            float orientation = aX * bY - aY * bX;
                            int t = ees.t - ees.s;
                            if (orientation > 0)
                            {
                                if (nodes[ees.s].ou)
                                {
                                    t = (t + 7) % 4;
                                }
                                else
                                {
                                    t = (t + 6) % 4;
                                }
                            }
                            else
                            {
                                if (nodes[ees.s].ou)
                                {
                                    t = (5 - t) % 4;
                                }
                                else
                                {
                                    t = (6 - t) % 4;
                                }
                            }
                            B = ees.s;// ee.tからたどれるジョイント
                            BR = t;
                        }
                    }                                                            ////
                    Edge ed = thisKnot.AddEdge(A, B, AR, BR, e);// iD = e (連番に限らない。)
                    Bead bdJointA = thisKnot.GetBeadByID(A);
                    Bead bdAR = thisKnot.GetBeadByID(ee.s);
                    Bead bdBR = thisKnot.GetBeadByID(ee.t);
                    Bead bdJointB = thisKnot.GetBeadByID(B);
                    if (bdJointA == null || bdAR == null || bdBR == null || bdJointB == null)
                    {
                        continue;
                    }
                    bdJointA.SetNU12(AR, bdAR);
                    bdAR.N1 = bdJointA;
                    bdAR.N2 = bdBR;
                    bdBR.N1 = bdJointB;
                    bdBR.N2 = bdAR;
                    bdJointB.SetNU12(BR, bdBR);
                }
            }
        }
        thisKnot.GetAllThings();

        thisKnot.Modify();
        // NodeEdgeからBeadsを整える
        thisKnot.UpdateBeads();
        //グラフの形を整える。現状ではR[]を整えるだけ。
        thisKnot.Modify();
        Display.SetDrawKnotMode();// drawモードの変更
    }

    /// <summary>
    /// ドウカー表示を出力する
    /// </summary>
    public void DowkerNotation()
    {
        thisKnot.GetAllThings();
        thisKnot.MakeOrientation();
        int NodeID, NodeRID, StartNodeID, StartNodeRID;
        int JointCount = 0;
        for(int i=0; i<thisKnot.AllNodes.Length; i++)
        {
            if (thisKnot.AllNodes[i].Joint)
            {
                JointCount++;
            }
        }
        if (JointCount > 0)
        {
            Edge ed =thisKnot.AllEdges[0];//とりあえず０番から始める
            if (ed == null) return;
            StartNodeID = ed.ANodeID;
            StartNodeRID = ed.ANodeRID;
            NodeID = ed.BNodeID;
            NodeRID = ed.BNodeRID;
            int[] DowkerSet = new int[JointCount * 2];
            bool[] WhetherOver = new bool[JointCount * 2];
            int repeatCount = 0;
            Node nd = thisKnot.GetNodeByID(NodeID);
            if (nd==null) return;
            if (nd.Joint)
            {
                DowkerSet[repeatCount] = NodeID;
                WhetherOver[repeatCount] = (NodeRID == 0 || NodeRID == 2);
                repeatCount++;
            }
            switch (NodeRID)
            {
                case 0: NodeRID = 2; break;
                case 1: NodeRID = 3; break;
                case 2: NodeRID = 0; break;
                case 3: NodeRID = 1; break;
            }
            
            //Debug.Log(NodeID + "," + NodeRID + "->");
            for (int repeat=0; repeat< thisKnot.AllEdges.Length; repeat++)
            {
                bool OK = false;
                for(int e=0; e<thisKnot.AllEdges.Length; e++)
                {
                    ed = thisKnot.AllEdges[e];
                    //Debug.Log("   "+ed.ANodeID + "," + ed.ANodeRID+"||"+ ed.BNodeID + "," + ed.BNodeRID);
                    if (ed.ANodeID == NodeID && ed.ANodeRID == NodeRID)
                    {
                        NodeID = ed.BNodeID;
                        NodeRID = ed.BNodeRID;
                        Debug.Log(ed.ANodeID + "," + ed.ANodeRID+":"+ed.BNodeID+","+ed.BNodeRID);
                        nd = thisKnot.GetNodeByID(NodeID);
                        if (nd == null) return;
                        OK = true;
                        if (nd.Joint)
                        {
                            Debug.Log(NodeID+","+ repeatCount);
                            DowkerSet[repeatCount] = NodeID;
                            WhetherOver[repeatCount] = (NodeRID == 0 || NodeRID == 2);
                            repeatCount++;
                        }
                        switch (NodeRID)
                        {
                            case 0: NodeRID = 2; break;
                            case 1: NodeRID = 3; break;
                            case 2: NodeRID = 0; break;
                            case 3: NodeRID = 1; break;
                        }
                        break;
                    }
                }
                if (NodeID == StartNodeID && NodeRID == StartNodeRID)
                {
                    break;
                }
                if (!OK)
                {
                    return;
                }
            }
            if (repeatCount != JointCount * 2)
            {
                Debug.Log("This is not a knot but a link.");
                return;
            }
            string ret = "";
            for (int i=0; i<JointCount; i++)
            {
                for(int j=0; j<JointCount; j++)
                {
                    if(DowkerSet[2*i] == DowkerSet[2*j+1] && WhetherOver[2*i] != WhetherOver[2 * j + 1])
                    {
                        if (WhetherOver[2 * j + 1])
                        {
                            ret += ((-2 * j - 2) + " ");
                        }
                        else
                        {
                            ret += ((2 * j + 2) + " ");
                        }
                    }
                }
            }
            Debug.Log(ret);
        }
    }
}

    //参考文献
    //https://www2.cs.arizona.edu/~kpavlou/Tutte_Embedding.pdf
    //これはかなりわかりやすい！！
