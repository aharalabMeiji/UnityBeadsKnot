﻿using System;
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

    public static int GetMode()
    {
        return Mode;
    }
    public static bool IsMenuMode()
    {
        return (Mode == 0);
    }

    public static bool IsDrawKnotMode()
    {
        return (Mode == 1);
    }
    public static bool IsFreeSizeDrawKnotMode()
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
    public static void SetFreeSizeDrawKnotMode()
    {
        Mode = 11;
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
    public FreeLoop thisFreeLoop;

    Vector3 MouseDownVec;
    Vector3 MouseDragVec;
    Node DraggedNode = null;
    Vector3 DraggedNodeStartPosition;

    bool StartFreeLoop = false;
    Vector3 PreviousPosition;

    private Vector3 StartFreeCurve;
    Bead StartFreeCurveBead;
    
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
        if (Input.GetMouseButtonDown(0))// left pressed
        {
            OnMouseDown();
        }
        if (Input.GetMouseButton(0))// left dragging
        {
            OnMouseDrag();
        }
        if (Input.GetMouseButtonUp(0))// left released
        {
            OnMouseUp();
        }
        if (Input.anyKeyDown)// keyboard
        {
            OnKeyDown();
        }
    }


    public void OnMouseDown()
    {
        MouseDownVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MouseDownVec.z = 0f;
        MouseDownVec /= thisKnot.GlobalRate;
        //Debug.Log(MouseDownVec);
        if (Display.IsDrawKnotMode()) { 
            thisKnot = ThisKnot.GetComponent<Knot>();
            thisKnot.GetAllThings();
            //ノード上をクリックしているかどうかをチェック 
            DraggedNode = null;
            for(int n=0; n<thisKnot.AllNodes.Length; n++)
            {
                Node thisNode = thisKnot.AllNodes[n];
                float dist = (MouseDownVec - thisNode.Position).magnitude;
                if(dist < 0.25f){
                    DraggedNode = thisNode;
                    DraggedNodeStartPosition = thisNode.Position;
                    return;
                }
            }
            //ビーズ（ノード以外）をクリックしているかどうかをチェック
            StartFreeCurveBead = null;
            for(int n=0; n<thisKnot.AllBeads.Length; n++)
            {
                //ノードを見つけたらreturn 
                if ((MouseDownVec - thisKnot.AllBeads[n].Position).magnitude < 0.15f)
                {
                    StartFreeCurve = thisKnot.AllBeads[n].Position;
                    StartFreeCurveBead = thisKnot.AllBeads[n];
                    thisFreeLoop.AddPoint2FreeCurve(MouseDownVec);
                    PreviousPosition = MouseDownVec;
                    // ノードにスポットをつける
                    //for(int i=0; i<thisKnot.AllNodes.Length; i++)
                    //{
                    //    Vector3 Pos = thisKnot.AllNodes[i].Position;
                    //    if (thisKnot.AllNodes[i].Active)
                    //    {
                    //        GameObject prefab = Resources.Load<GameObject>("Prefabs/Spot");
                    //        GameObject obj = Instantiate<GameObject>(prefab, new Vector3(Pos.x, Pos.y, 0.1f), Quaternion.identity);
                    //        obj.transform.localScale = new Vector3(0.25f, 0.25f, 1f);
                    //    }
                    //}
                    return;
                }
            }
            // ノードやビーズ以外にマウスダウンしたとき
            // 新しくフリーループを認める。
            FreeLoopMouseDown();
        }
        else if (Display.IsFreeLoopMode())
        {
            FreeLoopMouseDown();
        }
        else if (Display.IsEditKnotMode())
        {

        }
    }

    /// <summary>
    /// left button dragged
    /// </summary>
    public void OnMouseDrag()
    {
        MouseDragVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MouseDragVec.z = 0f;
        MouseDragVec /= thisKnot.GlobalRate;
        if (Display.IsDrawKnotMode())//通常モードドラッグ中
        {
            //ノードをドラッグしているとき
            if (DraggedNode != null)
            {
                NodeDragging();
            }
            //通常モードでフリーカーブを描いているとき
            else if (StartFreeCurveBead != null)
            {
                float dist = (PreviousPosition - MouseDragVec).magnitude;
                if (dist > 0.1f)
                {//少し進んだら点を追加。
                    int divisionNumber = Mathf.CeilToInt(dist /0.1f) ;
                    for (int repeat = 1; repeat <= divisionNumber; repeat++)
                    {
                        float ratio = 1f * repeat / divisionNumber;
                        thisFreeLoop.AddPoint2FreeCurve(PreviousPosition*(1f-ratio)+MouseDragVec*ratio);
                    }
                    PreviousPosition = MouseDragVec;
                    //ジョイントノードの近くを通り過ぎたら、フリーカーブをやめる。
                    for (int i = 0; i < thisKnot.AllNodes.Length; i++)
                    {
                        Vector3 Pos = thisKnot.AllNodes[i].Position;
                        if (thisKnot.AllNodes[i].Active)
                        {
                            if ((MouseDragVec - Pos).magnitude<0.25f)
                            {//フリーカーブをやめる。
                                thisFreeLoop.FreeCurve.Clear();
                                // スポットを消去する
                                //Spot[] NodeSpots = FindObjectsOfType<Spot>();
                                //for (int j = 0; j < NodeSpots.Length; j++)
                                //{
                                //    Destroy(NodeSpots[j].gameObject);
                                //}
                                StartFreeCurveBead = null;
                            }
                        }
                    }
                }
            }
            else if (StartFreeLoop)
            {// 通常モードでフリーループを追加しているとき
                FreeLoopDragging();
                //ジョイントノードの近くを通り過ぎたら、フリーカーブをやめる。
                for (int i = 0; i < thisKnot.AllNodes.Length; i++)
                {
                    Vector3 Pos = thisKnot.AllNodes[i].Position;
                    if (thisKnot.AllNodes[i].Active)
                    {
                        if ((MouseDragVec - Pos).magnitude < 0.25f)
                        {//フリーカーブをやめる。
                            thisFreeLoop.FreeCurve.Clear();
                            // スポットを消去する
                            //Spot[] NodeSpots = FindObjectsOfType<Spot>();
                            //for (int j = 0; j < NodeSpots.Length; j++)
                            //{
                            //    Destroy(NodeSpots[j].gameObject);
                            //}
                            StartFreeLoop = false;
                        }
                    }
                }
            }
        }
        else if (Display.IsFreeLoopMode())//フリーループモード、ドラッグ中
        {
            FreeLoopDragging();
        }
        else if (Display.IsEditKnotMode())
        {

        }
    }


    public void OnMouseUp()
    {
        Vector3 MouseUpVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MouseUpVec.z = 0f;
        MouseUpVec /= thisKnot.GlobalRate;
        if (Display.IsDrawKnotMode())
        {
            if (DraggedNode != null)
            {//ノードマウスダウンから始めたとき
                if ((MouseUpVec - MouseDownVec).magnitude < 0.05f && DraggedNode.ThisBead.Joint)
                {// ノードをクリック -> クロシングチェンジ
                 //ビーズのデータの変更
                    Bead bd = DraggedNode.ThisBead;
                    Bead tmp = bd.N1;
                    bd.N1 = bd.U2;
                    bd.U2 = bd.N2;
                    bd.N2 = bd.U1;
                    bd.U1 = tmp;
                    //ノードのデータの変更
                    DraggedNode.Theta -= Mathf.PI / 2f;
                    float rtmp = DraggedNode.R[0];
                    DraggedNode.R[0] = DraggedNode.R[3];
                    DraggedNode.R[3] = DraggedNode.R[2];
                    DraggedNode.R[2] = DraggedNode.R[1];
                    DraggedNode.R[1] = rtmp;
                    //エッジのデータの変更
                    Edge[] AllEdges = FindObjectsOfType<Edge>();
                    for (int e = 0; e < AllEdges.Length; e++)
                    {
                        Edge ed = AllEdges[e];
                        if (ed.ANodeID == DraggedNode.ID)
                        {
                            ed.ANodeRID = (ed.ANodeRID + 1) % 4;
                        }
                        else if (ed.BNodeID == DraggedNode.ID)
                        {
                            ed.BNodeRID = (ed.BNodeRID + 1) % 4;
                        }
                    }
                }
                //ノードをドラッグしたときは後処理なし
                DraggedNode = null;
            }
            // フリーカーブを描いているとき
            else if (StartFreeCurveBead != null)
            {
                // スポットを消去する
                //Spot[] NodeSpots = FindObjectsOfType<Spot>();
                //for(int i= NodeSpots.Length; i>=0; i--)
                //{
                //    Destroy(NodeSpots[i].gameObject);
                //}
                ////終了地点がノードでないことが要件
                bool NoProc = false;
                //終了地点がノードだったら、非処理フラグを立てる
                thisKnot.GetAllThings();
                for (int n = 0; n < thisKnot.AllNodes.Length; n++)
                {
                    if ((thisKnot.AllNodes[n].Position - MouseUpVec).magnitude < 0.2f)
                    {
                        NoProc = true;
                    }
                }
                //終了地点がビーズだったら、処理に入る。
                //終了地点がビーズでなかったら、非処理フラグを立てる
                Bead EndFreeCurveBead = null;
                if (!NoProc)
                {
                    NoProc = true;
                    for (int b = 0; b < thisKnot.AllBeads.Length; b++)
                    {
                        if ((thisKnot.AllBeads[b].Position - MouseUpVec).magnitude < 0.15f)
                        {
                            NoProc = false;
                            // 終了ビーズを記録
                            EndFreeCurveBead = thisKnot.AllBeads[b];
                            break;
                        }
                    }
                }
                //非処理フラグが立っていたらフリーカーブを消去して終わり
                if (NoProc)
                {
                    thisFreeLoop.FreeCurve.Clear();
                }
                else// 処理に入る
                {
                    // 開始ビーズから終了ビーズをたどる方法を探索
                    // 開始ビーズから終了ビーズの間にクロシングがどのように表れるかを調査。
                    // first : 0:何もなし、1:オーバーのみ、2:アンダーのみ
                    // second : ビーズ距離
                    PairInt GotoN1 = thisKnot.FindBeadAlongCurve(StartFreeCurveBead, StartFreeCurveBead.N1, EndFreeCurveBead);
                    PairInt GotoN2 = thisKnot.FindBeadAlongCurve(StartFreeCurveBead, StartFreeCurveBead.N2, EndFreeCurveBead);
                    //Debug.Log(GotoN1.first + "," + GotoN2.first);
                    bool BothOK = (GotoN1.first != -1) && (GotoN2.first != -1);
                    // オーバーのみ、またはアンダーのみの時には処理を開始（どちらともとれる場合には、短いほう）
                    if (GotoN1.first != -1 || (BothOK && GotoN1.second < GotoN2.second))
                    {
                        //　開始ビーズから終了ビーズまでの既存のビーズラインを消去
                        thisKnot.DeleteBeadsFromTo(StartFreeCurveBead, StartFreeCurveBead.N1, EndFreeCurveBead);
                        // フリーループにあたる部分をビーズへと変換
                        thisKnot.FreeCurve2Bead(StartFreeCurveBead, EndFreeCurveBead, GotoN1.first);
                        // 旧フリーループと旧ビーズとの交点を探してジョイントにする。
                    }
                    else if (GotoN2.first != -1 || (BothOK && GotoN1.second > GotoN2.second))
                    {
                        //　開始ビーズから終了ビーズまでの既存のビーズラインを消去
                        thisKnot.DeleteBeadsFromTo(StartFreeCurveBead, StartFreeCurveBead.N2, EndFreeCurveBead);
                        // フリーループにあたる部分をビーズへと変換
                        thisKnot.FreeCurve2Bead(StartFreeCurveBead, EndFreeCurveBead, GotoN2.first);
                        // 旧フリーループと旧ビーズとの交点を探してジョイントにする。
                    }
                    else //書き換える条件を満たしていないとき
                    {//すべてを消去して終了
                        thisFreeLoop.FreeCurve.Clear();
                        Display.SetDrawKnotMode();
                    }
                    // グラフ構造を書き換える
                    // 形を整える
                    //thisKnot.Modify();
                    //thisKnot.UpdateBeads();
                }
            }
            else if (StartFreeLoop)
            {
                StartFreeLoop = false;//必ずフリーループモードは終了
                if (!thisFreeLoop.CircleEffectEnable)
                {
                    thisFreeLoop.FreeCurve.Clear();
                    Display.SetDrawKnotMode();
                    // フリーループモードから抜けたことが直感的にわかりにくい。
                }
                else
                {
                    // まず1列のbeadの列を作る。
                    int BeadCount = thisKnot.GetMaxIDOfBead();
                    int freeCurveSize = thisFreeLoop.FreeCurve.Count;
                    for (int b = 0; b < freeCurveSize; b++)
                    {
                        //ビーズを追加(ID番号=BeadCount + 1 + b)
                        thisKnot.AddBead(thisFreeLoop.FreeCurve[b], BeadCount + 1 + b);
                    }
                    //FreeCurveをクリアしておく
                    thisFreeLoop.CircleEffect.GetComponent<LineRenderer>().enabled
                        = thisFreeLoop.CircleEffectEnable = false;
                    thisFreeLoop.FreeCurve.Clear();
                    thisKnot.AllBeads = FindObjectsOfType<Bead>();
                    // N1,N2, NumOfNbhdを設定
                    for (int b = 0; b < freeCurveSize; b++)
                    {
                        Bead bd = thisKnot.GetBeadByID(BeadCount + 1 + b);
                        bd.N1 = thisKnot.GetBeadByID(BeadCount + 1 + (b + 1) % freeCurveSize);
                        bd.N2 = thisKnot.GetBeadByID(BeadCount + 1 + (b + freeCurveSize - 1) % freeCurveSize);
                        bd.NumOfNbhd = 2;
                    }
                    //重複も許して交点を検出
                    List<PairInt> meets = new List<PairInt>();
                    for (int b1 = 0; b1 < thisKnot.GetMaxIDOfBead() + 1; b1++)
                    {
                        Bead b1c = thisKnot.GetBeadByID(b1);
                        if (b1c == null) continue;
                        Bead b1n = b1c.N1;
                        Bead b1p = b1c.N2;
                        if (b1n == null || b1p == null) continue;
                        for (int b2 = BeadCount + 1; b2 < BeadCount + freeCurveSize; b2++)
                        {
                            if (b1 >= b2) continue;
                            Bead b2c = thisKnot.GetBeadByID(b2);
                            if (b2c == null) continue;
                            Bead b2n = b2c.N1;
                            Bead b2p = b2c.N2;
                            if (b2n == null || b2p == null) continue;
                            if (b1c == b2n || b1c == b2p || b1n == b2c || b1n == b2p) continue; // そもそも異なる場所である保証。
                            if (GetMeetPairOfNewFreeCurve(b1, b1n, b1p, b2, b2n, b2p, meets))
                            {
                                meets.Add(new PairInt(b1, b2));
                            }
                        }
                    }
                    if (meets.Count == 0)
                    {
                        // ある程度の長さがありかつ交点の個数が０ならば、トリビアルな成分を追加する。
                        if (freeCurveSize > 20)
                        {
                            Bead Bd = thisKnot.GetBeadByID(BeadCount + 1 + 1);
                            Bd.MidJoint = true;
                            Bd = thisKnot.GetBeadByID(BeadCount + 1 + Mathf.FloorToInt(freeCurveSize / 3));
                            Bd.MidJoint = true;
                            Bd = thisKnot.GetBeadByID(BeadCount + 1 + Mathf.FloorToInt(2 * freeCurveSize / 3));
                            Bd.MidJoint = true;
                            // BeadsからNodeEdgeを更新する
                            thisKnot.CreateNodesEdgesFromBeads();
                            // 形を整える
                            thisKnot.GetAllThings();
                            thisKnot.Modify();
                            thisKnot.UpdateBeads();
                        }
                        else
                        {// 交点の個数が0ならば、新規Beadを全部捨てる。
                            thisKnot.AllBeads = FindObjectsOfType<Bead>();
                            for (int i = 0; i < thisKnot.AllBeads.Length; i++)
                            {
                                Bead bd = thisKnot.AllBeads[i];
                                if (bd.ID > BeadCount)
                                {
                                    bd.N1 = bd.N2 = null;
                                    bd.NumOfNbhd = 0;
                                    bd.Active = false;
                                }
                            }
                        }
                        //モードを戻しておく
                        Display.SetDrawKnotMode();
                    }
                    else
                    {// さもなくば、交点に当たるところをJointにする
                        for (int i = 0; i < meets.Count; i++)
                        {
                            int b1 = meets[i].first;
                            int b2 = meets[i].second;
                            Bead bd1 = thisKnot.GetBeadByID(b1);
                            Bead bd2 = thisKnot.GetBeadByID(b2);
                            if (bd1 == null || bd2 == null) continue;
                            if (bd1.N1 == null || bd2.N1 == null || bd2.N2 == null) continue;
                            bd1.Joint = true;
                            //Nbhdの繋ぎ替え
                            // これをどちらにどちらをつなぐかは、選ぶ必要がある。
                            float bd1x = bd1.Position.x;
                            float bd1y = bd1.Position.y;
                            float n1x = bd1.N1.Position.x - bd1x;
                            float n1y = bd1.N1.Position.y - bd1y;
                            float bd2n1x = bd2.N1.Position.x - bd1x;
                            float bd2n1y = bd2.N1.Position.y - bd1y;
                            float bd2n2x = bd2.N2.Position.x - bd1x;
                            float bd2n2y = bd2.N2.Position.y - bd1y;
                            if (n1x * bd2n1y - n1y * bd2n1x > 0 && n1x * bd2n2y - n1y * bd2n2x < 0)
                            {
                                bd1.U1 = bd2.N1;
                                bd1.U2 = bd2.N2;
                            }
                            else
                            {
                                bd1.U1 = bd2.N2;
                                bd1.U2 = bd2.N1;
                            }
                            if (bd2.N1.N1 == bd2)
                                bd2.N1.N1 = bd1;
                            else if (bd2.N1.N2 == bd2)
                                bd2.N1.N2 = bd1;
                            if (bd2.N2.N1 == bd2)
                                bd2.N2.N1 = bd1;
                            else if (bd2.N2.N2 == bd2)
                                bd2.N2.N2 = bd1;
                            //消去
                            bd2.Active = false;// 
                            // Joint-Jointとなるエッジには間にMidJointを必ず追加する。
                            for (int r1 = 0; r1 < 4; r1++)
                            {
                                PairInt br2 = thisKnot.FindEndOfEdgeOnBead(bd1, r1, true);
                                Bead endBead = thisKnot.FindBeadByID(br2.first);
                                if (endBead != null && endBead.Joint)
                                {
                                    // ジョイント間のビーズ数を数える。
                                    int count = thisKnot.CountBeadsOnEdge(bd1, r1);
                                    // midJointを作る
                                    Bead midJointBead = thisKnot.GetBeadOnEdge(bd1, r1, Mathf.FloorToInt(count / 2));
                                    if (midJointBead != null)
                                        midJointBead.MidJoint = true;
                                    //Debug.Log(bd.ID + "," + r1 + "->" + br2.first + "," + br2.second + "(" + count + ")");
                                }
                            }

                        }
                        thisKnot.AllBeads = FindObjectsOfType<Bead>();
                        // BeadsからNodeEdgeを更新する
                        thisKnot.CreateNodesEdgesFromBeads();
                        // 形を整える
                        thisKnot.GetAllThings();
                        thisKnot.Modify();
                        thisKnot.UpdateBeads();
                    }
                }
            }
        }
        else if (Display.IsFreeLoopMode())
        {
            StartFreeLoop = false;//必ずフリーループモードは終了
            //スタート地点に近くない場所で終了した場合には、すべてを消去して終了
            if (!thisFreeLoop.CircleEffectEnable)
            {
                thisFreeLoop.FreeCurve.Clear();
                Display.SetDrawKnotMode();
                // フリーループモードから抜けたことが直感的にわかりにくい。
            }
            else //
            {
                //以下は長い処理なので、メソッド化が望ましい。
                //スタート地点に近い場所で終わった場合は、まずはBeadへと変換する。
                // すべてのビーズを消す（不要）
                thisKnot.ClearAll();
                // まず1列のbeadの列を作る。
                int freeCurveSize = thisFreeLoop.FreeCurve.Count;
                for (int b = 0; b < freeCurveSize; b++)
                {
                    //ビーズを追加(b=ID番号)
                    thisKnot.AddBead(thisFreeLoop.FreeCurve[b], b);
                }
                //FreeCurveをクリアしておく
                thisFreeLoop.CircleEffect.GetComponent<LineRenderer>().enabled
                    = thisFreeLoop.CircleEffectEnable = false;
                thisFreeLoop.FreeCurve.Clear();
                // 新規ビーズのデータを整える
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
                                        //Debug.Log("(" + b1 + "," + b2 + ")=(" + m1 + "," + m2 + ")");
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
                if (meets.Count == 0)
                {// ある程度の長さがありかつ交点の個数が０ならば、トリビアルな成分を追加する。
                    if (freeCurveSize > 20) {
                        Bead Bd = thisKnot.AllBeads[1];
                        Bd.MidJoint = true;
                        Bd = thisKnot.AllBeads[Mathf.FloorToInt(freeCurveSize / 3)];
                        Bd.MidJoint = true;
                        Bd = thisKnot.AllBeads[Mathf.FloorToInt(2 * freeCurveSize / 3)];
                        Bd.MidJoint = true;
                        // BeadsからNodeEdgeを更新する
                        thisKnot.CreateNodesEdgesFromBeads();
                        // 形を整える
                        thisKnot.GetAllThings();
                        thisKnot.Modify();
                        thisKnot.UpdateBeads();
                    }
                    else
                    {
                        // 全体が短くてかつ交点の個数が0ならば、Beadを全部捨てる。
                        thisKnot.ClearAllBeads();
                    }
                    //モードを戻しておく
                    Display.SetDrawKnotMode();
                }
                else
                {// さもなくば、交点に当たるところをJointにする
                    for (int i = 0; i < meets.Count; i++)
                    {
                        int b1 = meets[i].first;
                        int b2 = meets[i].second;
                        Bead bd1 = thisKnot.AllBeads[b1];
                        Bead bd2 = thisKnot.AllBeads[b2];
                        bd1.Joint = true;
                        //Nbhdの繋ぎ替え
                        // これをどちらにどちらをつなぐかは、選ぶ必要がある。
                        float bd1x = bd1.Position.x;
                        float bd1y = bd1.Position.y;
                        float n1x = bd1.N1.Position.x - bd1x;
                        float n1y = bd1.N1.Position.y - bd1y;
                        float bd2n1x = bd2.N1.Position.x - bd1x;
                        float bd2n1y = bd2.N1.Position.y - bd1y;
                        float bd2n2x = bd2.N2.Position.x - bd1x;
                        float bd2n2y = bd2.N2.Position.y - bd1y;
                        if (n1x * bd2n1y - n1y * bd2n1x > 0 && n1x * bd2n2y - n1y * bd2n2x < 0)
                        {
                            bd1.U1 = bd2.N1;
                            bd1.U2 = bd2.N2;
                        }
                        else
                        {
                            bd1.U1 = bd2.N2;
                            bd1.U2 = bd2.N1;
                        }
                        thisKnot.AllBeads[b2 - 1].N1 = bd1;
                        thisKnot.AllBeads[b2 + 1].N2 = bd1;
                        //消去
                        bd2.Active = false;// 
                    }
                    thisKnot.AllBeads = FindObjectsOfType<Bead>();
                    freeCurveSize = thisKnot.AllBeads.Length;
                    // Joint-Jointとなるエッジには間にMidJointを必ず追加する。
                    for (int b1 = 0; b1 < freeCurveSize; b1++)
                    {
                        Bead bd = thisKnot.AllBeads[b1];
                        if (bd.Joint)
                        {
                            for (int r1 = 0; r1 < 4; r1++)
                            {
                                PairInt br2 = thisKnot.FindEndOfEdgeOnBead(bd, r1, true);
                                Bead endBead = thisKnot.FindBeadByID(br2.first);
                                if (endBead != null && endBead.Joint)
                                {
                                    // ジョイント間のビーズ数を数える。
                                    int count = thisKnot.CountBeadsOnEdge(bd, r1);
                                    // midJointを作る
                                    Bead midJointBead = thisKnot.GetBeadOnEdge(bd, r1, Mathf.FloorToInt(count / 2));
                                    if (midJointBead != null)
                                        midJointBead.MidJoint = true;
                                    //Debug.Log(bd.ID + "," + r1 + "->" + br2.first + "," + br2.second + "(" + count + ")");
                                }
                            }
                        }
                    }

                    // BeadsからNodeEdgeを更新する
                    thisKnot.CreateNodesEdgesFromBeads();
                    // 形を整える
                    thisKnot.GetAllThings();
                    thisKnot.Modify();
                    thisKnot.UpdateBeads();

//                    thisKnot.AdjustEdgeLine();

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


    /// keyCode ここから 

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
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            KeyCodeUpArrow();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            KeyCodeDownArrow();
        }
        else if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            KeyCodeShiftLeftArrow();
        }
        else if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.RightArrow))
        {
            KeyCodeShiftRightArrow();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            KeyCodeD();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            KeyCodeE();
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            KeyCodeN();
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            KeyCodeO();
            thisMenu.HideMenu();
            Display.SetDrawKnotMode();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            KeyCodeS();
            thisMenu.HideMenu();
            Display.SetDrawKnotMode();
        }
    }

    void KeyCodeUpArrow()
    {
        thisKnot.Scale(1.1f);
    }
    void KeyCodeDownArrow()
    {
        thisKnot.Scale(0.909f);
    }
    void KeyCodeShiftLeftArrow()
    {
        thisKnot.Rotation(0.05f);
    }
    void KeyCodeShiftRightArrow()
    {
        thisKnot.Rotation(-0.05f);
    }


    void KeyCodeD()
    {
        Dowker dwk = new Dowker();
        dwk.MakeKnotFromDowkerCode();
        //dwk.dowker_notation();
    }
    void KeyCodeE()
    {
        Dowker dwk = new Dowker();
        dwk.DowkerNotation();
    }

    void KeyCodeN()
    {
        // 消してよいですか[保存][消してよい][戻る]的なダイアログが欲しい
        // 消す
        thisKnot.ClearAll();
        // マウスドラッグで一本線を入力するモードに入る
        Display.SetFreeLoopMode();
        Debug.Log("FreeLoopMode()");
        ThisMenu.GetComponent<Menu>().HideMenu();

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

    void KeyCodeS()
    {
        string filePath = Crosstales.FB.FileBrowser.SaveFile( "Samples", "txt");
        Debug.Log(filePath);
        thisKnot.SaveTxtFile(filePath);
    }


    /// keyCode ここまで 

    void NodeDragging()
    {
        float minDist = (MouseDragVec - DraggedNodeStartPosition).magnitude;
        //int minNodeId = DraggedNode.ID;
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
        //Debug.Log("mousePosition"+ MouseDragVec);
        // エッジを作り直す。// Knot.UpdateBeadsを呼び出す。
        if (thisKnot == null) thisKnot = ThisKnot.GetComponent<Knot>();
        thisKnot.Modify();
        //Debug.Log("before UpdateBeadsAtNode " + thisKnot.GetNodeByID(0).ThisBead.Position);
        thisKnot.UpdateBeadsAtNode(DraggedNode);
        //Debug.Log("after UpdateBeadsAtNode " + thisKnot.GetNodeByID(0).ThisBead.Position);
        //// ドラッグしているノードについて、回転して適正な位置にする。
        if (Input.GetKey(KeyCode.Q))
        {
            DraggedNode.Theta += 0.01f;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            DraggedNode.Theta -= 0.01f;
        }
        //thisKnot.UpdateNodeTheta(DraggedNode);

        //// thisKnot.UpdateNodeRotation();

    }

    void FreeLoopMouseDown()
    {
        Debug.Log("Free loop starts");
        thisFreeLoop.AddPoint2FreeCurve(MouseDownVec);
        PreviousPosition = MouseDownVec;
        StartFreeLoop = true;
    }

    void FreeLoopDragging()
    {
        float dist = (PreviousPosition - MouseDragVec).magnitude;
        if (dist > 0.1f)
        // 未解決 // だいたい同じ方向を向いている、という条件も付けるか？
        {
            int divisionNumber = Mathf.CeilToInt(dist / 0.1f);
            for (int repeat = 1; repeat <= divisionNumber; repeat++)
            {
                float ratio = 1f * repeat / divisionNumber;
                thisFreeLoop.AddPoint2FreeCurve(PreviousPosition * (1f - ratio) + MouseDragVec * ratio);
            }
            PreviousPosition = MouseDragVec;
            // スタート地点に近ければ、画面上にメッセージを出す
            if ((MouseDownVec - MouseDragVec).magnitude < 1f /* && ---- */)
            {
                thisFreeLoop.RenderCircleEffect();
                thisFreeLoop.CircleEffectPosition = MouseDownVec;
                thisFreeLoop.CircleEffect.GetComponent<LineRenderer>().enabled = thisFreeLoop.CircleEffectEnable = true;
            }
            else
            {
                thisFreeLoop.CircleEffect.GetComponent<LineRenderer>().enabled = thisFreeLoop.CircleEffectEnable = false;
            }
        }
    }
    bool GetMeetPairOfNewFreeCurve(int b1, Bead b1n, Bead b1p, int b2, Bead b2n, Bead b2p, List<PairInt> meets)
    {
        float x1 = b1p.Position.x;
        float y1 = b1p.Position.y;
        float x2 = b1n.Position.x;
        float y2 = b1n.Position.y;
        float x3 = b2p.Position.x;
        float y3 = b2p.Position.y;
        float x4 = b2n.Position.x;
        float y4 = b2n.Position.y;
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
            for (int mt = 0; mt < meets.Count; mt++)
            {
                int m1 = meets[mt].first;
                int m2 = meets[mt].second;
                if ( b1==m1 || b2 == m2)
                {
                    //Debug.Log("(" + b1 + "," + b2 + ")=(" + m1 + "," + m2 + ")");
                    return false;
                }
                if (b1n.ID == m1 || b1p.ID == m1 || b2n.ID == m2 || b2p.ID == m2)
                {
                    //Debug.Log("(" + b1 + "," + b2 + ")=(" + m1 + "," + m2 + ")");
                    return false;
                }
            }
            //Debug.Log("(" + b1 + "," + b2 + ")");
            return true;
        }
        return false;
    }
}
