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
    csMoveObjectControl moveObjectControl = null;
    private csLoadAssetBundle assertBundle = null;

    public GameObject WallObject = null;
    public GameObject ParentObject = null;



    private void Awake()
    {
        Application.runInBackground = true;
        client = TCPClientProtoBuf.Instance;
        client.Connect("127.0.0.1", 9000);
        header = new Header();
        Debug.Log("connected...");
        moveObjectControl = GameObject.Find("MoveObjectManager").GetComponent<csMoveObjectControl>();
        assertBundle = ParentObject.GetComponent<csLoadAssetBundle>();
        //StartCoroutine(coRecevicePacket());
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
                        gameObj.transform.parent = ParentObject.transform;
                        gameObj.tag = "Wall";
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
                        moveObjectControl.SelecetdObject = GameObject.Find(wallInfo.Name);
                        break;


                    case WallInfoAction.DESELECT:
                        moveObjectControl.SelecetdObject = null;
                        break;
                }
                break;


            case PacketType.ObjectInfoPacket:
                {
                    ObjectInfoPacket objectInfo = (ObjectInfoPacket)header.Data;

                    switch(objectInfo.Action)
                    {
                        case ObjectAction.CREATE:
                            assertBundle.LoadGameObject(objectInfo);
                            break;

                        case ObjectAction.MOVE:
                            gameObj = GameObject.Find(objectInfo.Name);
                            gameObj.transform.position = new Vector3(objectInfo.PosX, objectInfo.PosY, objectInfo.PosZ);
                            break;


                        case ObjectAction.SELECT:
                            moveObjectControl.SelecetdObject = GameObject.Find(objectInfo.Name);
                            break;

                        case ObjectAction.REMOVE:
                            gameObj = GameObject.Find(objectInfo.Name);
                            Destroy(gameObj);
                            break;


                        case ObjectAction.DESELECT:
                            moveObjectControl.SelecetdObject = null;
                            break;

                    }
                }
                break;
        }
    }

    private void OnApplicationQuit()
    {
        client.Disconnect();
    }
}
