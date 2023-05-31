using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDP_Send : MonoBehaviour
{

    private static int localPort;
    
    // prefs
    private string IP;  // define in init
    public int port;  // define in init
    

    // "connection" things
    IPEndPoint remoteEndPoint;
    UdpClient client;

    // gui
    string strMessage = "";


    // call it from shell (as program)
    public void Update()
    {
        UDP_Send sendObj = new UDP_Send(); //gameObject.AddComponent<UDP_Send>()
        sendObj.init();

        // testing via console
        // sendObj.inputFromConsole();

        // as server sending endless
        //sendObj.sendEndless(" endless infos \n");
        
        sendObj.sendString(strMessage);
             

    }
    // start from unity3d
    public void Start()
    {
        
        init();
    }

    // OnGUI graphical user interface
    void OnGUI()
    {
        Rect rectObj = new Rect(40, 380, 200, 400);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        GUI.Box(rectObj, "# UDPSend-Data\n127.0.0.1 " + port + " #\n"
                    + "shell> nc -lu 127.0.0.1  " + port + " \n"
                , style);

        // ------------------------
        // send it button
        // ------------------------
        strMessage = GUI.TextField(new Rect(40, 420, 140, 20), strMessage);
        if (GUI.Button(new Rect(190, 420, 40, 20), "send"))
        {
            sendString(strMessage + "\n");
        }
    }

    // init
    public void init()
    {
        // Print it on a console
        print("UDP_Send.init()");

        // define
        IP = "127.0.0.1";
        port = 8051;

        // ----------------------------
        // Send it
        // ----------------------------
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();

        // status
        print("Sending to " + IP + " : " + port);
        print("Testing: nc -lu " + IP + " : " + port);

    }

    // inputFromConsole
    private void inputFromConsole()
    {
        try
        {
            string text;
            do
            {
                text = Console.ReadLine();

                // Text to send.
                if (text != "")
                {

                    // transfer to Binar code .
                    byte[] data = Encoding.UTF8.GetBytes(text);

                    // data send
                    client.Send(data, data.Length, remoteEndPoint);
                }
            } while (text != "");
        }
        catch (Exception err)
        {
            print(err.ToString());
        }

    }

    // sendData
    private void sendString(string pos)
    {
        try
        {
            // if (pos != "")
          //   {

            // Daten mit der UTF8-Kodierung in das Binärformat kodieren.
            //byte[] data = Encoding.UTF8.GetBytes(message);
            //byte[] data = Encoding.UTF8.GetBytes(message);
            pos = GameObject.FindWithTag("Prefab").transform.position.ToString();//sending data with the tag Prefab
            byte[] data = Encoding.UTF8.GetBytes(pos);
            Debug.Log(pos);

            // Den message zum Remote-Client senden.
            client.Send(data, data.Length, remoteEndPoint);
            
          //   }
        } 
        catch (Exception err)
        {
            print(err.ToString());
        }
    }


    // endless test
    private void sendEndless(string testStr)
    {
        do
        {
            sendString(testStr);
        }
        while (true);

    }


}
 

