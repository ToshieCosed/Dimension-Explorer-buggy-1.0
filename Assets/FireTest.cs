using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTest : MonoBehaviour {
    // Use this for initialization
    Vector2 targetpos;
    float targetx;
    float targety;
	void Start () {
        //this.targetx = Random.Range(0f, 200f);
        //this.targety = Random.Range(0f, 200f);
        //targetpos = new Vector2(this.targetx, this.targety);
    }
	
	// Update is called once per frame
	void Update () {

        MoveTowardsTargetOnScreen(targetx, targety);
        
	}

    public void MoveTowardsTargetOnScreen(float targetx, float targety)
    {
        Vector2 pos = this.transform.position;
        bool resting = false;

        //if (Mathf.Abs(targetx - pos.x) >10 && Mathf.Abs(targety - pos.y) > 10) { resting = true; }
        //if (Mathf.Abs(pos.x - targetx) >10 && Mathf.Abs(pos.y - targety) > 10) { resting = true; }

        float distance  = Vector2.Distance(new Vector2(targetx, targety), pos);
        //Debug.Log("the distance is " + distance);

        if (distance > 4f) { resting = true; }

        float max_speed = distance / 5;
        if (max_speed < 0.05f)
        {
            max_speed = 0.05f;
        }

        if (max_speed > 0.2f)
        {
            max_speed = 0.2f;
        }

        if (!resting)
        {
            
            if (pos.x < targetx) { pos.x += Random.Range(0.05f, Mathf.Round(max_speed)); }
            if (pos.x > targetx) { pos.x -= Random.Range(0.05f, Mathf.Round(max_speed)); }
            if (pos.y < targety) { pos.y += Random.Range(0.05f, Mathf.Round(max_speed)); }
            if (pos.y > targety) { pos.y -= Random.Range(0.05f, Mathf.Round(max_speed)); }
            this.transform.position = pos;
        }
    }

    public void SetTarget(float targetx_, float targety_)
    {
        targetpos = new Vector2(targetx_, targety_);
        targetx = targetx_;
        targety = targety_;
    }

    public void UpdateTarget(float targetx_, float targety_)
    {
        Vector2 targetpos_ = new Vector2(targetx_, targety_);
        this.targetpos = targetpos_;
        targetx = targetx_;
        targety = targety_;
        
    }
}
