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
                FreeLoop.GetComponent<FreeLoop>().AddPoint2FreeCurve(MouseDragVec);
                PreviousPosition = MouseDragVec;
            }
            // スタート地点に近ければ、画面上にメッセージを出す
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
