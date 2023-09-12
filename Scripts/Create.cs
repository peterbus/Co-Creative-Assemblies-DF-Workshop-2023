using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.Net;
using System.Net.Sockets;
using System.Text;


//the core is based on tutorials DLA algorithm
//reference : https://www.youtube.com/watch?v=vgD273g22Gk&list=PLZ55wFj-13MRLrwX7IAl99rhj4D5OexJ2&index=20
//reference:  https://www.youtube.com/watch?v=vgD273g22Gk&list=PLZ55wFj-13MRLrwX7IAl99rhj4D5OexJ2&index=20



public class Create : MonoBehaviour

{
    //global variables
    private GameObject newPrefab;
    public GameObject[] objectsToInstatiate;
    private GameObject activePrefab;
    int CounterPrefab = 0;
    public int GrowthCount = 0;
    public Transform Pos;
    public GameObject environment;
    private GameObject Generator = null;
    public Agent_builder agent;

    List<GameObject> allPrefabs = null;



    // Start is called before the first frame update
    void Start()
    {
        SetupScene();
    }

    // Update is called once per frame
    void Update()
    {
        //AddComponents();
        UDP();
    }

    public IEnumerator RunGrowth()

    {
        for (int i = 0; i < GrowthCount; i++)
        {
            yield return new WaitForSeconds(0.1f);
            AddComponents();
        }
    }



    public void SetupScene()

    {

        if (allPrefabs != null)

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


        int n = UnityEngine.Random.Range(0, objectsToInstatiate.Length);
        allPrefabs = new List<GameObject>();//declaration of the "empty container"
        GameObject newPrefab = Instantiate(objectsToInstatiate[n], Pos.position, Pos.rotation);//vreation of the new component
        newPrefab.transform.parent = environment.transform;
        allPrefabs.Add(newPrefab);//adding it into the list

        if (activePrefab == null)
        {
            activePrefab = Instantiate(objectsToInstatiate[n], Pos.position, Pos.rotation);
            activePrefab.GetComponent<MeshRenderer>().material.color = Color.magenta;
            activePrefab.tag = "activePrefab";
        }

        activePrefab.transform.position = newPrefab.transform.position;
        activePrefab.transform.parent = environment.transform;


        if (Generator == null)
        {
            Generator = Instantiate(objectsToInstatiate[n], Pos.position, Pos.rotation);
            Generator.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            Generator.GetComponent<MeshRenderer>().material.color = Color.blue;
            Generator.transform.parent = environment.transform;
        }

    }


    public void AddComponents()
    {

        if (activePrefab != null)
        {

            Vector3 CreateDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(0f, 0f), UnityEngine.Random.Range(-1f, 1f)) * 1f;

            if (allPrefabs.Count < 50)// maximum number in the assembly
            {
                Vector3 dTester = CreateDirection + activePrefab.transform.position;

                dTester.x = UnityEngine.Random.Range(-8f, -1f);
                dTester.y = UnityEngine.Random.Range(-1f, 10f);
                dTester.z = UnityEngine.Random.Range(-1f, 8f);

              //  Vector3 dTester = CreateDirection + activePrefab.transform.position;
           

               //dTester.y = 0;

                Generator.transform.position = dTester;

                GameObject closestGeo = GetClosestGeo(dTester, allPrefabs);
                Vector3 growthDir = dTester - closestGeo.transform.position;
                AddOneprefab(closestGeo, growthDir);
            }
            else
            {

                agent.EndEpisode();
                agent.AddReward(5f);//this is the treat for the building agent, as it reached the desired number of components in the assembly
                SetupScene();


            }

        }

    }



    private GameObject GetClosestGeo(Vector3 testV, List<GameObject> allPrefabs)
    {
        GameObject closestGeo = null;

        if (allPrefabs.Count > 0)
        {
            float closestDistance = 2f;

            for (int i = 0; i < allPrefabs.Count; i++)
            {
                GameObject currentGeo = allPrefabs[i];
                float dist = Vector3.Distance(testV, currentGeo.transform.position);

                if (dist <= closestDistance || dist > 1f)

                {

                    closestGeo = currentGeo;
                    closestDistance = dist;
                }
            }

        }

        return closestGeo;
    }


    //creation of the building block in the scene
    public void AddOneprefab(GameObject testGameObj, Vector3 growDir)
    {

        growDir.Normalize();
        growDir = forceOrtho(growDir);
        int n = UnityEngine.Random.Range(0, objectsToInstatiate.Length);

        Vector3 newPos = testGameObj.transform.position + growDir;

        //just to make sure the building block is not in collision with other blocks. If overlapping, move it in an offset distance
        Collider[] colliders = Physics.OverlapBox(newPos, new Vector3(0f, 0.5f, 0f));
        if (colliders.Length > 0)
        {
            float yOffset = 1.0f;
            newPos.y += yOffset;
        }

        newPrefab = Instantiate(objectsToInstatiate[n], newPos, Pos.rotation);
        newPrefab.transform.parent = environment.transform;

        activePrefab.transform.position = newPrefab.transform.position;
        activePrefab.transform.parent = environment.transform;

        allPrefabs.Add(newPrefab);
       
    }

    //control of building the assembly always in an orthogonal way
    public Vector3 forceOrtho(Vector3 inVec)
    {
        Vector3 temp = inVec;
        if (Mathf.Abs(temp.x) > Mathf.Abs(temp.y) && Mathf.Abs(temp.x) > Mathf.Abs(temp.z))
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
        Vector3 assembly = new Vector3(temp.x, temp.y, temp.z);
        return assembly;
    }


    //vector transform position  of the active prefab, necessary for the agent builder, called in the Agent class
    public Vector3 GetPosition()
    {
        if (activePrefab != null)
        {
            return activePrefab.transform.position;
        }
        else
        {
            return Vector3.zero;
        }

    }

    public void UDP()
    {
        //UDP sending data
        UdpClient client = new UdpClient(8050);
        try
        {
            client.Connect("127.0.0.1", 8050);

            foreach (GameObject prefab in allPrefabs)
            {

                string pos = prefab.transform.position.ToString();
                Debug.Log(pos);
                byte[] data = Encoding.UTF8.GetBytes(pos);
                // byte[] sendBytes = Encoding.ASCII.GetBytes("Hello, from the client");

                //client.Send(sendBytes, sendBytes.Length);
                client.Send(data, data.Length);
            }
        }
        catch (Exception e)
        {
            print("Exception thrown " + e.Message);
        }
    }

}




