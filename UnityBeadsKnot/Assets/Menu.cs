using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public bool Show;
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
        GameObject prefab = Resources.Load<GameObject>("Prefabs/MenuItem");
        GameObject obj = Instantiate<GameObject>(prefab, new Vector3(-4.5f, 4f, -1f), Quaternion.identity, transform);
        TextMesh tm = obj.GetComponent<TextMesh>();
        tm.text = "[o] Open file";
        obj = Instantiate<GameObject>(prefab, new Vector3(-4.5f, 3f, -1f), Quaternion.identity, transform);
        tm = obj.GetComponent<TextMesh>();
        tm.text = "[n] free loop";
        MenuBoard.transform.localPosition = Vector3.forward * 0.5f;
        Show = true;
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
        GameObject obj = Instantiate<GameObject>(prefab, new Vector3(-4.5f,4f,-1f), Quaternion.identity, transform);
        TextMesh tm = obj.GetComponent<TextMesh>();
        //tm.text = new string("[Esc] Menu");
        tm.text = "[Esc] Menu";
        MenuBoard.transform.localPosition = Vector3.forward * 1.5f;
        Show = false;
    }
}
