using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Packet;
using ProtoBuf;
using TransportTCP;
using System;

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
        client.Connect("127.0.0.1", 9010);
        header = new Header();
        Debug.Log("connected...");
        moveObjectControl = GameObject.Find("MoveObjectManager").GetComponent<csMoveObjectControl>();
        assertBundle = ParentObject.GetComponent<csLoadAssetBundle>();

    }

    private void Update()
    {

        processReceivedPacket();
    }


    private void processReceivedPacket()
    {

        if (!client.Recevie(ref header))
            return;

        switch (header.ObjectType)
        {
            case Packet.PacketType.WallInfo:
                processReceivedWallInfo(header.Data as WallInfo);
                break;

            case PacketType.ObjectInfoPacket:
                processReceivedObjectInfo(header.Data as ObjectInfoPacket);
                break;

            case PacketType.FloorInfoPacket:
                processReceivedFloorInfo(header.Data as FloorInfoPacket);
                break;

            case PacketType.CommandPacket:
                processRecivedCommandInfo(header.Data as CommandPacket);
                break;
        }
    }


    private void processReceivedFloorInfo(FloorInfoPacket floorInfo)
    {
        GameObject gameObj;

        switch (floorInfo.Action)
        {
            case FloorInfoAction.CREATE:
                assertBundle.LoadFloor(floorInfo);
                break;

            case FloorInfoAction.MOVE:
                moveObjectControl.SelecetdObject.transform.position =
                    new Vector3(floorInfo.PosX, floorInfo.PosY, floorInfo.PosZ);
                break;

            case FloorInfoAction.SELECT:
                moveObjectControl.SelecetdObject = GameObject.Find(floorInfo.Name);
                break;


            case FloorInfoAction.DESELECT:
                moveObjectControl.SelecetdObject = null;
                break;


            case FloorInfoAction.REMOVE:
                gameObj = GameObject.Find(floorInfo.Name);
                Destroy(gameObj);
                break;


        }

    }

    private void processReceivedWallInfo(WallInfo wallInfo)
    {
        GameObject gameObj;

        switch (wallInfo.Action)
        {
            case WallInfoAction.CREATE:
                assertBundle.LoadWall(wallInfo);
                break;


            case WallInfoAction.MOVE:
                gameObj = moveObjectControl.SelecetdObject.transform.gameObject;
                gameObj.transform.position = new Vector3(wallInfo.PosX, wallInfo.PosY, wallInfo.PosZ);

                var components = gameObj.GetComponentsInChildren<Transform>();
                for (int i = 1; i < components.Length; i++)
                {
                    if (components[i].gameObject.name == "front" || components[i].gameObject.name == "back")
                        components[i].gameObject.GetComponent<Renderer>().material.mainTextureScale = new Vector2(wallInfo.ScaleX, wallInfo.ScaleY);
                    else
                        components[i].gameObject.GetComponent<Renderer>().material.mainTextureScale = new Vector2(wallInfo.ScaleZ, wallInfo.ScaleY);
                }

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
    }

    private void processReceivedObjectInfo(ObjectInfoPacket objectInfo)
    {
        GameObject gameObj;

        switch (objectInfo.Action)
        {
            case ObjectAction.CREATE:
                assertBundle.LoadObject(objectInfo);
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

            case ObjectAction.ROTATION:
                float x = moveObjectControl.SelecetdObject.transform.eulerAngles.x;
                //float y = moveObjectControl.SelecetdObject.transform.eulerAngles.y;
                float z = moveObjectControl.SelecetdObject.transform.eulerAngles.z;
                moveObjectControl.SelecetdObject.transform.rotation = Quaternion.Euler(x, (float)objectInfo.Angle, z);
                break;

        }
    }

    private void processRecivedCommandInfo(CommandPacket command)
    {

        switch (command.Action)
        {
            case CommandAction.ALLCLEAR:
                ClearAllObjects();
                break;
        }
    }

    public void Send(Packet.Header data)
    {
        client.Send(data);
    }



    private void ClearAllObjects()
    {
        var objects = ParentObject.GetComponentsInChildren<Transform>();

        for (int i = 1; i < objects.Length; i++)
        {
            Destroy(objects[i].gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        client.Disconnect();
    }
}
