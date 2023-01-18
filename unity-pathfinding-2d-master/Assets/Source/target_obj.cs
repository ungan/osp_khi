using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target_obj : MonoBehaviour
{
    Vector3 target1;
    Vector3 target2;
    Vector3 destination;

    public float t = 1;
    bool change = true;
    float speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        target1 = new Vector3(3, -1, 0);
        target2 = new Vector3(-3, -1, 0);
        destination = target2;


        transform.position = target1;

    }

    // Update is called once per frame
    void Update()
    {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            if (Mathf.Round(transform.position.x) == Mathf.Round(destination.x) && Mathf.Round(transform.position.y) == Mathf.Round(destination.y) && Mathf.Round(transform.position.z) == Mathf.Round(destination.z) && change)
            {
                
                if(t == 1)
                {
                    change = false;
                    destination = target1;
                    t = 2;
                }
            }
    }
}
