using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Packet;
using ProtoBuf;
using TransportTCP;

public class csTCPClientManager : MonoBehaviour
{

    TCPClientProtoBuf client = null;
    Packet.Header header = null;
    GameObject parentObject = null;
    csMoveWall moveWall = null;
    public GameObject WallObject = null;


    private void Awake()
    {
        Application.runInBackground = true;
        client = TCPClientProtoBuf.Instance;
        client.Connect("127.0.0.1", 9000);
        header = new Header();
        Debug.Log("connected...");
        parentObject = GameObject.Find("Objects");
        moveWall = GameObject.Find("MoveObjectManager").GetComponent<csMoveWall>();
        //StartCoroutine(coRecevicePacket());
    }


    IEnumerator coRecevicePacket()
    {
        while (true)
        {
            if (header == null)
            {
                yield return new WaitForSeconds(0.01f);
                continue;
            }

            if (!client.Recevie(ref header))
            {
                yield return new WaitForSeconds(0.01f);
                continue;
            }

            switch (header.ObjectType)
            {
                case Packet.PacketType.WallInfo:
                    GameObject cube;
                    Debug.Log("recevied Data PacketType.WallInfo");
                    WallInfo wallInfo = (WallInfo)header.Data;
                    switch (wallInfo.Action)
                    {
                        case WallInfoAction.CREATE:
                            //cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            cube = Instantiate(WallObject);
                            cube.name = wallInfo.Name;
                            cube.transform.position = new Vector3(wallInfo.PosX, wallInfo.PosY, wallInfo.PosZ);
                            cube.transform.localScale = new Vector3(wallInfo.ScaleX, wallInfo.ScaleY, wallInfo.ScaleZ);
                            cube.transform.parent = parentObject.transform;
                            cube.tag = "Wall";
                            break;

                        case WallInfoAction.MOVE:
                            cube = GameObject.Find(wallInfo.Name);
                            cube.transform.position = new Vector3(wallInfo.PosX, wallInfo.PosY, wallInfo.PosZ);
                            cube.transform.localScale = new Vector3(wallInfo.ScaleX, wallInfo.ScaleY, wallInfo.ScaleZ);
                            break;

                        case WallInfoAction.REMOVE:
                            cube = GameObject.Find(wallInfo.Name);
                            Destroy(cube);
                            break;
                    }
                    break;

            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    public void Send(Packet.Header data)
    {
        client.Send(data);
    }



    private void Update()
    {
        if (header == null)
            return;

        if (!client.Recevie(ref header))
            return;

        switch (header.ObjectType)
        {
            case Packet.PacketType.WallInfo:
                GameObject gameObj;
                Debug.Log("recevied Data PacketType.WallInfo");
                WallInfo wallInfo = (WallInfo)header.Data;
                switch (wallInfo.Action)
                {
                    case WallInfoAction.CREATE:
                        //gameObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        gameObj = Instantiate(WallObject);
                        gameObj.GetComponent<MeshRenderer>().enabled = true;
                        gameObj.name = wallInfo.Name;
                        gameObj.transform.position = new Vector3(wallInfo.PosX, wallInfo.PosY, wallInfo.PosZ);
                        gameObj.transform.localScale = new Vector3(wallInfo.ScaleX, wallInfo.ScaleY, wallInfo.ScaleZ);
                        gameObj.transform.parent = parentObject.transform;
                        gameObj.tag = "Object";
                        break;

                    case WallInfoAction.MOVE:
                        gameObj = GameObject.Find(wallInfo.Name);
                        gameObj.transform.position = new Vector3(wallInfo.PosX, wallInfo.PosY, wallInfo.PosZ);
                        gameObj.transform.localScale = new Vector3(wallInfo.ScaleX, wallInfo.ScaleY, wallInfo.ScaleZ);
                        break;

                    case WallInfoAction.REMOVE:
                        gameObj = GameObject.Find(wallInfo.Name);
                        Destroy(gameObj);
                        break;


                    case WallInfoAction.SELECT:
                        moveWall.SelecetdObject = GameObject.Find(wallInfo.Name);
                        break;


                    case WallInfoAction.DESELECT:
                        moveWall.SelecetdObject = null;
                        break;
                }




                break;
        }
    }

    private void OnApplicationQuit()
    {
        client.Disconnect();
    }
}
