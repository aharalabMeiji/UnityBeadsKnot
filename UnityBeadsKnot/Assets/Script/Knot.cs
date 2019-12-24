using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Knot : MonoBehaviour
{
    public GameObject Beads, Nbhds, Graph, Nodes, Edges;
    public Bead[] AllBeads;
    public Node[] AllNodes;
    public Edge[] AllEdges;

    // Start is called before the first frame update
    void Start()
    {
        // サンプルデータの直接代入
        CreateGraphFromData(Constant.NodesSample, Constant.EdgesSample);
        // 以下、たぶん無意味
        GetAllThings();
    }

    // Update is called once per frame
    void Update()
    {
        AdjustDisplay();
    }

    public Bead AddBead(Vector3 v)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Bead");
        GameObject obj = Instantiate(prefab, v, Quaternion.identity, Beads.transform);
        Bead Bd = obj.GetComponent<Bead>();
        Bd.Position = v;
        Bd.SetNU12(null, null, null, null);
        Bd.NumOfNbhd = 0;
        Bd.Joint = Bd.MidJoint = Bd.NearJoint = Bd.BandJoint = false;
        return Bd;
    }

    public Bead AddBead(Vector3 v, int id)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Bead");
        GameObject obj = Instantiate(prefab, v, Quaternion.identity, Beads.transform);
        Bead Bd = obj.GetComponent<Bead>();
        Bd.Position = v;
        Bd.SetNU12(null, null, null, null);
        Bd.NumOfNbhd = 0;
        Bd.ID = id;
        Bd.Joint = Bd.MidJoint = Bd.NearJoint = Bd.BandJoint = false;
        return Bd;
    }

    public Node AddNode(Vector3 pos, int id)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Node");
        GameObject obj = Node.Instantiate<GameObject>(prefab, Vector3.zero, Quaternion.identity, Nodes.transform);
        Node nd = obj.GetComponent<Node>();
        nd.R = new float[4];
        nd.SetPosition(pos);
        for (int i = 0; i < 4; i++)
        {
            nd.R[i] = 1f;
        }
        nd.ID = id;
        nd.inUse = true;
        return nd;
    }

    public Edge AddEdge(int idA, int idB, int ridA, int ridB,int edgeId)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Edge");
        GameObject obj = Node.Instantiate<GameObject>(prefab, Vector3.zero, Quaternion.identity, Edges.transform);
        Edge ed = obj.GetComponent<Edge>();
        ed.ANodeID = idA;
        ed.ANode = GetNodeByID(idA);
        ed.ANodeRID = ridA;
        ed.BNodeID = idB;
        ed.BNode = GetNodeByID(idB);
        ed.BNodeRID = ridB;
        ed.ID = edgeId;
        ed.inUse = true;
        ed.ParentKnot = this;
        return ed;
    }


    /// <summary>
    /// 画面からはみ出ないように位置を整える。
    /// </summary>
    void AdjustDisplay()
    {
        float l = 0, t = 0, r = 0, b = 0;
        AllBeads = FindObjectsOfType<Bead>();
        for (int i = 0; i < AllBeads.Length; i++)
        {
            Bead bd = AllBeads[i];
            if (l > bd.Position.x) l = bd.Position.x;
            if (r < bd.Position.x) r = bd.Position.x;
            if (b > bd.Position.y) b = bd.Position.y;
            if (t < bd.Position.y) t = bd.Position.y;
        }
        float dx = 0f, dy = 0f, rate = 1f;
        if (l + r > 0.1f) dx = -0.05f;
        else if (l + r < -0.1f) dx = +0.05f;
        if (b + t > 0.1f) dy = -0.05f;
        else if (b + t < -0.1f) dy = 0.05f;
        if (r - l > 9.5f) rate = 0.95f;
        if (t - b > 9.5f) rate = 0.95f;
        for (int i = 0; i < AllBeads.Length; i++)
        {
            Bead bd = AllBeads[i];
            bd.Position *= rate;
            bd.Position.x += dx;
            bd.Position.y += dy;
        }
        AllEdges = FindObjectsOfType<Edge>();
        for (int i = 0; i < AllEdges.Length; i++)
        {
            Edge ed = AllEdges[i];
            ed.AdjustLineRenderer();
        }


    }

    public void ClearAllNodes()
    {
        AllNodes = FindObjectsOfType<Node>();
        if (AllNodes.Length == 0) return;
        for (int i = AllNodes.Length - 1; i >= 0; i--)
        {
            AllNodes[i].gameObject.SetActive(false);
            Destroy(AllNodes[i].gameObject);
        }
    }

    public void ClearAllEdges()
    {
        AllEdges = FindObjectsOfType<Edge>();
        for (int i = AllEdges.Length - 1; i >= 0; i--)
        {
            AllEdges[i].gameObject.SetActive(false);
            Destroy(AllEdges[i].gameObject);
        }
    }

    public void ClearAllBeads()
    {
        AllBeads = FindObjectsOfType<Bead>();
        for (int i = AllBeads.Length - 1; i >= 0; i--)
        {
            AllBeads[i].gameObject.SetActive(false);
            Destroy(AllBeads[i].gameObject);
        }
    }

    void CreateGraphFromData(double[,] nodes, int[,] edges)
    {
        int nodesSize = nodes.Length / 7;
        ClearAllNodes();
        ClearAllBeads();
        int CountBeads = 0;
        for (int n = 0; n < nodesSize; n++)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Node");
            GameObject obj = Instantiate(prefab,
                Vector3.zero,
                Quaternion.identity,
                Nodes.transform);
            Node nd = obj.GetComponent<Node>();
            nd.ID = n;
            nd.Position = new Vector3((float)nodes[n,0] * 0.01f - 5f, -(float)nodes[n,1] * 0.01f + 5f);//補正
            nd.Theta = (float)nodes[n,2];
            nd.R = new float[4];
            nd.R[0] = (float)nodes[n,3] * 0.01f;
            nd.R[1] = (float)nodes[n,4] * 0.01f;
            nd.R[2] = (float)nodes[n,5] * 0.01f;
            nd.R[3] = (float)nodes[n,6] * 0.01f;
            nd.Joint = true;
            Bead bd = AddBead(nd.Position);
            bd.ID = CountBeads;
            nd.ThisBead = bd;
            CountBeads++;
        }
        int edgesSize = edges.Length / 4;
        AllNodes = FindObjectsOfType<Node>();
        ClearAllEdges();
        for (int n = 0; n < edgesSize; n++)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Edge");
            GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, Edges.transform);
            Edge ed = obj.GetComponent<Edge>();
            // edに対応するBeadを一つだけ作る。
            ed.ANodeID = edges[n,0];
            ed.ANodeRID = edges[n,1];
            ed.BNodeID = edges[n,2];
            ed.BNodeRID = edges[n,3];
            ed.ANode = GetNodeByID(ed.ANodeID);
            ed.BNode = GetNodeByID(ed.BNodeID);
            ed.ParentKnot = this;
            if (ed.ANode == null || ed.BNode == null) break;// たぶんこれはない。
            Bead ABead = ed.ANode.ThisBead;
            Bead BBead = ed.BNode.ThisBead;
            if (ABead == null || BBead == null) break;// たぶんこれはない。
            Bead bd = AddBead(0.5f * (ABead.Position + BBead.Position));
            bd.ID = CountBeads;
            //Debug.Log(CountBeads);
            CountBeads++;

            bd.SetNU12(ABead, null, BBead, null);
            ABead.SetNU12(ed.ANodeRID, bd);
            BBead.SetNU12(ed.BNodeRID, bd);
        }
        for (int n = 0; n < nodesSize; n++)
        {
            Node node = GetNodeByID(n);
            if (node == null) continue;
            Bead bd = node.ThisBead;
            if (bd == null) continue;
            bd.NumOfNbhd = 0;
            for (int i = 0; i < 4; i++)
            {
                if (bd.GetNU12(i) != null)
                {
                    bd.NumOfNbhd++;
                }
            }
            if (bd.NumOfNbhd == 2)
            {
                node.MidJoint = bd.MidJoint = true;
                node.BandJoint = bd.BandJoint = false;
                node.Joint = bd.Joint = false;
            }
            else if (bd.NumOfNbhd == 3)
            {
                node.MidJoint = bd.MidJoint = false;
                node.BandJoint = bd.BandJoint = true;
                node.Joint = bd.Joint = false;
            }
            else if (bd.NumOfNbhd == 4)
            {
                node.MidJoint = bd.MidJoint = false;
                node.BandJoint = bd.BandJoint = false;
                node.Joint = bd.Joint = true;
            }
            else if (bd.NumOfNbhd == 0)
            {
                Debug.Log("Num " + n + " is not in use.");
                GetNodeByID(n).inUse = false;
                bd.gameObject.SetActive(false);

            }
        }

        GetAllThings();
        //グラフの形を整える。現状ではR[]を整えるだけ。
        //Modify();
        //
        UpdateBeads();
        //  CloseJointの設定を行う（マストではない）            
        //graph.add_close_point_Joint();
        //            Draw.beads();// drawモードの変更
        AdjustEdgeLine();
    }

    /// <summary>
    /// Beads列から「結ぶ線」を構成する。
    /// ただし、EdgeのLineRendererを用いる。
    /// </summary>
    public void AdjustEdgeLine()
    {

        AllEdges = FindObjectsOfType<Edge>();
        for (int i=0; i<AllEdges.Length; i++)
        {
            Edge ed = AllEdges[i];
            ed.AdjustLineRenderer();
        }
    }

    /// <summary>
    /// AllNodes,AllEdges.AllBeads の内容をリセットする。
    /// </summary>
    public void GetAllThings()
    {
        AllNodes = this.GetComponentsInChildren<Node>();
        AllEdges = this.GetComponentsInChildren<Edge>();
        AllBeads = this.GetComponentsInChildren<Bead>();
    }

    public Bead GetBeadByID(int id)
    {
        for(int i=0; i<AllBeads.Length; i++)
        {
            if(AllBeads[i].ID == id)
            {
                return AllBeads[i];
            }
        }
        return null;
    }

    int GetBeadsNumberOnEdge(Node a, int ar, Node b, int br)
    {
        int result = 0;
        Bead prev =  a.ThisBead;
        Bead now = prev.GetNU12(ar);
        Bead next = null;
        for(int repeat=0; repeat<AllBeads.Length; repeat++)// リピートしすぎない工夫。
        {
            if (now.GetNU12(0).ID == prev.ID) next = now.GetNU12(2);
            else if (now.GetNU12(2).ID == prev.ID) next = now.GetNU12(0);
            else Debug.Log("error in GetBeadsNumberOnEdge");
            result++;
            //Debug.Log("" + next.ID + " " + b.ThisBead.ID);
            if (next.ID == b.ThisBead.ID)
            {
                return result;
            }
            else
            {
                prev = now;
                now = next;
            }
        }
        return result;
    }

    public int GetNodeIDFromBeadID(int beadId)
    {
        for(int i=0; i<AllNodes.Length; i++)
        {
            if(AllNodes[i].ThisBead.ID == beadId)
            {
                return AllNodes[i].ID;
            }
        }
        return -1;
    }

        Vector3 GetBezier(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float t)
    {
        Vector3 v12 = (1f - t) * v1 + t * v2;
        Vector3 v23 = (1f - t) * v2 + t * v3;
        Vector3 v34 = (1f - t) * v3 + t * v4;
        Vector3 v123 = (1f - t) * v12 + t * v23;
        Vector3 v234 = (1f - t) * v23 + t * v34;
        return (1f - t) * v123 + t * v234;
    }

    public Node GetNodeByID(int id)
    {
        for (int i = 0; i < AllNodes.Length; i++)
        {
            if (AllNodes[i]!=null && AllNodes[i].ID == id)
            {
                return AllNodes[i];
            }
        }
        return null;
    }

    float GetRealArclength(Node a, int ar, Node b, int br)
    {
        Vector3 v1 = a.Position;
        Vector3 v2 = a.GetCoordEdgeEnd(ar);
        Vector3 v3 = b.GetCoordEdgeEnd(br);
        Vector3 v4 = b.Position;
        float result =0f;
        Vector3 now = v1;
        Vector3 next = v1;
        for (float t=0.05f; t<1.001f; t += 0.05f)
        {
            next = GetBezier(v1, v2, v3, v4, t);
            result += (next - now).magnitude;
            now = next;
        } 
        return result;
    }

   /// <summary>
   /// グラフの形を整える。ただし、これが良いとは限らない。
   /// ビーズに戻して物理モデルで整形するという考えもある。
   /// </summary>
    public void Modify()
    {
        //Nodeのr[]を最適化する
        Debug.Log("Modify starts.");
        AllEdges = FindObjectsOfType<Edge>();
        for (int i=0; i<AllEdges.Length; i++)
        {
            Edge edge = AllEdges[i];
            edge.ScalingShapeModifier(AllNodes);
        }
        //Nodeのthetaを最適化する

        // ジョイントをドラッグするときに限り、
        //該当するジョイントの周りのthetaを最適化するようにする。
        //未実装（20190128メモ）
        
        //Nodeの(x,y)を最適化する
        //未実装
    }

    /// <summary>
    /// 旧 update_points。
    /// edgeに乗っているBeadの個数と位置を調整する。
    /// ただし、midJointのあるなしは変えない？←オプションで選べるようにしたい。
    /// </summary>
    public void UpdateBeads()
    {
        //AllNodes = FindObjectsOfType<Node>();
        //AllEdges = FindObjectsOfType<Edge>();
        //AllBeads = FindObjectsOfType<Bead>();
        for (int e = 0; e < AllEdges.Length; e++)
        {
            Edge ed = AllEdges[e];
            UpdateBeadsOnEdge(ed);
            
        }
        AllBeads = FindObjectsOfType<Bead>();

    }

    void UpdateBeadsOnEdge(Edge ed)
    {
        float beadsInterval = 0.2f;
        Node ANode = ed.ANode;// うまくいかないようだったらed.ANodeIDから計算する
        Node BNode = ed.BNode;// うまくいかないようだったらed.ANodeIDから計算する
        if (ANode == null || BNode == null)
        {
            ANode = ed.ANode = GetNodeByID(ed.ANodeID);
            BNode = ed.BNode = GetNodeByID(ed.BNodeID);
        }
        //            if (!ANode.inUse || !BNode.inUse) return;
        Bead ABead = ANode.ThisBead;
        Bead BBead = BNode.ThisBead;
        //Debug.Log("EdgeData:"+ANode.Position+"-"+BNode.Position);
        //// 理想とするエッジの弧長の概数を計算する。
        float arclength = GetRealArclength(ANode, ed.ANodeRID, BNode, ed.BNodeRID);
        //// 理想とするビーズの内個数を計算する。
        int beadsNumber = Mathf.FloorToInt(arclength / beadsInterval) - 2;
        //// 必要な内個数が少ない場合を想定して、最小数を決めておく。（多分3くらいがベスト）
        if (beadsNumber < 3) beadsNumber = 3;
        //// edgeの上にある現在のビーズの内個数を数える。
        int beadsCount = GetBeadsNumberOnEdge(ANode, ed.ANodeRID, BNode, ed.BNodeRID);
        //Debug.Log("必要数,現状数:"+beadsNumber+","+ beadsCount);
        if (beadsNumber > beadsCount)
        {// 必要数のほうが多い→ビーズの追加が必要
            int NewBeadID = AllBeads.Length;
            Bead bead1 = ABead;
            int bead1RID = ed.ANodeRID;
            Bead bead2 = ABead.GetNU12(ed.ANodeRID);// ANodeRIDに応じたビーズの番号;
            int bead2RID = bead2.GetRID(bead1);
            AllBeads = FindObjectsOfType<Bead>();
            for (int repeat = 0; repeat < beadsNumber - beadsCount; repeat++)
            {
                Bead newBead = AddBead((bead1.Position + bead2.Position) * 0.5f);// 
                newBead.SetNU12(0, bead1);
                newBead.SetNU12(2, bead2);
                newBead.NumOfNbhd = 2;
                newBead.ID = NewBeadID;
                NewBeadID++;
                bead1.SetNU12(bead1RID, newBead);
                bead2.SetNU12(bead2RID, newBead);
                bead2 = newBead;
            }
        }
        else if (beadsNumber < beadsCount)
        {//現在数のほうが多い→ビーズの削除が必要
            for (int repeat = 0; repeat < beadsCount - beadsNumber; repeat++)
            {
                Bead bd2 = ABead.GetNU12(ed.ANodeRID);
                Bead bd3 = (bd2.GetNU12(0).ID == ABead.ID) ? bd2.GetNU12(2) : bd2.GetNU12(0);
                //Debug.Log("delete beads" + ABead.ID + " " + bd2.ID + " " + bd3.ID);
                // bd2 を無効にする
                bd2.SetActive(false);
                ABead.SetNU12(ed.ANodeRID, bd3);
                if (bd3.GetNU12(0) == bd2)
                    bd3.SetNU12(0, ABead);
                else
                    bd3.SetNU12(2, ABead);
                //        bd2.n1 = bd2.n2 = -1;// 使わないもののデータを消す。
                //        de.removeBeadFromPoint(bead2);
            }
        }
        ////今一度、エッジに乗っているビーズの座標を計算しなおす。
        //Node ANode = nodes.get(ed.ANodeID);
        //Node BNode = nodes.get(ed.BNodeID);
        AllBeads = FindObjectsOfType<Bead>();
        Vector3 v1 = ANode.Position;
        Vector3 v2 = ANode.GetCoordEdgeEnd(ed.ANodeRID);
        Vector3 v3 = BNode.GetCoordEdgeEnd(ed.BNodeRID);
        Vector3 v4 = BNode.Position;
        Bead prev = ABead;
        Bead now = ABead.GetNU12(ed.ANodeRID);
        float step = arclength * 0.98f / (beadsNumber + 1);
        int bd = 0;
        float arclen = 0f;
        Vector3 pt0 = v1;
        Vector3 pt1;
        for (float repeat = 0.01f; repeat < 1.00f; repeat += 0.01f)
        {
            pt1 = GetBezier(v1, v2, v3, v4, repeat);
            arclen += (pt1 - pt0).magnitude;
            if (arclen >= step * (bd + 1))
            {
                //print("update_points():" + arclen + "," + step);
                //now.SetPosition( pt1);//作ったばかりのときは、transform.positionが優先。
                now.Position = now.transform.position = pt1;
                bd++;
                Bead next;
                if (now.GetNU12(0) == prev)
                {
                    next = now.GetNU12(2);
                }
                else
                {
                    next = now.GetNU12(0);
                }
                prev = now;
                now = next;
                if (now.ID == BBead.ID)
                {
                    break;
                }
            }
            pt0 = pt1;
        }
    }

    public void UpdateBeadsAtNode(Node nd)
    {
        //Debug.Log("UpdateBeadsAtNode");
        //AllNodes = FindObjectsOfType<Node>();
        //AllEdges = FindObjectsOfType<Edge>();
        //AllBeads = FindObjectsOfType<Bead>();
        for (int e = 0; e < AllEdges.Length; e++)
        {
            Edge ed = AllEdges[e];
            if(ed.ANodeID == nd.ID || ed.BNodeID == nd.ID)
            {
                //Debug.Log(nd.Position);
                UpdateBeadsOnEdge(ed);
            }

        }
        AllBeads = FindObjectsOfType<Bead>();
        AdjustEdgeLine();
    }

    /// <summary>
    /// id番号でBeadを探す
    /// </summary>
    public Bead FindBeadByID(int id)
    {
        //AllBeads = FindObjectsOfType<Bead>();
        for(int b=0; b < AllBeads.Length; b++)
        {
            Bead bd = AllBeads[b];
            if (bd.ID == id)
                return bd;
        }
        return null;
    }
    /// <summary>
    /// すべてのBead, Node, Edgeを消去する
    /// </summary>
    public void ClearAll()
    {//
        ClearAllBeads();
        ClearAllNodes();
        ClearAllEdges();
        GetAllThings();
    }

    public void OpenTxtFile(string path)
    {
        //ExecuteDeleteAll();
        try
        {
            using (StreamReader reader = new StreamReader(path, false))
            {
                string str;
                int phase = 0;
                int repeat = 0;
                int CountBeads = 0;
                do
                {
                    str = reader.ReadLine();
                    if (str != null)
                    {
                        if (phase == 0 && str.Contains("BeadsKnot,0"))
                        {
                            phase = 1;
                        }
                        if (phase == 1 && str.Contains("Nodes"))
                        {
                            phase = 2;
                            string[] lines = str.Split(',');
                            repeat = int.Parse(lines[1]);
                            Debug.Log(repeat);
                            ClearAllNodes();
                            AllNodes = Nodes.GetComponentsInChildren<Node>();
                            ClearAllBeads();
                            for (int n = 0; n < repeat; n++)
                            {
                                str = reader.ReadLine();
                                lines = str.Split(',');
                                float x = float.Parse(lines[0]);
                                float y = float.Parse(lines[1]);
                                float th = float.Parse(lines[2]);
                                float r0 = float.Parse(lines[3]);
                                float r1 = float.Parse(lines[4]);
                                float r2 = float.Parse(lines[5]);
                                float r3 = float.Parse(lines[6]);
                                GameObject prefab = Resources.Load<GameObject>("Prefabs/Node");
                                GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, Nodes.transform);
                                Node nd = obj.GetComponent<Node>();
                                nd.ID = n;
                                nd.Position = new Vector3(x * 0.01f - 5f, -y * 0.01f + 5f);//補正
                                nd.Theta = th;
                                nd.R = new float[4];
                                nd.R[0] = r0 * 0.01f;
                                nd.R[1] = r1 * 0.01f;
                                nd.R[2] = r2 * 0.01f;
                                nd.R[3] = r3 * 0.01f;
                                nd.Joint = true;
                                nd.inUse = true;
                                Bead bd = AddBead(nd.Position);
                                bd.ID = CountBeads;
                                nd.ThisBead = bd;
                                bd.Active = true;
                                CountBeads++;
                            }
//                          AllNodes = FindObjectsOfType<Node>();
                            AllNodes = Nodes.GetComponentsInChildren<Node>();
                            Debug.Log("AllNodes.Length = "+AllNodes.Length);
                        }
                        if (phase == 2 && str.Contains("Edges"))
                        {
                            phase = 3;
                            string[] lines = str.Split(',');
                            repeat = int.Parse(lines[1]);
                            Debug.Log(repeat);
                            ClearAllEdges();
                            for (int n = 0; n < repeat; n++)
                            {
                                str = reader.ReadLine();
                                lines = str.Split(',');
                                int aID = int.Parse(lines[0]);
                                int aRID = int.Parse(lines[1]);
                                int bID = int.Parse(lines[2]);
                                int bRID = int.Parse(lines[3]);
                                Debug.Log(aID+","+aRID+":"+bID+","+bRID);
                                GameObject prefab = Resources.Load<GameObject>("Prefabs/Edge");
                                GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, Edges.transform);
                                Edge ed = obj.GetComponent<Edge>();
                                ed.ANodeID = aID;
                                ed.ANodeRID = aRID;
                                ed.BNodeID = bID;
                                ed.BNodeRID = bRID;
                                ed.ANode = GetNodeByID(ed.ANodeID);
                                ed.BNode = GetNodeByID(ed.BNodeID);
                                ed.ParentKnot = this;
                                if (ed.ANode == null || ed.BNode == null)
                                {
                                    break;// たぶんこれはない。
                                }
                                Bead ABead = ed.ANode.ThisBead;
                                Bead BBead = ed.BNode.ThisBead;
                                if (ABead == null || BBead == null)
                                {
                                    break;// たぶんこれはない。
                                }
                                // edに対応するBeadを一つだけ作る。
                                Bead bd = AddBead(0.5f * (ABead.Position + BBead.Position));
                                bd.ID = CountBeads;
                                Debug.Log("***"+bd.Position+":"+bd.ID);
                                CountBeads++;

                                bd.SetNU12(ABead, null, BBead, null);
                                ABead.SetNU12(ed.ANodeRID, bd);
                                BBead.SetNU12(ed.BNodeRID, bd);
                            }
                            for (int n = 0; n < AllNodes.Length; n++)
                            {
                                Node node = AllNodes[n];
                                if (node == null) continue;
                                Bead bd = node.ThisBead;
                                if (bd == null) continue;
                                Debug.Log(bd.ID + "/" + AllNodes.Length+":"+bd.N1.ID+","+bd.N2.ID);
                                bd.NumOfNbhd = 0;
                                for (int i = 0; i < 4; i++)
                                {
                                    if (bd.GetNU12(i) != null)
                                    {
                                        bd.NumOfNbhd++;
                                    }
                                }
                                if (bd.NumOfNbhd == 2)
                                {
                                    node.MidJoint = bd.MidJoint = true;
                                    node.BandJoint = bd.BandJoint = false;
                                    node.Joint = bd.Joint = false;
                                }
                                else if (bd.NumOfNbhd == 3)
                                {
                                    node.MidJoint = bd.MidJoint = false;
                                    node.BandJoint = bd.BandJoint = true;
                                    node.Joint = bd.Joint = false;
                                }
                                else if (bd.NumOfNbhd == 4)
                                {
                                    node.MidJoint = bd.MidJoint = false;
                                    node.BandJoint = bd.BandJoint = false;
                                    node.Joint = bd.Joint = true;
                                }
                                else if (bd.NumOfNbhd == 0)
                                {
                                    Debug.Log("Num " + n + " is not in use.");
                                    GetNodeByID(n).inUse = false;
                                    bd.gameObject.SetActive(false);

                                }
                            }
                            GetAllThings();
                            //グラフの形を整える。現状ではR[]を整えるだけ。
                            Modify();
                            //
                            UpdateBeads();
                            //  CloseJointの設定を行う（マストではない）            
                            //graph.add_close_point_Joint();
                            //            Draw.beads();// drawモードの変更
                            AdjustEdgeLine();
                        }

                    }
                    else
                    {
                        break;
                    }
                }
                while (str != null);
                reader.Close();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Data);
            Debug.Log(e.Message);
        }
        //Debug.Log("End of ExecuteOpen");
        //WorldObject.GetComponent<World>().MenuOffButtons();
        //World.Mode = MODE.ADD_POINT;
    }


    // findOnBeads系のメソッド
    /// <summary>
    /// ビーズbから初めて、r方向へたどり、最初にMidJointかJointを見つけたら、そのビーズIDとr方向をペアにして返す。
    /// </summary>
    public PairInt FindEndOfEdgeOnBead(Bead b, int r, bool midJoint = false)
    {
        Bead Prev = b;
        Bead Now = b.GetNU12(r);
        if(Now == null)
        {
            return new PairInt(-1, -1);// 失敗
        }
        Bead Next = null;
        int MaxRepeat = AllBeads.Length;
        //以降は基本的にN1,N2しか見ない。
        for(int repeat=0; repeat<MaxRepeat; repeat++)
        {
            if(Now.N1 == Prev || Now.N1.ID == Prev.ID)//IDベースですすめる。
            {
                Next = Now.N2;
            }
            else if(Now.N2 == Prev || Now.N2.ID == Prev.ID)
            {
                Next = Now.N1;
            }
            else
            {
                Debug.Log("error in FindEndOfEdgeOnBead : 0 ");
                break;
            }
            Prev = Now;
            Now = Next;
            if(Now.Joint || (midJoint && Now.MidJoint))
            {
                if (Prev.ID == Now.N1.ID)
                    return new PairInt(Now.ID, 0);
                else if (Now.U1 != null && Prev.ID == Now.U1.ID)
                    return new PairInt(Now.ID, 1);
                else if (Prev.ID == Now.N2.ID)
                    return new PairInt(Now.ID, 2);
                else if (Now.U2 != null && Prev.ID == Now.U2.ID)
                    return new PairInt(Now.ID, 3);
                else
                {
                    Debug.Log("error in FindEndOfEdgeOnBead : 1 ");
                    break;
                }
            }
        }
        return new PairInt(-1, -1);// 失敗
    }

    /// <summary>
    /// ビーズbから初めて、r方向へたどり、最初にMidJointかJointを見つけたら、ビーズの個数を返す。
    /// </summary>
    public int CountBeadsOnEdge(Bead b, int r, bool midJoint = false)
    {
        Bead Prev = b;
        Bead Now = b.GetNU12(r);
        if (Now == null)
        {
            return 0;// 失敗
        }
        int count = 0;
        Bead Next = null;
        int MaxRepeat = AllBeads.Length;
        //以降は基本的にN1,N2しか見ない。
        for (int repeat = 0; repeat < MaxRepeat; repeat++)
        {
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
                Debug.Log("error in FindEndOfEdgeOnBead : 0 ");
                break;
            }
            Prev = Now;
            Now = Next;
            count++;
            if (Now.Joint || (midJoint && Now.MidJoint))
            {
                return count;
            }
        }
        return 0;// 失敗
    }

    /// <summary>
    /// ビーズbから初めて、r方向へたどり、ｃ個めのBeadを返す。
    /// </summary>
    public Bead GetBeadOnEdge(Bead b, int r, int c)
    {
        Bead Prev = b;
        Bead Now = b.GetNU12(r);
        if (Now == null)
        {
            return null;// 失敗
        }
        int count = c;
        Bead Next = null;
        int MaxRepeat = AllBeads.Length;
        //以降は基本的にN1,N2しか見ない。
        for (int repeat = 0; repeat < MaxRepeat; repeat++)
        {
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
                Debug.Log("error in FindEndOfEdgeOnBead : 0 ");
                break;
            }
            Prev = Now;
            Now = Next;
            count--;
            if (Now.Joint)
            {
                return null;//失敗
            }
            if (count <= 0)
            {
                return Now;
            }
        }
        return null;// 失敗
    }


}
