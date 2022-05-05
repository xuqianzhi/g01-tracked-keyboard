using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualizationManagerBehavior : MonoBehaviour
{

    public GameObject keyQ;
    public GameObject keyW;
    public GameObject keyE;
    public GameObject keyR;
    public GameObject keyT;
    public GameObject keyY;
    public GameObject keyU;
    public GameObject keyI;
    public GameObject keyO;
    public GameObject keyP;
    public GameObject keyA;
    public GameObject keyS;
    public GameObject keyD;
    public GameObject keyF;
    public GameObject keyG;
    public GameObject keyH;
    public GameObject keyJ;
    public GameObject keyK;
    public GameObject keyL;
    public GameObject keyZ;
    public GameObject keyX;
    public GameObject keyC;
    public GameObject keyV;
    public GameObject keyB;
    public GameObject keyN;
    public GameObject keyM;
    // public GameObject space;

    private Dictionary<String, GameObject> textToKey;

    private Color originalColor;
    // Start is called before the first frame update
    void Start()
    {
        originalColor = keyA.GetComponent<Image>().color;
        // originalColor = new Color(46, 44, 54);
        textToKey = new Dictionary<string, GameObject>();
        textToKey.Add("a", keyA);
        textToKey.Add("b", keyB);
        textToKey.Add("c", keyC);
        textToKey.Add("d", keyD);
        textToKey.Add("e", keyE);
        textToKey.Add("f", keyF);
        textToKey.Add("g", keyG);
        textToKey.Add("h", keyH);
        textToKey.Add("i", keyI);
        textToKey.Add("j", keyJ);
        textToKey.Add("k", keyK);
        textToKey.Add("l", keyL);
        textToKey.Add("m", keyM);
        textToKey.Add("n", keyN);
        textToKey.Add("o", keyO);
        textToKey.Add("p", keyP);
        textToKey.Add("q", keyQ);
        textToKey.Add("r", keyR);
        textToKey.Add("s", keyS);
        textToKey.Add("t", keyT);
        textToKey.Add("u", keyU);
        textToKey.Add("v", keyV);
        textToKey.Add("w", keyW);
        textToKey.Add("x", keyX);
        textToKey.Add("y", keyY);
        textToKey.Add("z", keyZ);
    }

    // Update is called once per frame
    void Update()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.keyCode.ToString().Length == 1 && char.IsLetter(e.keyCode.ToString()[0]))
        {
            String pressed = e.keyCode.ToString().ToLower();
            StartCoroutine(VisualizePress(pressed));
        }
    }

    private IEnumerator VisualizePress(String pressed)
    {
        GameObject key = textToKey[pressed];
        Image img = key.GetComponent<Image>();
        img.color = Color.cyan;
        yield return new WaitForSeconds(0.15f);
        img.color = originalColor;
    }
}
