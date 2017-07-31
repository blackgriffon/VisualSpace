using System;
using System.Collections;
using UnityEngine;


public class csCommunicationManager : MonoBehaviour {

    public GameObject transportTcpPrefab;
    _TransportTCP m_transport = null;


    // 접속할 IP 주소
    private string IP = "127.0.0.1";
    private int PORT = 9000;
    private const int buf_size = 1024;


    private void Start()
    {


        StartCoroutine(coRecvie());
    }

    
    IEnumerator coRecvie()
    {
        while(true)
        { 
        if (m_transport != null && m_transport.IsConnected() == true)
        {
            byte[] buffer = new byte[buf_size];
            int recvSize = m_transport.Receive(ref buffer, buffer.Length);
                Debug.Log("수신 길이 : " + recvSize.ToString());
            if (recvSize > 0)
            {
                string message = System.Text.Encoding.UTF8.GetString(buffer);
                Debug.Log(message);
                    AnalyzePacket(message);


            }
        }

            yield return null;
        }
    }


    private void AnalyzePacket(string message)
    {
        string[] cmd = message.Split('|');




        Debug.Log("Cmd : " + cmd[0]);
        if (cmd[0] == "110")
        {

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = cmd[1];
            float startX = Convert.ToInt32(cmd[2]);
            float startZ = Convert.ToInt32(cmd[3]);
            float endX = Convert.ToInt32(cmd[4]);
            float endZ = Convert.ToInt32(cmd[5]);


            cube.transform.position = new Vector3(startX, 1.5f, startZ);
            cube.transform.localScale = new Vector3(endX - startX + 1, 3, endZ - startZ + 1);

        }


    }


    private void OnEnable()
    {
        // Transport 클래스의 컴포넌트를 가져온다.

        //GameObject obj = GameObject.Instantiate(transportTcpPrefab) as GameObject;
        // m_transport = transportTcpPrefab.GetComponent<TransportTCP>();
        // 유니티 창이 비활성화되 있어서 메인스레드가 멈추지 않도록 한다.
        Application.runInBackground = true;

        m_transport = new _TransportTCP();
        m_transport.Connect(IP, PORT);

    }


}
