using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoScript : MonoBehaviour {
    int visibility = 0;

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<SpriteRenderer>().color = new Color32(200, 5, 8, 0);
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 6;
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.GetComponent<SpriteRenderer>().color = new Color32(200, 5, 8, (byte) visibility);
        visibility = 200;
        Vector2 pos_ = gameObject.transform.position;
        pos_.x += 0.02f;
        float screenbounds = 6f;
        if (pos_.x >= screenbounds){ pos_.x = 0f; }
        gameObject.transform.position = pos_;
    }
}
