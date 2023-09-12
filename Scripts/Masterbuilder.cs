using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Masterbuilder : MonoBehaviour
{
    public Create create;


    
    void Start()
    {
        
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var mousePos = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out hit))
            {
                var currentObject = hit.collider.gameObject;
                create.AddOneprefab(currentObject, hit.normal.normalized);
                StartCoroutine(create.RunGrowth());


            }

        }
        
    }
}
