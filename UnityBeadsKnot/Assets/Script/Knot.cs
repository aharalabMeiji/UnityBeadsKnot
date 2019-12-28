using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Knot : MonoBehaviour
{
    public GameObject Beads, Graph, Nodes, Edges, FreeLoop;
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
    /// <summary>
    /// ベジエカーブの座標を与える
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="v3"></param>
    /// <param name="v4"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    Vector3 GetBezier(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float t)
    {
        Vector3 v12 = (1f - t) * v1 + t * v2;
        Vector3 v23 = (1f - t) * v2 + t * v3;
        Vector3 v34 = (1f - t) * v3 + t * v4;
        Vector3 v123 = (1f - t) * v12 + t * v23;
        Vector3 v234 = (1f - t) * v23 + t * v34;
        return (1f - t) * v123 + t * v234;
    }
    /// <summary>
    /// ビーズIDの最大値を求める
    /// </summary>
    /// <returns></returns>
    int GetMaxIDOfBead()
    {
        GetAllThings();
        int Max = 0;
        for (int i = 0; i < AllBeads.Length; i++)
        {
            if (AllBeads[i].ID > Max)
            {
                Max = AllBeads[i].ID;
            }
        }
        return Max;
    }
    /// <summary>
    /// IDからビーズを求める
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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
    /// カーブに沿ってたどる。
    /// オーバークロシングのみ、またはアンダークロシングのみでゴールできれば1,2のどちらかを返す。
    /// </summary>
    /// <param name="startBd">出発するビーズ</param>
    /// <param name="goalBd">ゴールするビーズ</param>
    /// <returns>-1: illegal, 0: nothing, 1: yes for N1, 2: yes for N2</returns>
    public int FindBeadAlongCurve(Bead startBd, Bead startNextBd, Bead goalBd)
    {
        Bead Prev = startBd;
        Bead Now = startNextBd;
        int overUnder = 0;// 1: Overのみ, 2:アンダーのみ
        if (Prev == null || Now == null)
        {
            return -1;
        }
        Bead Next = null;
        int MaxRepeat = AllBeads.Length;
        //以降は基本的にN1,N2しか見ない。
        for (int repeat = 0; repeat < MaxRepeat; repeat++)
        {
            if (Now.N1 == Prev || Now.N1.ID == Prev.ID)//IDベースですすめる。
            {
                if (Now.Joint)
                {
                    if (overUnder == 0 || overUnder == 1) overUnder = 1;
                    else return -1;
                }
                Next = Now.N2;
            }
            else if (Now.N2 == Prev || Now.N2.ID == Prev.ID)
            {
                if (Now.Joint)
                {
                    if (overUnder == 0 || overUnder == 1) overUnder = 1;
                    else return -1;
                }
                Next = Now.N1;
            }
            else if (Now.U1 == Prev || (Now.U1!=null && Now.U2 != null && Now.U1.ID == Prev.ID))//IDベースですすめる。
            {
                if (Now.Joint)
                {
                    if (overUnder == 0 || overUnder == 2) overUnder = 2;
                    else return -1;
                }
                Next = Now.U2;
            }
            else if (Now.U2 == Prev || (Now.U1 != null && Now.U2 != null && Now.U2.ID == Prev.ID))
            {
                if (Now.Joint)
                {
                    if (overUnder == 0 || overUnder == 2) overUnder = 2;
                    else return -1;
                }
                Next = Now.U1;
            }
            else
            {
                Debug.Log("error in FindEndOfEdgeOnBead : 0 ");
                break;
            }
            Prev = Now;
            Now = Next;
            if (Now == goalBd || Now.ID==goalBd.ID)
            {
                return overUnder;
            }
        }
        return -1;// 失敗
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

    public void DeleteBeadsFromTo(Bead StartFreeCurveBead, Bead StartFreeCurveBeadN1, Bead EndFreeCurveBead)
    {
        Bead Prev = StartFreeCurveBead;
        Bead Now = StartFreeCurveBeadN1;
        if (Now == null)
        {
            return;// 失敗
        }
        Bead Next = null;
        Prev.NumOfNbhd = 1;
        if (Prev.N1 == StartFreeCurveBeadN1) Prev.N1 = Prev.N2;
        Prev.N2 = null;
        int MaxRepeat = AllBeads.Length;
        //以降は基本的にN1,N2しか見ない。
        //ノードに行き当たったら、ノードを開放する（農奴の開放）→保留
        for (int repeat = 0; repeat < MaxRepeat; repeat++)
        {
            if (Now.N1 == Prev || Now.N1.ID == Prev.ID)//IDベースですすめる。
            {
                Next = Now.N2;
                if (Now.Joint)
                {
                    //U1をN1へ、U2をN1へコピ
                    Now.N1 = Now.U1;
                    Now.N2 = Now.U2;
                    // U1,U2をnullにする
                    Now.U1 = Now.U2 = null;
                    // 4人様をお二人様に。
                    Now.NumOfNbhd = 2;
                    //Jointをやめる
                    Now.Joint = Now.MidJoint = false;
                }
                else
                {
                    // Nowを使用不可にする。
                    Now.Active = false;
                }
            }
            else if (Now.N2 == Prev || Now.N2.ID == Prev.ID)
            {
                Next = Now.N1;
                if (Now.Joint)
                {
                    //U1をN1へ、U2をN1へコピ
                    Now.N1 = Now.U1;
                    Now.N2 = Now.U2;
                    // U1,U2をnullにする
                    Now.U1 = Now.U2 = null;
                    // 4人様をお二人様に。
                    Now.NumOfNbhd = 2;
                    //Jointをやめる
                    Now.Joint = Now.MidJoint = false;
                    //ノードとエッジの処理は後で行う。ので、ここでは不必要
                }
                else
                {
                    // Nowを使用不可にする。
                    Now.Active = false;
                }
            }
            else if (Now.U1 == Prev || (Now.U1 != null && Now.U2 != null && Now.U1.ID == Prev.ID))//IDベースですすめる。
            {
                Next = Now.U2;
                if (Now.Joint)
                {
                    // U1,U2をnullにする
                    Now.U1 = Now.U2 = null;
                    // 4人様をお二人様に。
                    Now.NumOfNbhd = 2;
                    //Jointをやめる
                    Now.Joint = Now.MidJoint = false;
                    //ノードとエッジの処理は後で行う。ので、ここでは不必要
                }
                else
                {
                    // Nowを使用不可にする。
                    Now.Active = false;
                }
            }
            else if (Now.U2 == Prev || (Now.U1 != null && Now.U2 != null && Now.U2.ID == Prev.ID))
            {
                Next = Now.U1;
                if (Now.Joint)
                {
                    // U1,U2をnullにする
                    Now.U1 = Now.U2 = null;
                    // 4人様をお二人様に。
                    Now.NumOfNbhd = 2;
                    //Jointをやめる
                    Now.Joint = Now.MidJoint = false;
                    //ノードとエッジの処理は後で行う。ので、ここでは不必要
                }
                else
                {
                    // Nowを使用不可にする。
                    Now.Active = false;
                }
            }
            else
            {
                Debug.Log("error in FindEndOfEdgeOnBead : 0 ");
                break;
            }
            Prev = Now;
            Now = Next;
            if(Now == EndFreeCurveBead || Now.ID == EndFreeCurveBead.ID)
            {
                // NowのN2をnull にして、
                if (Now.N1 == Prev)
                {
                    Now.N1 = Now.N2;
                }
                Next.N2 = null;
                // NowのNumberOfNbhdを１にする。
                Next.NumOfNbhd = 1;
                break;
            }
        }
        // Active==false なbeadを消す？
    }


    
    /// <summary>
    /// フリーカーブでつながれた経路を新しい経路とする。
    /// </summary>
    /// <param name="startBead"></param>
    /// <param name="endBead"></param>
    public void FreeCurve2Bead(Bead startBead, Bead endBead)
    {
        // まず、traceをすべてbeadに置き換える。（両端は除く）
        //Debug.Log("FreeLoopをbeadsに変換");
        int startID = startBead.ID;
        int freeLoopStartBeadID = GetMaxIDOfBead();// 最初の番号は freeLoopStartBeadID+1 
        if (startBead == null || endBead == null)
        {
            Debug.Log("FreeCurve2Bead:error:入力の値が不正");
            return;
        }
        if (startBead.NumOfNbhd != 1 || startBead.N1==null)
        {
            //即刻辞める
            Debug.Log("FreeCurve2Bead:error:startBeadの異常");
            return;
        }
        if (endBead.NumOfNbhd != 1 || endBead.N1==null)
        {
            //即刻辞める
            Debug.Log("FreeCurve2Bead:error:endBeadの異常");
            return;
        }
        //FreeLoopをひとつひとつbeadに置き換える(最初と最後は省く)
        FreeLoop freeloop = FreeLoop.GetComponent<FreeLoop>();
        int freeCurveSize = freeloop.FreeCurve.Count;
        for (int i = 1; i < freeCurveSize-1; i++)
        {
            //ビーズを追加(freeLoopStartBeadID + i=ID番号)
            AddBead(freeloop.FreeCurve[i], freeLoopStartBeadID + i);
        }
        AllBeads = FindObjectsOfType<Bead>();
        for (int i = 1; i < freeCurveSize-1; i++)
        {
            // N1,N2, NumOfNbhdを設定
            int ID = freeLoopStartBeadID + i;
            Bead bd = GetBeadByID(ID);
            //Debug.Log(bd + "," + ID);
            if (i == 1) 
            { 
                bd.N1 = startBead;
            }
            else
            {
                bd.N1 = GetBeadByID(ID - 1);
            }
            if(i== freeCurveSize - 2)
            {
                bd.N2 = endBead;
            }
            else
            {
                bd.N2 = GetBeadByID(ID + 1);
            }
        }


        {//スタートビーズのデータを整える
            if (startBead.NumOfNbhd == 0)
            {
                startBead.N1 = GetBeadByID(freeLoopStartBeadID + 1);
                startBead.NumOfNbhd = 1;
            } else if(startBead.NumOfNbhd == 1)
            {
                startBead.N2 = GetBeadByID(freeLoopStartBeadID + 1);
                startBead.NumOfNbhd = 2;
            }
        }
        {//エンドビーズのデータを整える
            if (endBead.NumOfNbhd == 0)
            {
                endBead.N1 = GetBeadByID(freeLoopStartBeadID + freeCurveSize - 2);
                endBead.NumOfNbhd = 1;
            }
            else if (endBead.NumOfNbhd == 1)
            {
                endBead.N2 = GetBeadByID(freeLoopStartBeadID + freeCurveSize - 2);
                endBead.NumOfNbhd = 2;
            }
        }
        // freeloopを開放
        freeloop.FreeCurve.Clear();
        //そののちに、既存のビーズ列、自分自身との交差を判定し、jointを追加する。
        //ArrayList<PVector> meets = new ArrayList<PVector>();
        //重複も許して交点を検出
        List<PairInt> meets = new List<PairInt>();
        int MaxBeadMax = GetMaxIDOfBead();
        for (int b1 = 0; b1 < MaxBeadMax; b1++)
        {
            Bead Bd1 = GetBeadByID(b1);
            if (Bd1 == null) continue;
            Bead Bd1N1 = Bd1.N1;
            Bead Bd1N2 = Bd1.N2;
            if (Bd1N1 == null || Bd1N2 == null) continue;
            int startB2 = Mathf.Max(freeLoopStartBeadID + 1,b1+1);
            for (int b2 = startB2; b2 < MaxBeadMax; b2++)
            {
                Bead Bd2 = GetBeadByID(b2);
                if (Bd2 == null) continue;
                Bead Bd2N1 = Bd2.N1;
                Bead Bd2N2 = Bd2.N2;
                int difference = b2 - b1;
                if (Bd1N1 != Bd2 && Bd1N2 != Bd2 && Bd2N1 != Bd1 && Bd2N2 != Bd1)
                {// そもそも異なる場所である保証。
                    float x1 = Bd1N1.Position.x;
                    float y1 = Bd1N1.Position.y;
                    float x2 = Bd1N2.Position.x;
                    float y2 = Bd1N2.Position.y;
                    float x3 = Bd2N1.Position.x;
                    float y3 = Bd2N1.Position.y;
                    float x4 = Bd2N2.Position.x;
                    float y4 = Bd2N2.Position.y;
                    // (x2-x1)s+x1 = (x4-x3)t+x3
                    // (y2-y1)s+y1 = (y4-y3)t+y3
                    // (x2-x1)s - (x4-x3)t = +x3-x1
                    // (y2-y1)s - (y4-y3)t = +y3-y1
                    float a = x2 - x1;
                    float b = -x4 + x3;
                    float c = y2 - y1;
                    float d = -y4 + y3;
                    float p = x3 - x1;
                    float q = y3 - y1;
                    float s1 = p * d - b * q; // s = s1/st
                    float t1 = a * q - p * c; // t = t1/st
                    float st = a * d - b * c;
                    if (st < 0)
                    {
                        st *= -1;
                        s1 *= -1;
                        t1 *= -1;
                    }
                    if (0 < s1 && s1 < st && 0 < t1 && t1 < st)
                    { // 線分が交わっている条件
                      //重複を排してListに貯める
                        bool OK = true;
                        for (int mt = 0; mt < meets.Count; mt++)
                        {
                            int m1 = meets[mt].first;
                            int m2 = meets[mt].second;
                            if (m1 == Bd1.ID || m1 == Bd1N1.ID || m1 == Bd1N2.ID || m1 == Bd2.ID || m1 == Bd2N1.ID || m1 == Bd2N2.ID
                             || m2 == Bd1.ID || m2 == Bd1N1.ID || m2 == Bd1N2.ID || m2 == Bd2.ID || m2 == Bd2N1.ID || m2 == Bd2N2.ID)
                            {// 
                                OK = false;
                                break;
                            }
                        }
                        if (OK)
                        {
                            meets.Add(new PairInt(b1, b2));
                            Debug.Log("OK:(" + b1 + "," + b2+")");
                        }
                    }
                }
            }
        }
            //for (int bdID1 = traceStartBeadID; bdID1 < beadsNumber; bdID1++)
            //{
            //    Bead bd1 = data.getBead(bdID1);
            //    if (bd1.c >= 2)
            //    {
            //        for (int bdID2 = 0; bdID2 < beadsNumber; bdID2++)
            //        {
            //            Bead bd2 = data.getBead(bdID2);
            //            if (bd2 != null && bdID2 < bdID1 && bd2.c >= 2)
            //            {
            //                int bd1n1 = bd1.n1;
            //                int bd1n2 = bd1.n2;
            //                int bd2n1 = bd2.n1;
            //                int bd2n2 = bd2.n2;
            //                if (bd1n1 != -1 && bd1n2 != -1 && bd2n1 != -1 && bd2n2 != -1
            //                  && bd1n1 != bd2n1 && bd1n1 != bdID2 && bd1n1 != bd2n2
            //                  && bdID1 != bd2n1 && bdID1 != bdID2 && bdID1 != bd2n2
            //                  && bd1n2 != bd2n1 && bd1n2 != bdID2 && bd1n2 != bd2n2)
            //                {
            //                    float x1 = data.getBead(bd1n1).x;
            //                    float y1 = data.getBead(bd1n1).y;
            //                    float x2 = data.getBead(bd1n2).x;
            //                    float y2 = data.getBead(bd1n2).y;
            //                    float x3 = data.getBead(bd2n1).x;
            //                    float y3 = data.getBead(bd2n1).y;
            //                    float x4 = data.getBead(bd2n2).x;
            //                    float y4 = data.getBead(bd2n2).y;
            //                    //   (x2-x1)s - (x4-x3)t = +x3-x1 
            //                    //   (y2-y1)s - (y4-y3)t = +y3-y1
            //                    float a = x2 - x1;
            //                    float b = -x4 + x3;
            //                    float c = y2 - y1;
            //                    float d = -y4 + y3;
            //                    float p = x3 - x1;
            //                    float q = y3 - y1;
            //                    float s1 = p * d - b * q;  // s = s1/st
            //                    float t1 = a * q - p * c;  // t = t1/st
            //                    float st = a * d - b * c;
            //                    if (st < 0)
            //                    {
            //                        st *= -1;
            //                        s1 *= -1;
            //                        t1 *= -1;
            //                    }
            //                    if (0 < s1 && s1 < st && 0 < t1 && t1 < st)
            //                    {
            //                        //trace.get(tr1+1) と trace.get(tr2+1)とを合流してJointにする。
            //                        // 合流する点がJointに極めて近いときは失敗扱いにしたいが、
            //                        //そもそもtraceがJointの近くを通らないことを保証しているので、信じることにする。
            //                        //Jointの二重登録を避けるための作業。
            //                        boolean localOK = true;
            //                        for (int mt = 0; mt < meets.size(); mt++)
            //                        {
            //                            int js1 = int(meets.get(mt).x);
            //                            int js2 = int(meets.get(mt).y);
            //                            if (js1 == bd1n1 || js1 == bdID1 || js1 == bd1n2
            //                              || js1 == bd2n1 || js1 == bdID2 || js1 == bd2n2
            //                              || js2 == bd1n1 || js2 == bdID1 || js2 == bd1n2
            //                              || js2 == bd2n1 || js2 == bdID2 || js2 == bd2n2)
            //                            {
            //                                println(bdID1, bdID2, js1, js2);
            //                                localOK = false;
            //                                break;
            //                            }
            //                        }
            //                        if (localOK)
            //                        {
            //                            println(bdID1, "meets", bdID2);
            //                            meets.add(new PVector(bdID1, bdID2));
            //                            bd1 = data.getBead(bdID1);
            //                            bd2 = data.getBead(bdID2);
            //                            ///////Jointかunderかoverかで変わる
            //                            ///overならbd1を採用し、underならbd2を採用する
            //                            if (data.over_crossing)
            //                            {
            //                                bd1.c = 2;
            //                                bd1.Joint = true;
            //                                bd1.u1 = bd2n1;
            //                                bd1.u2 = bd2n2;
            //                                // bd1.c = 4;
            //                                data.removeBeadFromPoint(bdID2);
            //                                //bd2.n1 = -1;
            //                                //bd2.n2 = -1;
            //                                //bd2.x = bd2.y = -1f;
            //                                //bd2.c = -1;

            //                                Bead bd11 = data.getBead(bd2n1);
            //                                if (bd11.n1 == bdID2)
            //                                {
            //                                    bd11.n1 = bdID1;
            //                                }
            //                                else if (bd11.n2 == bdID2)
            //                                {
            //                                    bd11.n2 = bdID1;
            //                                }
            //                                Bead bd12 = data.getBead(bd2n2);
            //                                if (bd12.n1 == bdID2)
            //                                {
            //                                    bd12.n1 = bdID1;
            //                                }
            //                                else if (bd12.n2 == bdID2)
            //                                {
            //                                    bd12.n2 = bdID1;
            //                                }
            //                            }
            //                            else
            //                            {
            //                                bd2.c = 2;
            //                                bd2.Joint = true;
            //                                bd2.u1 = bd1n1;
            //                                bd2.u2 = bd1n2;
            //                                //bd2.c = 4;
            //                                data.removeBeadFromPoint(bdID1);
            //                                //bd1.n1 = -1;
            //                                //bd1.n2 = -1;
            //                                //bd1.x = bd1.y = -1f;
            //                                //bd1.c = -1;

            //                                Bead bd11 = data.getBead(bd1n1);
            //                                if (bd11.n1 == bdID1)
            //                                {
            //                                    bd11.n1 = bdID2;
            //                                }
            //                                else if (bd11.n2 == bdID1)
            //                                {
            //                                    bd11.n2 = bdID2;
            //                                }
            //                                Bead bd12 = data.getBead(bd1n2);
            //                                if (bd12.n1 == bdID1)
            //                                {
            //                                    bd12.n1 = bdID2;
            //                                }
            //                                else if (bd12.n2 == bdID1)
            //                                {
            //                                    bd12.n2 = bdID2;
            //                                }
            //                            }
            //                        }
            //                        //  }
            //                        //}
            //                        //終了条件の確認
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //boolean OK = true;//図が完了しているかどうかのフラグ。
            //for (int bdID = 0; bdID < data.points.size(); bdID++)
            //{
            //    Bead bd = data.getBead(bdID);
            //    if (bd != null)
            //    {
            //        if (bd.n1 != -1 || bd.n2 != -1 || bd.u1 != -1 || bd.u2 != -1)
            //        {
            //            if (bd.c != 2 && bd.c != 4)
            //            {
            //                OK = false;
            //                return;
            //            }
            //        }
            //    }
            //}
            //if (OK)
            //{
            //    println("complete figure");
            //    //data.points.clear();
            //    //for (int bdID=0; bdID<edit.beads.size(); bdID++) {
            //    //  Bead bd = edit.beads.get(bdID);
            //    //  if (bd.c==4) {
            //    //    bd.c=2;
            //    //    bd.Joint = true;
            //    //  } else if (bd.c==2) {
            //    //    bd.Joint = false;
            //    //  }
            //    //  data.points.add(bd);
            //    //}

            //    graph.make_data_graph();
            //    Draw.beads();
            //}// OK=falseならば、図が未完成なので、さらなるトレースを待つ。
    }


}
