using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    // current scores
    int score;

    // records
    static int records;

    // number of bullets remaining
    int num_bullets;

    // player component (script)
    player player;

    // UI component
    Text score_txt;
    Text record_txt;
    Text health_txt;
    Text bullet_txt;


    //public static GameManager getInstance
    // Start is called before the first frame update
    void Start()
    {
        // initialize all the member variables
        Instance = this;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<player>();

        foreach (Transform t in this.transform.GetComponentsInChildren<Transform>()) {
            if (t.name.Equals("bullet")) {
                bullet_txt = t.GetComponent<Text>();
            } else if (t.name.Equals("record")) {
                record_txt = t.GetComponent<Text>();
                record_txt.text = "Records: " + records;
            } else if (t.name.Equals("health")) {
                health_txt = t.GetComponent<Text>();
            } else if (t.name.Equals("score")) {
                score_txt = t.GetComponent<Text>();
            }
        }
    }

    // add score to current score
    public void SetScore(int score) {
        this.score += score;
        if (this.score > records) {
            records = this.score;
        }
        score_txt.text = "Score: <color=yellow>" + this.score + "</color>";
        record_txt.text = "Record: " + records;
    }

    // Subtract bullets_used from num_bullets
    public void SetBullets(int bullets_used) {
        this.num_bullets -= bullets_used;
        if (this.num_bullets <= 0) {
            // sleep for 0.2s, etc.
            this.num_bullets = 50;
        }
        bullet_txt.text = this.num_bullets + "/50";
    }

    public void SetHealth(int health) {
        health_txt.text = health.ToString();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
