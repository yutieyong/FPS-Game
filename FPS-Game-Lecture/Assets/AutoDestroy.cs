using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    static float timer = 0.5f;
    static int number = 0;
    // Start is called before the first frame update
    void Start()
    {
        number++;
        if (number > 1) {
            Destroy(this.gameObject, timer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
