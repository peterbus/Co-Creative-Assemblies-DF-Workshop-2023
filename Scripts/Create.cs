using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

/*The script provided is based on the DLA Generator script introduced and developed by 
SoomeenHahm Design 
Diffusion Limited Aggregation in Unity C# Part 1 
https://www.youtube.com/watch?v=WBBAT2pJfs8&list=PLZ55wFj-13MRLrwX7IAl99rhj4D5OexJ2&index=19
and Part 2
https://www.youtube.com/watch?v=vgD273g22Gk&list=PLZ55wFj-13MRLrwX7IAl99rhj4D5OexJ2&index=20 
The original scripts are available via https://www.dropbox.com/sh/sw4icek6jxcpi5a/AAD7fdMe5ZzlxFQMdIJLLqLva?dl=0
all accessed 10/04/2023 [ONLINE].
*/

public class Create : MonoBehaviour
{
    public GameObject[] objectsToInstantiate;
    public Transform Pos;
    public Builder builder;
    
    List<GameObject> allPrefabs = null;
    public int GrowthCount = 0;
    private GameObject activePrefab;
    private GameObject Generator = null;
    public GameObject Environment;
    public LayerMask collisionLayer;
    private GameObject newPrefab;
    int counterPrefab = 0;

    
    // Start is called before the first frame update
    void Start()
    {
        SetupScene();
        foreach (GameObject obj in objectsToInstantiate)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            //if the object does not have a rigid body, add one
            if (rb == null)
            {
                rb = obj.AddComponent<Rigidbody>();
            }

        }

    }
    private void Update()
    {
        // AddComponentAgent();      
    }
    public IEnumerator RunGrowth()//certain time delay for each component to be added, to observe the growth 
    {        
        for (int i=0; i<GrowthCount; i++) 
        {
            yield return new WaitForSeconds(0.1f);         
            AddComponentAgent();
            //Debug.Log("Coroutine started");
        }
    }

    public void SetupScene()
    {
        // This is a reset function to delete all previously created components in the scene after the reset button is pressed
        //GetPosition();
        if (allPrefabs != null )
        {
            if (allPrefabs.Count > 0)
            {
                foreach (var pf in allPrefabs)
                {
                    Destroy(pf);
                }
                allPrefabs.Clear();
            }
        }
            Vector3 offset_env = Environment.transform.position;
            Vector3 correctPos = offset_env - Pos.localPosition;

            int n = UnityEngine.Random.Range(0, objectsToInstantiate.Length);
            allPrefabs = new List<GameObject>();
            GameObject newPrefab = Instantiate(objectsToInstantiate[n], correctPos, Pos.localRotation, Environment.transform);
            //newPrefab.transform.SetParent(Environment.gameObject.transform);
            allPrefabs.Add(newPrefab);


        
        if (activePrefab == null)
        {
            activePrefab = Instantiate(objectsToInstantiate[n], correctPos, Pos.localRotation, Environment.transform);//lastly added prefab each time
            activePrefab.transform.localScale = new Vector3(1f, 2f, 1f); //initial starting cube. The scale units are related exactly to properties of the prefab. 
            activePrefab.GetComponent<MeshRenderer>().material.color = Color.magenta;// new Color(1.0f, 0.15f, 0.15f);
            activePrefab.tag = "activePrefab";
            //activePrefab.transform.SetParent(Environment.gameObject.transform);
            // Debug.Log("Component" + objectsToInstantiate[n] + "added");

        }
        activePrefab.transform.localPosition = newPrefab.transform.localPosition;
        //visualisation where the tester vector is created
        if(Generator == null)
        {
            Generator = Instantiate(objectsToInstantiate[n], correctPos, Pos.localRotation, Environment.transform);
            Generator.transform.localScale = new Vector3(1f, 1f, 1f); //initial starting cube
            Generator.GetComponent<MeshRenderer>().material.color = new Color(0.15f, 0.15f, 1.0f);
           // Generator.transform.SetParent(Environment.gameObject.transform);
            // Debug.Log("Component" + objectsToInstantiate[n] + "added");
        }
    }
    //function to automatically add multiple components
    public void AddComponentAgent() 
    {   
        if (activePrefab !=null)
        {
            Vector3 CreateDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-1f, 1f))*2f;//the multuplication controls the spread of the components. e.g. the increased Y coordinate generates more vertical assemblies. 
           
            //find the closest existing component and add a new next to it
            if (allPrefabs.Count<70)//the MAXIMUM number of added components if the function is called separately in Update() method, the maximum components in the assembly
               {
                 Vector3 dTester = CreateDirection + activePrefab.transform.localPosition;//constrained growth closer to where we click(lastly added component)
                 //dTester.y = 0;//in case of 2D only
                 Generator.transform.localPosition = dTester;//blue cube position
                 GameObject closestGeo = GetClosestGeo(dTester, allPrefabs);//take the closest geometry out of the assembly
                 
                 //Debug.Log(closestGeo.transform.localPosition);
                 Vector3 growthDir = dTester - closestGeo.transform.localPosition;
                 AddOnePrefab(closestGeo, growthDir);

            }
            else //restart the scene after reaching the specific amount of assembled components
            {
                var agent = builder.gameObject.GetComponent<Builder>(); //access to the Builder class
                agent.AddReward(+10f);//the treat
                agent.EndEpisode();//using the method from the builder class

                SetupScene();

               // Debug.Log("Scene set done");
            }       
        }
    }

    //calculate the closest component from the tester vector
    private GameObject GetClosestGeo(Vector3 testV, List<GameObject> allPrefabs) //List<GameObject> allPrefabs
    {
        GameObject closestGeo = null;
        if (allPrefabs.Count>0)
        {
            float closestDistance = 1.5f;//maximum distance within which the new component is added
            for(int i=0; i<allPrefabs.Count; i++)
            {
                GameObject currentGeo = allPrefabs[i];
                float dist = Vector3.Distance(testV, currentGeo.transform.localPosition);
                
                if(dist <= closestDistance || dist > 0.5f) //range within which the component is added  - two options. The coroutine will add the components within the 1.5 f distance from the agent.             
                {
                    closestGeo = currentGeo;
                    closestDistance = dist;
                    Debug.Log(closestGeo);
                }
            }
        }
        return closestGeo;
        
    }

    public void AddOnePrefab(GameObject testGameObj, Vector3 growDir)
    {
        growDir.Normalize();
        growDir = forceOrtho(growDir);//orthogonal vector, growth is executed within those constraints, defines the position of the growing system       
        int n = UnityEngine.Random.Range(0, objectsToInstantiate.Length);
        Vector3 newPos = testGameObj.transform.localPosition + growDir;
        Vector3 offset_env = Environment.transform.position;
        Vector3 correctPos = offset_env - Pos.localPosition;
        //this part checks the collisions-adjustment of the positions in y direction
        Collider[] colliders = Physics.OverlapBox(newPos, new Vector3(0f, 1.0f, 0f));
        if (colliders.Length > 0)
        {
            float yOffset = 1.0f;
            newPos.y += yOffset;
        }

        //instantiate specific order of prefabs, based on counting
          if (counterPrefab == 0)
          {
              newPrefab = Instantiate(objectsToInstantiate[0], correctPos, Pos.rotation, Environment.transform);
              counterPrefab++;
          }else
          if (counterPrefab==1)
          {
          newPrefab = Instantiate(objectsToInstantiate[1], correctPos, Pos.rotation, Environment.transform);
             counterPrefab++;
          }else 
          if(counterPrefab==2)
          {
          newPrefab = Instantiate(objectsToInstantiate[2], correctPos, Pos.rotation, Environment.transform);
             counterPrefab=0;
          }

        //newPrefab = Instantiate(objectsToInstantiate[n], newPos, Pos.localRotation);
        activePrefab.transform.localPosition = newPos;
        allPrefabs.Add(newPrefab);  

    }

    //rectangular grid of voxels, snapped on all 6 sizes and scaled by 1 or 2 in y direction
    public Vector3 forceOrtho(Vector3 inVec) //orthogonal vector forcing the components being within the orthogonal grid
    {//
        Vector3 temp = inVec;//temporary vector
        if (Mathf.Abs(temp.x) > Mathf.Abs(temp.y) && Mathf.Abs(temp.x) > Math.Abs(temp.z))
        {
            temp.y = 0;
            temp.z = 0;
        }
        else if (Mathf.Abs(temp.y) > Mathf.Abs(temp.z))
        {
            temp.z = 0;
            temp.x = 0;          
        }
        else
        {
            temp.x = 0;
            temp.y = 0;
        }
        temp.Normalize();

        //final assembly positions
        Vector3 assembly = new Vector3(temp.x, temp.y, temp.z);//adding certain value to the current temp xyz variables can influnece the proportions of the rectangular grid. depends of the scale size of the prefab, I.e. if the object's scale size in Y direction is 2, one half of it is 1. Shif in one half in Y direction can be controled through this
        return assembly;
        
    }

public Vector3 GetPosition() //for the agent's observation
    {
        if (activePrefab != null)
        {
            return activePrefab.transform.localPosition;
        }
        else
        {
            return Vector3.zero;
        }
    }

}
