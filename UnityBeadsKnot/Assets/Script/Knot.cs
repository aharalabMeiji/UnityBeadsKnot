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
        CreateFromNodeEdge(Constant.NodesSample, Constant.EdgesSample);
        // 以下、たぶん無意味
        GetAllThings();
    }

    // Update is called once per frame
    void Update()
    {
        AdjustDisplay();
        ClearAllNonactiveEdges();
        ClearAllNonactiveNodes();
        ClearAllNonactiveBeads();
    }

    public Bead AddBead(Vector3 v)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Bead");
        GameObject obj = Instantiate(prefab, v, Quaternion.identity, Beads.transform);
        Bead Bd = obj.GetComponent<Bead>();
        Bd.Position = v;
        Bd.SetNU12(null, null, null, null);
        Bd.NumOfNbhd = 0;
        Bd.Joint = Bd.MidJoint  = false;
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
        Bd.Joint = Bd.MidJoint = false;
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
        nd.Active = true;
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
        ed.Active = true;
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
    /// <summary>
    /// ノードを全部クリアする。
    /// </summary>
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
    public void ClearAllNonactiveNodes()
    {
        AllNodes = FindObjectsOfType<Node>();
        if (AllNodes.Length == 0) return;
        for (int i = AllNodes.Length - 1; i >= 0; i--)
        {
            if (!AllNodes[i].Active)
            {
                AllNodes[i].gameObject.SetActive(false);
                Destroy(AllNodes[i].gameObject);
            }
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
    public void ClearAllNonactiveEdges()
    {
        AllEdges = FindObjectsOfType<Edge>();
        for (int i = AllEdges.Length - 1; i >= 0; i--)
        {
            if (!AllEdges[i].Active) { 
                AllEdges[i].gameObject.SetActive(false);
                Destroy(AllEdges[i].gameObject);
            }
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
    public void ClearAllNonactiveBeads()
    {
        AllBeads = FindObjectsOfType<Bead>();
        for (int i = AllBeads.Length - 1; i >= 0; i--)
        {
            if (!AllBeads[i].Active)
            {
                AllBeads[i].gameObject.SetActive(false);
                Destroy(AllBeads[i].gameObject);
            }
        }
    }

    void CreateFromNodeEdge(double[,] nodes, int[,] edges)
    {
        int nodesSize = nodes.Length / 7;
        ClearAllNodes();
        ClearAllBeads();
        int CountBeads = 0;
        for (int n = 0; n < nodesSize; n++)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Node");
            GameObject obj = Instantiate(prefab, Vector3.zero,  Quaternion.identity,  Nodes.transform);
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
            nd.Active = true;
            Bead bd = AddBead(nd.Position);
            bd.ID = CountBeads;
            bd.N1 = bd.N2 = bd.U1 = bd.U2 = null;
            nd.ThisBead = bd;
            nd.Active = true;
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
            if (bd.N1 != null && bd.N2 != null && bd.U1 == null && bd.U2 == null)
            {
                bd.NumOfNbhd = 2;
                node.MidJoint = bd.MidJoint = true;
                node.Joint = bd.Joint = false;
            }
            else if (bd.N1 != null && bd.N2 != null && bd.U1 != null && bd.U2 != null)
            {
                bd.NumOfNbhd = 4;
                node.MidJoint = bd.MidJoint = false;
                node.Joint = bd.Joint = true;
            }
            else if (bd.N1 == null && bd.N2 == null && bd.U1 == null && bd.U2 == null)
            {
                bd.NumOfNbhd = 0;
                Debug.Log("Node ID " + n + " is not in use.");
                node.MidJoint = bd.MidJoint = false;
                node.Joint = bd.Joint = false;
                node.Active = bd.Active = false;
            }
            else 
            {
                Debug.LogError("invalid bead");
                node.MidJoint = bd.MidJoint = false;
                node.Joint = bd.Joint = false;
                node.Active = bd.Active = false;
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
    /// <summary>
    /// ビーズのデータからノードとエッジのデータを構成する
    /// </summary>
    public void CreateNodesEdgesFromBeads()
    {
        //エッジごとにビーズの個数を数え、一定数以上であればMidJointを追加する。
        for (int i = 0; i < AllBeads.Length; i++)
        {
            Bead bd = AllBeads[i];
            if (bd.Joint)
            {
                for(int r=0; r<4; r++)
                {
                    Bead bd3 = bd.GetNU12(r);
                    if (bd3 != null) {
                        int edgeLength1 = CountBeadsOnEdge(bd, r, false);
                        int edgeLength2 = CountBeadsOnEdge(bd, r, true);
                        if (edgeLength2 < 2)// とにかく短いエッジ
                        {
                            //Debug.Log("処理が必要: edgeLength = " + edgeLength2 + "bdID = " + bd.ID + ", " + r);
                            int NewID = GetMaxIDOfBead()+1;
                            // 点を二つ追加
                            Bead bd1 = AddBead(0.7f * bd.Position + 0.3f * bd3.Position, NewID);
                            Bead bd2 = AddBead(0.3f * bd.Position + 0.7f * bd3.Position, NewID+1);
                            bd1.N1 = bd;
                            bd1.N2 = bd2;
                            bd1.Joint = bd1.MidJoint = false;
                            bd2.N1 = bd1;
                            bd2.N2 = bd3;
                            bd2.Joint = bd2.MidJoint = false;
                            if (bd.N1 == bd3) bd.N1 = bd1;
                            else if (bd.N2 == bd3) bd.N2 = bd1;
                            else if (bd.U1 == bd3) bd.U1 = bd1;
                            else if (bd.U2 == bd3) bd.U2 = bd1;
                            if (bd3.N1 == bd) bd3.N1 = bd2;
                            else if (bd3.N2 == bd) bd3.N2 = bd2;
                            else if (bd3.U1 == bd) bd3.U1 = bd2;
                            else if (bd3.U2 == bd) bd3.U2 = bd2;
                        }
                        if (edgeLength2 > 40) // とにかく長いエッジ
                        {
                            int divNumber = Mathf.CeilToInt(1f * edgeLength2 / 40f);
                            for (int b = 1; b < divNumber; b++)
                            {
                                Bead bd2 = GetBeadOnEdge(bd, r, (int)(edgeLength2 * b / divNumber));
                                bd2.MidJoint = true;// //midJoint追加
                            }
                        }
                        else if (edgeLength1 == edgeLength2)// midJointがないエッジ
                        {
                            PairInt end = FindEndOfEdgeOnBead(bd, r);
                            if (end.first == bd.ID)// midJointがないループ
                            {
                                Bead bd2 = GetBeadOnEdge(bd, r, (int)(edgeLength1 / 2));
                                bd2.MidJoint = true;// //midJoint追加
                            }
                        }
                    }
                }
            }
        }
        AllBeads = FindObjectsOfType<Bead>();
        //Nodesを一度クリアする
        ClearAllNodes();
        int nodeID = 0;
        // AllBeadsからJointだけを取り出してNodeにする
        for (int i = 0; i < AllBeads.Length; i++)
        {
            Bead bd = AllBeads[i];
            if (bd.Active && bd.Joint)
            {
                Node nd = AddNode(bd.Position, nodeID);
                nodeID++;
                nd.ThisBead = bd;
                nd.Joint = true;
                nd.MidJoint = false;
                float n1x = bd.N1.Position.x - bd.Position.x;
                float n1y = bd.N1.Position.y - bd.Position.y;
                nd.Theta = Mathf.Atan2(n1y, n1x);  //ここでできるだけ計算する
            }
        }
        // AllBeadsからMidJointだけを取り出してNodeにする。（以上で通し番号をつける）
        for (int i = 0; i < AllBeads.Length; i++)
        {
            Bead bd = AllBeads[i];
            if (bd.Active && bd.MidJoint)
            {
                Node nd = AddNode(bd.Position, nodeID);
                nodeID++;
                nd.ThisBead = bd;
                nd.Joint = false;
                nd.MidJoint = true;
                float n1x = bd.N1.Position.x - bd.Position.x;
                float n1y = bd.N1.Position.y - bd.Position.y;
                nd.Theta = Mathf.Atan2(n1y, n1x);  //ここでできるだけ計算する
            }
        }
        AllNodes = FindObjectsOfType<Node>();
        // Edgeを一度クリアする
        ClearAllEdges();
        int edgeID = 0;
        // JointとMidJointからエッジを探し出してデータ化する
        for (int i = 0; i < AllNodes.Length; i++)
        {
            Node nd = AllNodes[i];
            Bead bd = nd.ThisBead;
            if (bd.Joint || bd.MidJoint)// JointとMidJointと、すべてから一応たどってみる
            {
                for (int r = 0; r < 4; r++)
                {
                    // ビーズと方向から、たどれるJoint,MidJointのBeadIDと先方からみた方向を返す
                    PairInt br = FindEndOfEdgeOnBead(bd, r, true);
                    // BeadIDからノードのIDを割り出す
                    int nd2 = GetNodeIDFromBeadID(br.first);
                    if (br.first != -1 && nd.ID <= nd2 )// 重複をはぶく工夫
                        //if (br.first != -1 && (nd.ID < nd2 || (nd.ID == nd2 && r < br.second)))// 重複をはぶく工夫
                    {
                        /*Edge ed = */AddEdge(nd.ID, nd2, r, br.second, edgeID);
                        edgeID++;
                        //Debug.Log(nd.ID + "," + r + "," + nd2 + "," + br.second + ":" + edgeID);
                    }
                }
            }
        }
        GetAllThings();
        //グラフの形を整える。現状ではR[]を整えるだけ。
        //Modify();
        //
        //UpdateBeads();
        //  CloseJointの設定を行う（マストではない）            
        //graph.add_close_point_Joint();
        //            Draw.beads();// drawモードの変更
        //AdjustEdgeLine();
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
        if (now == null)
        {
            return -1;
        }
        if (now.ID == b.ThisBead.ID)
        {
            return 0;
        }
        Bead next;
        for(int repeat=0; repeat<AllBeads.Length; repeat++)// リピートしすぎない工夫。
        {
            if (now.N1.ID == prev.ID)
            {
                next = now.N2;
            }
            else if (now.N2.ID == prev.ID)
            {
                next = now.N1;
            }
            else
            {
                Debug.LogError("error in GetBeadsNumberOnEdge");
                return -1;
            }
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
        return -1;
    }

    public int GetNodeIDFromBeadID(int beadId)
    {
        for(int i=0; i<AllNodes.Length; i++)
        {
            if(AllNodes[i].Active && AllNodes[i].ThisBead.Active && AllNodes[i].ThisBead.ID == beadId)
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
            if (AllBeads[i].Active && AllBeads[i].ID > Max)
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
            if (AllNodes[i] != null)
            {
                if (AllNodes[i].Active && AllNodes[i].ID == id)
                {
                    return AllNodes[i];
                }
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
        Vector3 next;
        for (float t=0.05f; t<1.001f; t += 0.05f)
        {
            next = GetBezier(v1, v2, v3, v4, t);
            result += (next - now).magnitude;
            now = next;
        } 
        return result;
    }

    float GetRealArclength(Edge ed)
    {
        Node ANode = GetNodeByID(ed.ANodeID);
        Node BNode = GetNodeByID(ed.BNodeID);
        Vector3 v1 = ANode.Position;
        Vector3 v2 = ANode.GetCoordEdgeEnd(ed.ANodeRID);
        Vector3 v3 = BNode.GetCoordEdgeEnd(ed.BNodeRID);
        Vector3 v4 = BNode.Position;
        float result = 0f;
        Vector3 now = v1;
        Vector3 next;
        for (float t = 0.05f; t < 1.001f; t += 0.05f)
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
        //Debug.Log("Modify starts.");
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
        GetAllThings();
        for (int e = 0; e < AllEdges.Length; e++)
        {
            Edge ed = AllEdges[e];
            UpdateBeadsOnEdge(ed);
            
        }
        AllBeads = FindObjectsOfType<Bead>();

    }

    public void UpdateBeadsAtNode(Node nd)
    {
        for (int e = 0; e < AllEdges.Length; e++)
        {
            Edge ed = AllEdges[e];
            if (ed.ANodeID == nd.ID || ed.BNodeID == nd.ID)
            {
                UpdateBeadsOnEdge(ed);
            }
        }
        AllBeads = FindObjectsOfType<Bead>();
        AdjustEdgeLine();
    }

    void UpdateBeadsOnEdge(Edge ed)
    {
        float beadsInterval = 0.15f;
        Node ANode = ed.ANode;// うまくいかないようだったらed.ANodeIDから計算する
        Node BNode = ed.BNode;// うまくいかないようだったらed.ANodeIDから計算する
        if (ANode == null || BNode == null)
        {
            ANode = ed.ANode = GetNodeByID(ed.ANodeID);
            BNode = ed.BNode = GetNodeByID(ed.BNodeID);
        }
        //if (!ANode.Active || !BNode.Active) return;
        Bead ABead = ANode.ThisBead;
        Bead BBead = BNode.ThisBead;
        //Debug.Log("EdgeData:"+ANode.Position+"-"+BNode.Position);
        //// 理想とするエッジの弧長の概数を計算する。
        float arclength = GetRealArclength(ANode, ed.ANodeRID, BNode, ed.BNodeRID);
        //// 理想とするビーズの内個数を計算する。
        int beadsNumber = Mathf.FloorToInt(arclength / beadsInterval) - 2;
        //// 必要な内個数が少ない場合を想定して、最小数を決めておく。（多分4くらいがベスト）
        if (beadsNumber < 4) beadsNumber = 4;
        //// edgeの上にある現在のビーズの内個数を数える。
        int beadsCount = GetBeadsNumberOnEdge(ANode, ed.ANodeRID, BNode, ed.BNodeRID);
        if (beadsCount < 0) 
        {
            return;
        }
        //Debug.Log("必要数,現状数:"+beadsNumber+","+ beadsCount);
        if (beadsNumber > beadsCount)
        {// 必要数のほうが多い→ビーズの追加が必要
            int NewBeadID = GetMaxIDOfBead()+1;
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
                bead2RID = bead2.GetRID(bead1);// ここに虫がいた。（駆除済み）
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
                bd2.Active = false;
                ABead.SetNU12(ed.ANodeRID, bd3);
                if (bd3.GetNU12(0) == bd2)
                    bd3.SetNU12(0, ABead);
                else
                    bd3.SetNU12(2, ABead);
                //        bd2.n1 = bd2.n2 = -1;// 使わないもののデータを消す。
                //        de.removeBeadFromPoint(bead2);
            }
        }
        //Debug.Log("(" + ed.ANodeID + "->" + ed.BNodeID + ") " + GetNodeByID(0).ThisBead.Position);
        ////今一度、エッジに乗っているビーズの座標を計算しなおす。
        //Node ANode = nodes.get(ed.ANodeID);
        //Node BNode = nodes.get(ed.BNodeID);
        //AllBeads = FindObjectsOfType<Bead>();
        Vector3 v1 = ANode.Position;
        Vector3 v2 = ANode.GetCoordEdgeEnd(ed.ANodeRID);
        Vector3 v3 = BNode.GetCoordEdgeEnd(ed.BNodeRID);
        Vector3 v4 = BNode.Position;
        Bead prev = ABead;
        Bead now = ABead.GetNU12(ed.ANodeRID);
        string log = "";
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
                log += (""+now.ID+" ");
                if (GetNodeByID(0).ThisBead.ID == now.ID)
                {
                    Debug.Log(log+"!!!!!"+now.Position+"->"+pt1);
                }
                now.Position = now.transform.position = pt1;
                bd++;
                Bead next;
                if (now.N1 == prev)
                {
                    next = now.N2;
                }
                else
                {
                    next = now.N1;
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
        //Debug.Log("(" + ed.ANodeID + "->" + ed.BNodeID + ") " + GetNodeByID(0).ThisBead.Position);
    }

    public void UpdateNodeTheta(Node nd)
    {
        float delta = 0.03f;
        float localArcLength = 0;
        Edge[] edges= new Edge[4];
        GetAllThings();
        for(int e=0; e<AllEdges.Length; e++)
        {
            Edge ed = AllEdges[e];
            if(ed.ANodeID == nd.ID)
            {
                edges[ed.ANodeRID] = ed;
                localArcLength += GetRealArclength(ed);
            }
        }
        nd.Theta += delta;
        float localArclengthPlus = 0f;
        for(int r=0; r<4; r++)
        {
            if (edges[r] != null)
            {
                localArclengthPlus += GetRealArclength(edges[r]);
            }
        }
        if (localArcLength > localArclengthPlus)
        {
            //Debug.Log("turn left");
            return;
        }
        else nd.Theta -= delta;
        nd.Theta -= delta;
        float localArclengthMinus = 0f;
        for (int r = 0; r < 4; r++)
        {
            if (edges[r] != null)
            {
                localArclengthPlus += GetRealArclength(edges[r]);
            }
        }
        if (localArcLength > localArclengthMinus)
        {
            //Debug.Log("turn right");
            return;
        }
        else nd.Theta += delta;

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
    /// <para>カーブに沿ってたどる。</para>
    /// <para>第1成分：クロシングがなければ0、オーバークロシングのみなら1、アンダークロシングのみでゴールできれば2</para>
    /// <para>第2成分：ビーズ距離</para>
    /// </summary>
    /// <param name="startBd">出発するビーズ</param>
    /// <param name="goalBd">ゴールするビーズ</param>
    /// <returns>-1: illegal, 0: nothing, 1: yes for overcrossing, 2: yes for undercrossing</returns>
    public PairInt FindBeadAlongCurve(Bead startBd, Bead startNextBd, Bead goalBd)
    {
        Bead Prev = startBd;
        Bead Now = startNextBd;
        int overUnder = 0;// 1: Overのみ, 2:アンダーのみ
        if (Prev == null || Now == null)
        {
            return new PairInt(-1,-1);
        }
        Bead Next;
        Bead PreviousJoint = null;
        int beadCount = 0;
        int MaxRepeat = AllBeads.Length;
        //以降は基本的にN1,N2しか見ない。
        for (int repeat = 0; repeat < MaxRepeat; repeat++)
        {
            if (Now.N1 == Prev || Now.N1.ID == Prev.ID)//IDベースですすめる。
            {
                if (Now.Joint)
                {
                    if (overUnder == 0 || overUnder == 1)
                    {
                        PreviousJoint = Now;
                        overUnder = 1;
                    }
                    else if (PreviousJoint == Now && overUnder == 2)
                    {
                        PreviousJoint = null;
                        overUnder = 0;
                    }
                    else
                    {
                        return new PairInt(-1, -1);
                    }
                }
                Next = Now.N2;
            }
            else if (Now.N2 == Prev || Now.N2.ID == Prev.ID)
            {
                if (Now.Joint)
                {
                    if (overUnder == 0 || overUnder == 1)
                    {
                        PreviousJoint = Now;
                        overUnder = 1;
                    }
                    else if (PreviousJoint == Now && overUnder == 2)
                    {
                        PreviousJoint = null;
                        overUnder = 0;
                    }
                    else
                    {
                        return new PairInt(-1, -1);
                    }
                }
                Next = Now.N1;
            }
            else if (Now.U1 == Prev || (Now.U1!=null && Now.U2 != null && Now.U1.ID == Prev.ID))//IDベースですすめる。
            {
                if (Now.Joint)
                {
                    if (overUnder == 0 || overUnder == 2)
                    {
                        PreviousJoint = Now;
                        overUnder = 2;
                    }
                    else if (PreviousJoint == Now && overUnder == 1)
                    {
                        PreviousJoint = null;
                        overUnder = 0;
                    }
                    else
                    {
                        return new PairInt(-1, -1);
                    }
                }
                Next = Now.U2;
            }
            else if (Now.U2 == Prev || (Now.U1 != null && Now.U2 != null && Now.U2.ID == Prev.ID))
            {
                if (Now.Joint)
                {
                    if (overUnder == 0 || overUnder == 2)
                    {
                        PreviousJoint = Now;
                        overUnder = 2;
                    }
                    else if (PreviousJoint == Now && overUnder == 1)
                    {
                        PreviousJoint = null;
                        overUnder = 0;
                    }
                    else
                    {
                        return new PairInt(-1, -1);
                    }
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
            beadCount++;
            if (Now == goalBd || Now.ID==goalBd.ID)
            {
                return new PairInt(overUnder,beadCount);
            }
        }
        return new PairInt(-1,-1);// 失敗
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
                            //Debug.Log(repeat);
                            ClearAllNodes();
                            //AllNodes = Nodes.GetComponentsInChildren<Node>();
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
                                nd.Joint = nd.MidJoint = true;// 不確定
                                nd.Active = true;
                                Bead bd = AddBead(nd.Position);
                                nd.ThisBead = bd;
                                bd.ID = CountBeads;
                                bd.N1 = bd.N2 = bd.U1 = bd.U2 = null;
                                bd.Joint = bd.MidJoint = true;// 不確定                                
                                bd.Active = true;
                                CountBeads++;
                            }
                            AllNodes = Nodes.GetComponentsInChildren<Node>();
                            Debug.Log("AllNodes.Length = " + AllNodes.Length);
                        }
                        if (phase == 2 && str.Contains("Edges"))
                        {
                            phase = 3;
                            string[] lines = str.Split(',');
                            repeat = int.Parse(lines[1]);
                            ClearAllEdges();
                            for (int n = 0; n < repeat; n++)
                            {
                                str = reader.ReadLine();
                                lines = str.Split(',');
                                int aID = int.Parse(lines[0]);
                                int aRID = int.Parse(lines[1]);
                                int bID = int.Parse(lines[2]);
                                int bRID = int.Parse(lines[3]);
                                //Debug.Log(aID + "," + aRID + ":" + bID + "," + bRID);
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
                                //Debug.Log("bead on edge" + bd.Position + ":" + bd.ID);
                                CountBeads++;

                                bd.SetNU12(ABead, null, BBead, null);
                                ABead.SetNU12(ed.ANodeRID, bd);
                                BBead.SetNU12(ed.BNodeRID, bd);
                            }
                            Debug.Log("AllEdges.Length = " + AllEdges.Length);
                            for (int n = 0; n < AllNodes.Length; n++)
                            {
                                Node node = AllNodes[n];
                                if (node == null) continue;
                                Bead bd = node.ThisBead;
                                if (bd == null) continue;
                                bd.NumOfNbhd = 0;
                                if (bd.N1 != null && bd.N2 != null && bd.U1 == null && bd.U2 == null)
                                {
                                    bd.NumOfNbhd = 2;
                                    //Debug.Log(bd.ID + ":" + bd.N1.ID + "," + bd.N2.ID);
                                    node.MidJoint = bd.MidJoint = true;
                                    node.Joint = bd.Joint = false;
                                }
                                else if (bd.N1 != null && bd.N2 != null && bd.U1 != null && bd.U2 != null)
                                {
                                    bd.NumOfNbhd = 4;
                                    //Debug.Log(bd.ID + ":" + bd.N1.ID + "," + bd.N2.ID + "," + bd.U1.ID + "," + bd.U2.ID);
                                    node.MidJoint = bd.MidJoint = false;
                                    node.Joint = bd.Joint = true;
                                }
                                else if (bd.N1 == null && bd.N2 == null && bd.U1 == null && bd.U2 == null)
                                {
                                    bd.NumOfNbhd = 0;
                                    node.MidJoint = bd.MidJoint = false;
                                    node.Joint = bd.Joint = false;
                                    node.Active = bd.Active = false;
                                    Debug.Log("Num " + n + " is not in use.");
                                }
                                else
                                {
                                    node.MidJoint = bd.MidJoint = false;
                                    node.Joint = bd.Joint = false;
                                    node.Active = bd.Active = false;
                                    Debug.LogError("OpenTxtFile: error");
                                }
                            }
                            GetAllThings();
                            // BeadsからNodeEdgeを更新する
                            //CreateNodesEdgesFromBeads();
                            //
                            Modify();
                            UpdateBeads();
                            //グラフの形を整える。現状ではR[]を整えるだけ。
                            //
                            Modify();
                            //  CloseJointの設定を行う（マストではない）            
                            //graph.add_close_point_Joint();
                            //            Draw.beads();// drawモードの変更
                            AdjustEdgeLine();
                            Debug.Log("OpenTxtFile completes.");
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
            Debug.LogError(e.Data);
            Debug.LogError(e.Message);
        }
        //Debug.Log("End of ExecuteOpen");
        //WorldObject.GetComponent<World>().MenuOffButtons();
        //World.Mode = MODE.ADD_POINT;
    }

    public void Rotation(float angle)
    {
        if (Mathf.Abs(angle) < 0.1f)
        {
            GetAllThings();
            for (int i = 0; i < AllBeads.Length; i++)
            {
                Bead bd = AllBeads[i];
                float x = bd.Position.x;
                float y = bd.Position.y;
                bd.Position.x = Mathf.Cos(angle) * x - Mathf.Sin(angle) * y;
                bd.Position.y = Mathf.Sin(angle) * x + Mathf.Cos(angle) * y;
            }
            for (int i = 0; i < AllNodes.Length; i++)
            {
                Node nd = AllNodes[i];
                float x = nd.Position.x;
                float y = nd.Position.y;
                nd.Position.x = Mathf.Cos(angle) * x - Mathf.Sin(angle) * y;
                nd.Position.y = Mathf.Sin(angle) * x + Mathf.Cos(angle) * y;
            }
        }
    }


    public void SaveLogFile()
    {
        try
        {
            GetAllThings();
            DateTime dt = DateTime.Now;
            string FileName = dt.Month + "-" + dt.Day + "-" + dt.Hour + "-" + dt.Minute + ".txt";
            using (StreamWriter streamWriter = new StreamWriter(FileName, append: true))
            {
                streamWriter.WriteLine("BeadsKnot,0");
                int NumberOfNodes = AllNodes.Length;
                streamWriter.WriteLine("Nodes," + NumberOfNodes);
                for (int i = 0; i < NumberOfNodes; i++)
                {
                    Node nd = AllNodes[i];
                    streamWriter.WriteLine(""
                      + (100f * nd.Position.x + 500f) + ","
                        + (-100f * nd.Position.y + 500f) + ","
                        + nd.Theta + ","
                        + (100f * nd.R[0]) + ","
                        + (100f * nd.R[1]) + ","
                        + (100f * nd.R[2]) + ","
                        + (100f * nd.R[3])
                        );
                }
                int NumberOfEdges = AllEdges.Length;
                streamWriter.WriteLine("Edges," + NumberOfEdges);
                for (int i = 0; i < NumberOfEdges; i++)
                {
                    Edge ed = AllEdges[i];
                    streamWriter.WriteLine(""
                        + ed.ANodeID + ","
                        + ed.ANodeRID + ","
                        + ed.BNodeID + ","
                        + ed.BNodeRID
                        );
                }
                streamWriter.WriteLine("Region,0");
                streamWriter.WriteLine("BeadsKnotEnd");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Data);
            Debug.LogError(e.Message);
        }
    }

    public void SaveTxtFile(string filePath)
    {
        try
        {
            GetAllThings();
            using (StreamWriter streamWriter = new StreamWriter(filePath, append: false))
            {
                streamWriter.WriteLine("BeadsKnot,0");
                int NumberOfNodes = AllNodes.Length;
                streamWriter.WriteLine("Nodes," + NumberOfNodes);
                for (int i = 0; i < NumberOfNodes; i++)
                {
                    Node nd = AllNodes[i];
                    streamWriter.WriteLine(""
                      + (100f * nd.Position.x + 500f) + ","
                        + (-100f * nd.Position.y + 500f) + ","
                        + nd.Theta + ","
                        + (100f * nd.R[0]) + ","
                        + (100f * nd.R[1]) + ","
                        + (100f * nd.R[2]) + ","
                        + (100f * nd.R[3])
                        );
                }
                int NumberOfEdges = AllEdges.Length;
                streamWriter.WriteLine("Edges," + NumberOfEdges);
                for (int i = 0; i < NumberOfEdges; i++)
                {
                    Edge ed = AllEdges[i];
                    streamWriter.WriteLine(""
                        + ed.ANodeID + ","
                        + ed.ANodeRID + ","
                        + ed.BNodeID + ","
                        + ed.BNodeRID
                        );
                }
                streamWriter.WriteLine("Region,0");
                streamWriter.WriteLine("BeadsKnotEnd");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Data);
            Debug.LogError(e.Message);
        }
    }

    public void Scale(float a)
    {
        if (Mathf.Abs(a) > 0.9f)
        {
            GetAllThings();
            for (int i=0; i<AllBeads.Length; i++)
            {
                Bead bd = AllBeads[i];
                bd.Position = a * bd.Position;
            }
            for(int i=0; i<AllNodes.Length; i++)
            {
                Node nd = AllNodes[i];
                nd.Position = a * nd.Position;
            }
        }
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
        // NowがすでにJointの場合
        if (Now.Joint || (midJoint && Now.MidJoint))
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
                return new PairInt(-1, -1);// 失敗
            }
        }
        Bead Next;
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
                return new PairInt(-1, -1);// 失敗
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
                    return new PairInt(-1, -1);// 失敗
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
            return -1;// 失敗
        }
        if (Now.Joint || (midJoint && Now.MidJoint))
        {
            return 0;
        }
        int count = 0;
        Bead Next;
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
                Debug.LogError("error in CountBeadsOnEdge : " + Prev.ID+","+Now.ID);
                SaveLogFile();
                return -1;
            }
            Prev = Now;
            Now = Next;
            count++;
            if (Now.Joint || (midJoint && Now.MidJoint))
            {
                return count;
            }
        }
        return -1;// 失敗
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
        Bead Next;
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
        Bead Next;
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
                    if (Now.MidJoint)
                    {
                        int ndID = this.GetNodeIDFromBeadID(Now.ID);
                        Debug.LogWarning("Id "+ndID+" will be deleted");
                        Node nd = GetNodeByID(ndID);
                        if (nd != null)
                        {
                            nd.MidJoint = false;
                            nd.Active = false;
                        }
                        else
                        {
                            Debug.LogError("DeleteBeadsFromTo error : no Node for the bead");
                        }
                    }
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
                    if (Now.MidJoint)
                    {
                        int ndID = this.GetNodeIDFromBeadID(Now.ID);
                        Debug.LogWarning("Node ID " + ndID + " will be deleted");
                        Node nd = GetNodeByID(ndID);
                        if (nd != null)
                        {
                            nd.MidJoint = false;
                            nd.Active = false;
                        }
                        else
                        {
                            Debug.LogError("DeleteBeadsFromTo error : no Node for the bead");
                        }
                    }
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
    /// <param name="overUnder">1: 新しい経路がオーバー 2:新しい経路がアンダー</param>
    public void FreeCurve2Bead(Bead startBead, Bead endBead, int overUnder)
    {
        //int startID = startBead.ID;
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
                //int difference = b2 - b1;
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
                        bool AddMeetOK = true;
                        for (int mt = 0; mt < meets.Count; mt++)
                        {
                            int m1 = meets[mt].first;
                            int m2 = meets[mt].second;
                            if (m1 == Bd1.ID || m1 == Bd1N1.ID || m1 == Bd1N2.ID || m1 == Bd2.ID || m1 == Bd2N1.ID || m1 == Bd2N2.ID
                             || m2 == Bd1.ID || m2 == Bd1N1.ID || m2 == Bd1N2.ID || m2 == Bd2.ID || m2 == Bd2N1.ID || m2 == Bd2N2.ID)
                            {// 
                                AddMeetOK = false;
                                break;
                            }
                        }
                        if (AddMeetOK)
                        {
                            meets.Add(new PairInt(b1, b2));
                            Debug.Log("meets:(" + b1 + "," + b2+")");

                        }
                    }
                }
            }
        }
        for(int i=0; i<meets.Count; i++)
        {
            int b1 = meets[i].first;
            int b2 = meets[i].second;
            Bead Bd1 = GetBeadByID(b1);
            Bead Bd2 = GetBeadByID(b2);
            Bead Bd1N1 = Bd1.N1;
            Bead Bd1N2 = Bd1.N2;
            Bead Bd2N1 = Bd2.N1;
            Bead Bd2N2 = Bd2.N2;
            if(Bd1 == null || Bd1N1 == null || Bd1N2 == null || Bd2 == null || Bd2N1 == null || Bd2N2 == null)
            {
                Debug.LogError("FreeCurve2Bead. Invalid bead.");
                return;
            }
            ///overならbd1を採用し、underならbd2を採用する
            if (overUnder == 1)
            {// 新しい道が上//b2 を残す
                Bd2.Joint = true;
                //向きにより決める
                float v1x = Bd1N2.Position.x - Bd1N1.Position.x;
                float v1y = Bd1N2.Position.y - Bd1N1.Position.y;
                float v2x = Bd2N2.Position.x - Bd2N1.Position.x;
                float v2y = Bd2N2.Position.y - Bd2N1.Position.y;
//
                if (v1x * v2y > v1y * v2x) { 
                    Bd2.U1 = Bd1.N2;
                    Bd2.U2 = Bd1.N1;
                }
                else
                {
                    Bd2.U1 = Bd1.N1;
                    Bd2.U2 = Bd1.N2;
                }
                // Bd2からでる本数の調整
                Bd2.NumOfNbhd = 4;
                // Bd1に隣接していたビーズの調整１
                if (Bd1N1.N1 == Bd1)
                {
                    Bd1N1.N1 = Bd2;
                }
                else if (Bd1N1.N2 == Bd1)
                {
                    Bd1N1.N2 = Bd2;
                }
                // Bd1に隣接していたビーズの調整２
                if (Bd1N2.N1 == Bd1)
                {
                    Bd1N2.N1 = Bd2;
                }
                else if (Bd1N2.N2 == Bd1)
                {
                    Bd1N2.N2 = Bd2;
                }
                //Bd1の破棄
                Bd1.Active = false;
                Bd1.N1 = Bd1.N2 = null;
                Bd1.NumOfNbhd = 0;
            }
            else
            {//新しい道が下//b1を残す
                Bd1.Joint = true;
                //向きにより決める
                float v1x = Bd1N2.Position.x - Bd1N1.Position.x;
                float v1y = Bd1N2.Position.y - Bd1N1.Position.y;
                float v2x = Bd2N2.Position.x - Bd2N1.Position.x;
                float v2y = Bd2N2.Position.y - Bd2N1.Position.y;
                if (v1x * v2y > v1y * v2x)
                {
                    Bd1.U1 = Bd2.N1;
                    Bd1.U2 = Bd2.N2;
                }
                else
                {
                    Bd1.U1 = Bd2.N2;
                    Bd1.U2 = Bd2.N1;
                }
                // Bd1からでる本数の調整
                Bd1.NumOfNbhd = 4;
                // Bd2に隣接していたビーズの調整１
                if (Bd2N1.N1 == Bd2)
                {
                    Bd2N1.N1 = Bd1;
                }
                else if (Bd2N1.N2 == Bd2)
                {
                    Bd2N1.N2 = Bd1;
                }
                // Bd2に隣接していたビーズの調整２
                if (Bd2N2.N1 == Bd2)
                {
                    Bd2N2.N1 = Bd1;
                }
                else if (Bd2N2.N2 == Bd2)
                {
                    Bd2N2.N2 = Bd1;
                }
                //Bd2の破棄
                Bd2.Active = false;
                Bd2.N1 = Bd2.N2 = null;
                Bd2.NumOfNbhd = 0;
            }
        }
        //ビーズごとの「お隣さま」を数える
        AllBeads = AllBeads = this.GetComponentsInChildren<Bead>();
        for (int b = 0; b < AllBeads.Length; b++)
        {
            Bead Bd = AllBeads[b];
            if (Bd != null)
            {
                if (Bd.N1 == null && Bd.N2 == null && Bd.U1 == null && Bd.U2 == null)
                {
                    Bd.NumOfNbhd = 0;
                    Bd.Active = false;
                }
                else if (Bd.N1 != null && Bd.N2 != null && Bd.U1 == null && Bd.U2 == null)
                {
                    Bd.NumOfNbhd = 2;
                }
                else if (Bd.N1 != null && Bd.N2 != null && Bd.U1 != null && Bd.U2 != null)
                {
                    Bd.NumOfNbhd = 4;
                }
                else 
                {// 未完成であればやめる。
                    // editモードにしてから手放す
                    Display.SetEditKnotMode();
                    return;
                }
            }
        }
        Debug.Log("Complete figure");
        //ビーズのデータからノード・エッジのデータを再構成
        GetAllThings();
        CreateNodesEdgesFromBeads();
        //エッジの形を整える
        GetAllThings();
        Modify();
        //エッジに含まれるビーズを再構成する
        GetAllThings();
        UpdateBeads();
        //ねんのため、もう一度エッジの形を整える。
        //Modify();
    }
}



