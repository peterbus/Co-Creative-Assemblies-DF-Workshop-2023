using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System;


/*The script provided is based on the previous DLA Generator script introduced and developed by 
SoomeenHahm Design 
Diffusion Limited Aggregation in Unity C# Part 1 
https://www.youtube.com/watch?v=WBBAT2pJfs8&list=PLZ55wFj-13MRLrwX7IAl99rhj4D5OexJ2&index=19
and Part 2
https://www.youtube.com/watch?v=vgD273g22Gk&list=PLZ55wFj-13MRLrwX7IAl99rhj4D5OexJ2&index=20 
The original scripts are available via https://www.dropbox.com/sh/sw4icek6jxcpi5a/AAD7fdMe5ZzlxFQMdIJLLqLva?dl=0
all accessed 10/04/2023 [ONLINE]
*/

public class Player : MonoBehaviour //masterbuilder player
{
    public Create dCreate;
    public UDP_Send udp;
    public GameObject activePrefab;

    public Builder builder;


    public void Start()
    {
        activePrefab = GameObject.FindWithTag("activePrefab");
    }
    // Update is called once per frame
    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var mousePos = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out hit))
            {
                var currentObject = hit.collider.gameObject;
                //agents jumps on the masterbuilder's click     
                //and creates a component (active Prefab). It builds an assembly during the demonstration process, based on the lastly added object. The agent always follows the masterbuilder in the demo. 
                dCreate.AddOnePrefab(currentObject, hit.normal.normalized);//calling this function from the Create Class  
                StartCoroutine(dCreate.RunGrowth());//more random if you use more items in growth count 
                builder.transform.localPosition = currentObject.transform.localPosition;
                
            }
        }

    }
   
}


