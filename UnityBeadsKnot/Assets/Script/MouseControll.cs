using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DIR
{
    public const int O1 = 0;
    public const int U1 = 1;
    public const int O2 = 2;
    public const int U2 = 3;

}

public class CONST
{
    public static float BeadsDistance = 0.3f;

}

public class Display
{
    private static int Mode=1;

    public static bool IsMenuMode()
    {
        return (Mode == 0);
    }

    public static bool IsDrawKnotMode()
    {
        return (Mode == 1);
    }

    public static bool IsFreeLoopMode()
    {
        return (Mode == 2);
    }

    public static bool IsEditKnotMode()
    {
        return (Mode == 3);
    }

    public static void SetMenuMode()
    {
        Mode = 0;
    }
    public static void SetDrawKnotMode()
    {
        Mode = 1;
    }
    public static void SetFreeLoopMode()
    {
        Mode = 2;
    }
    public static void SetEditKnotMode()
    {
        Mode = 3;
    }
}

public class PairInt
{
    public int first, second;
    public PairInt(int a, int b)
    {
        first = a;
        second = b;
    }

    public bool Contained(int x)
    {
        if(x==first || x == second)
            return true;
        return false;
    }
}

public class MouseControll : MonoBehaviour {


    public  GameObject ThisKnot;
    Knot thisKnot;
    public GameObject ThisMenu;
    Menu thisMenu;
    public GameObject StaticTools;
    public GameObject FreeLoop;

    Vector3 MouseDownVec;
    Vector3 MouseDragVec;
    Node DraggedNode = null;
    Vector3 DraggedNodeStartPosition;
    Vector3 PreviousPosition;

    public static bool ModifyNode = true;
    public static bool ModifyBeads = false;
    public static bool DisplayEdgeLineRenderer = true;


    // Use this for initialization
    void Start () {
        thisKnot = ThisKnot.GetComponent<Knot>();
        thisMenu = ThisMenu.GetComponent<Menu>();
        thisMenu.HideMenu();
    }

