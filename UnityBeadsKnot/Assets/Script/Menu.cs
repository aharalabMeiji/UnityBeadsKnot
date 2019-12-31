using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject MenuBoard;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ShowMenu()
    {
        GameObject[] objs = FindObjectsOfType<GameObject>();
        for (int i = objs.Length - 1; i >= 0; i--)
        {
            if (objs[i].name.Contains("MenuItem"))
            {
                Destroy(objs[i]);
            }
        }
        float Top = 4.8f;
        float VerticalStep = 0.8f;
        GameObject prefab = Resources.Load<GameObject>("Prefabs/MenuItem");
        GameObject obj = Instantiate<GameObject>(prefab, new Vector3(-6.5f, Top, -0.1f), Quaternion.identity, transform);
        TextMesh tm = obj.GetComponent<TextMesh>();
        tm.text = "[o] Open file";
        obj = Instantiate<GameObject>(prefab, new Vector3(-6.5f, Top-VerticalStep, -0.1f), Quaternion.identity, transform);
        tm = obj.GetComponent<TextMesh>();
        tm.text = "[n] Free loop";
        obj = Instantiate<GameObject>(prefab, new Vector3(-6.5f, Top - 2*VerticalStep, -0.1f), Quaternion.identity, transform);
        tm = obj.GetComponent<TextMesh>();
        tm.text = "[up][down] Scaling";
        obj = Instantiate<GameObject>(prefab, new Vector3(-6.5f, Top - 3*VerticalStep, -0.1f), Quaternion.identity, transform);
        tm = obj.GetComponent<TextMesh>();
        tm.text = "[right][left] Rotation";
        obj = Instantiate<GameObject>(prefab, new Vector3(-6.5f, Top - 4 * VerticalStep, -0.1f), Quaternion.identity, transform);
        tm = obj.GetComponent<TextMesh>();
        tm.text = "[esc] Hide Mene";
    }

    public void HideMenu()
    {
        GameObject[] objs = FindObjectsOfType<GameObject>();
        for (int i = objs.Length - 1; i >= 0; i--)
        {
            if (objs[i].name.Contains("MenuItem"))
            {
                Destroy(objs[i]);
            }
        }
        GameObject prefab = Resources.Load<GameObject>("Prefabs/MenuItem");
        GameObject obj = Instantiate<GameObject>(prefab, new Vector3(-6.5f,4.8f,-0.1f), Quaternion.identity, transform);
        TextMesh tm = obj.GetComponent<TextMesh>();
        //tm.text = new string("[Esc] Menu");
        tm.text = "[esc] Show Menu   ";
        if (Display.IsFreeLoopMode())
        {
            tm.text += "Draw a free loop";
        }
    }
}
