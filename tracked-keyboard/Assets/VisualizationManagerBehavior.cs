using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualizationManagerBehavior : MonoBehaviour
{

    public List<GameObject> keys;
    // public GameObject space;

    private Dictionary<String, GameObject> textToKey;
    // Start is called before the first frame update
    void Start()
    {
        textToKey = new Dictionary<string, GameObject>();
        foreach (GameObject key in keys)
        {
            String s = key.GetComponentInChildren<Text>().text.ToLower();
            textToKey.Add(s, key);
        }
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
        Color old = img.color;
        img.color = Color.cyan;
        yield return new WaitForSeconds(0.5f);
        img.color = old;
    }
}