    void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDown();
        }
        if (Input.GetMouseButton(0))
        {
            OnMouseDrag();
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnMouseUp();
        }
        if (Input.anyKeyDown)
        {
            OnKeyDown();
        }
    }


    public void OnMouseDown()
    {
        MouseDownVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MouseDownVec.z = 0f;
        //Debug.Log(MouseDownVec);
        if (Display.IsDrawKnotMode()) { 
            thisKnot = ThisKnot.GetComponent<Knot>();
            thisKnot.GetAllThings();
        
            for(int n=0; n<thisKnot.AllNodes.Length; n++)
            {
                float dist = (MouseDownVec - thisKnot.AllNodes[n].Position).magnitude;
                if(dist < 0.25){
                    DraggedNode = thisKnot.AllNodes[n];
                    DraggedNodeStartPosition = thisKnot.AllNodes[n].Position;
                    return;
                }
            }
        }
        else if (Display.IsFreeLoopMode())
        {
            FreeLoop.GetComponent<FreeLoop>().AddPoint2FreeCurve(MouseDownVec);
            PreviousPosition = MouseDownVec;
        }
        else if (Display.IsEditKnotMode())
        {

        }
    }

    public void OnMouseDrag()
    {
        MouseDragVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MouseDragVec.z = 0f;
        if (Display.IsDrawKnotMode())
        {
            if (DraggedNode != null)
            {
                float minDist = (MouseDragVec - DraggedNodeStartPosition).magnitude;
                int minNodeId = DraggedNode.ID;
                for (int n = 0; n < thisKnot.AllNodes.Length; n++)
                {
                    Node nd = thisKnot.AllNodes[n];
                    if (nd.ID != DraggedNode.ID)
                    {
                        float dist = (MouseDragVec - nd.Position).magnitude;
                        if (dist < minDist)
                        {
                            return;
                        }
                    }
                }
                // ドラッグされたBeadの座標を変える。
                DraggedNode.ThisBead.Position = MouseDragVec;
                // Nodeの座標も同期する
                DraggedNode.Position = MouseDragVec;
                //Debug.Log("mousePosition"+tmpPos);
                // エッジを作り直す。// Knot.UpdateBeadsを呼び出す。
                if (thisKnot == null) thisKnot = ThisKnot.GetComponent<Knot>();
                thisKnot.UpdateBeadsAtNode(DraggedNode);
                // ドラッグしているノードについて、回転して適正な位置にする。
                // thisKnot.UpdateNodeRotation();
            }
        }
        else if (Display.IsFreeLoopMode())
        {
            if((PreviousPosition - MouseDragVec).magnitude > 0.2f)
            {
                FreeLoop freeloop = FreeLoop.GetComponent<FreeLoop>();
                freeloop.AddPoint2FreeCurve(MouseDragVec);
                PreviousPosition = MouseDragVec;
                // スタート地点に近ければ、画面上にメッセージを出す
                if((MouseDownVec - MouseDragVec).magnitude < 1f /* && ---- */)
                {
                    freeloop.RenderCircleEffect();
                    freeloop.CircleEffectPosition = MouseDownVec;
                    freeloop.CircleEffect.GetComponent<LineRenderer>().enabled = freeloop.CircleEffectEnable = true;
                }
                else
                {
                    freeloop.CircleEffect.GetComponent<LineRenderer>().enabled = freeloop.CircleEffectEnable = false ;
                }
            }
            //????
        }
        else if (Display.IsEditKnotMode())
        {

        }
    }

    public void OnMouseUp()
    {
        if (Display.IsDrawKnotMode())
        {
            DraggedNode = null;
        }
        else if (Display.IsFreeLoopMode())
        {
            FreeLoop freeloop = FreeLoop.GetComponent<FreeLoop>();
            //スタート地点に近くない場所で終了した場合には、すべてを消去して終了
            if (!freeloop.CircleEffectEnable)
            {
                freeloop.FreeCurve.Clear();
                Display.SetDrawKnotMode();
            }
            else //
            {
                //スタート地点に近い場所で終わった場合は、まずはBeadへと変換する。
                // すべてのビーズを消す（不要）
                thisKnot.ClearAll();
                int freeCurveSize = freeloop.FreeCurve.Count;
                // まず1列のbeadの列を作る。
                for (int b = 0; b < freeCurveSize; b++)
                {
                    //ビーズを追加(b=ID番号)
                    thisKnot.AddBead(freeloop.FreeCurve[b],b);
                }
                thisKnot.AllBeads = FindObjectsOfType<Bead>();
                freeCurveSize = thisKnot.AllBeads.Length;// おそらく無意味
                for (int b = 0; b < freeCurveSize; b++)
                {
                    // N1,N2, NumOfNbhdを設定
                    Bead bd = thisKnot.AllBeads[b];
                    bd.N1 = thisKnot.AllBeads[(b + 1) % freeCurveSize];
                    bd.N2 = thisKnot.AllBeads[(b + freeCurveSize - 1) % freeCurveSize];
                    bd.NumOfNbhd = 2;
                }
                //FreeCurveをクリアしておく
                freeloop.CircleEffect.GetComponent<LineRenderer>().enabled
                    = freeloop.CircleEffectEnable = false;
                freeloop.FreeCurve.Clear();
                //モードを戻しておく
                Display.SetDrawKnotMode();

                //重複も許して交点を検出
                List<PairInt> meets = new List<PairInt>();
                for (int b1 = 0; b1 < freeCurveSize; b1++)
                {
                    int b1n = (b1 + 1) % freeCurveSize;
                    int b1p = (b1 + freeCurveSize - 1) % freeCurveSize;
                    for (int b2 = b1 + 1; b2 < freeCurveSize; b2++)
                    {
                        int b2n = (b2 + 1) % freeCurveSize;
                        int b2p = (b2 + freeCurveSize - 1) % freeCurveSize;
                        //int difference = (b2 - b1 + freeCurveSize) % freeCurveSize;//なぜ？
                        int difference = b2 - b1;
                        if (2 < difference && difference < freeCurveSize - 2)
                        {// そもそも異なる場所である保証。
                            float x1 = thisKnot.AllBeads[b1p].Position.x;
                            float y1 = thisKnot.AllBeads[b1p].Position.y;
                            float x2 = thisKnot.AllBeads[b1n].Position.x;
                            float y2 = thisKnot.AllBeads[b1n].Position.y;
                            float x3 = thisKnot.AllBeads[b2p].Position.x;
                            float y3 = thisKnot.AllBeads[b2p].Position.y;
                            float x4 = thisKnot.AllBeads[b2n].Position.x;
                            float y4 = thisKnot.AllBeads[b2n].Position.y;
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
                                    if (Math.Abs(b1 - m1) <= 2 && Math.Abs(b2 - m2) <= 2)
                                    {
                                        Debug.Log("(" + b1 + "," + b2 + ")=(" + m1 + "," + m2 + ")");
                                        OK = false;
                                        break;
                                    }
                                }
                                if (OK)
                                {
                                    meets.Add(new PairInt(b1, b2));
                                }
                            }
                        }
                    }
                }
                if(meets.Count == 0)
                { // 交点の個数が0ならば、Beadを全部捨てる。
                    thisKnot.ClearAllBeads();
                    //モードを戻しておく
                    Display.SetDrawKnotMode();
                }
                else
                {// さもなくば、交点に当たるところをJointにする
                    for(int i=0; i<meets.Count; i++)
                    {
                        int b1 = meets[i].first;
                        int b2 = meets[i].second;
                        Bead bd1 = thisKnot.AllBeads[b1];
                        Bead bd2 = thisKnot.AllBeads[b2];
                        bd1.Joint = true;
                        //Nbhdの繋ぎ替え
                        bd1.U1 = bd2.N1;
                        bd1.U2 = bd2.N2;
                        thisKnot.AllBeads[b2 - 1].N1 = bd1;
                        thisKnot.AllBeads[b2 + 1].N2 = bd1;
                        //消去
                        GameObject obj2 = bd2.gameObject;
                        obj2.SetActive(false);//やる意味が分からないが、やっておいたほうがいいらしい。
                        Destroy(obj2);// 消去！
                    }
                    // 行先のJoint情報を調べる
                    for (int b1 = 0; b1 < freeCurveSize; b1++)
                    {
                        Bead bd = thisKnot.AllBeads[b1];
                        if (bd.Joint)
                        {
                            for(int r1=0; r1<4; r1++)
                            {
                                PairInt br2 = thisKnot.FindEndOfEdgeOnBead(bd, r1);
                            }
                        }
                    }
                    // ジョイント間のビーズ数を数える。
                    // midJointを作る

                    // Node,Edgeを作る

                    // Nbhdを作る

                    //モードを戻す
                    Display.SetDrawKnotMode();

                }
            }
        }
        else if (Display.IsEditKnotMode())
        {

        }
        else if (Display.IsMenuMode())
        {

        }
    }


    //Node AddNode(float x, float y, float t)
    //{
    //    PreFab = Resources.Load("Prefabs/Node") as GameObject;
    //    GameObject go = Node.Instantiate<GameObject>(PreFab, Vector3.zero, Quaternion.identity);
    //    Node nd = go.GetComponent<Node>();
    //    nd.R = new float[4];
    //    nd.EdgeID = new int[4];
    //    nd.SetNodeCoord(x, y, t);
    //    for (int i = 0; i < 4; i++)
    //    {
    //        nd.R[i] = 1f;
    //    }
    //    //Debug.Log("R[3] = "+nd.R[3]);
    //    nd.ID = NodesID;
    //    NodesID++;
    //    return nd;
    //}

    //Edge AddEdge(int ID1, int ID2, int RID1, int RID2)
    //{
    //    PreFab = Resources.Load("Prefabs/Edge") as GameObject;
    //    GameObject go = Node.Instantiate<GameObject>(PreFab, Vector3.zero, Quaternion.identity);
    //    Edge ed = go.GetComponent<Edge>();
    //    ed.SetEdgePara(ID1,ID2, RID1, RID2);
    //    Debug.Log("" + ed.ANodeID + " " + ed.BNodeID);
    //    ed.ID = EdgesID;
    //    GetNodeAt(ID1).EdgeID[RID1] = EdgesID;
    //    GetNodeAt(ID2).EdgeID[RID2] = EdgesID;
    //    EdgesID++;
    //    return ed;
    //}
    //static public int GetNodeLength()
    //{
    //    if (Nodes == null) return 0;
    //    return Nodes.Length;
    //}

    //public static Node GetNodeAt(int i)
    //{
    //    for(int n=0; n< Nodes.Length; n++)
    //    {
    //        if(Nodes[n].ID==i)
    //        {
    //            return Nodes[n];
    //        }
    //    }
    //    return null;
    //}

    //public static Edge GetEdgeAt(int i)
    //{
    //    for (int n = 0; n < Edges.Length; n++)
    //    {
    //        if (Edges[n].ID == i)
    //        {
    //            return Edges[n];
    //        }
    //    }
    //    return null;
    //}


    //void ModifyR_Nodes()
    //{

    //}

    //void ModifyTh_Nodes()
    //{

    //}

    //void ModifyXY_Nodes()
    //{

    //}

    void OnKeyDown()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Display.IsMenuMode())
            {
                thisMenu.HideMenu();
                Display.SetDrawKnotMode();
            }
            else
            {
                thisMenu.ShowMenu();
                Display.SetMenuMode();
            }
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            KeyCodeN();
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            KeyCodeO();
        }
    }

    void KeyCodeN()
    {
        // 消してよいですか[保存][消してよい][戻る]的なダイアログが欲しい
        // 消す
        thisKnot.ClearAll();
        // マウスドラッグで一本線を入力するモードに入る
        Display.SetFreeLoopMode();
        // Mode.
        //　閉じるような曲線の入力を受け付ける
        //　点列から交点を抽出
        //　グラフ構造を抽出

    }

    void KeyCodeO()
    {
        // ファイルダイアログをだしてファイル名を取得
        string[] exts = { "txt", "jpg", "png", "dwk", "pdt"};
        string filePath = Crosstales.FB.FileBrowser.OpenSingleFile("Open a BeadsKnot file", "Samples", exts);
        Debug.Log(filePath);
        if (filePath.Length < 4) return;
        // 拡張子で場合分け
        string ext = filePath.Substring(filePath.Length-3);
        // jpg, png  画像からの読み込み
        if(ext == "jpg" || ext == "png")
        {
        }
        // dwk ドーカー表示
        // if(ext == "dwk"){}
        // pdt pData ファイル？
        // if(ext == "dwk"){}
        // txt BeadsKnotフォーマット
        else
        {
            thisKnot.OpenTxtFile(filePath);
        }
        // OpenBeadsKnotFile(string filename)
    }
}
