using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WpfUnityPacket;
using ProtoBuf;
using TransportTCP;
using System;

public class csWpfUnityTCPClientManager : MonoBehaviour
{

    WpfUnityTCPClient client = null;

    public csObjetControlManager ObjetControlManager = null;
    public csObjectGenerationManager ObjectGenerationManager = null;
    public GameObject ParentGameObject = null;

   private WpfUnityPacketHeader header;


    private void Awake()
    {
        Application.runInBackground = true;
        client = WpfUnityTCPClient.Instance;
        client.Connect("127.0.0.1", 9010);
        header = new WpfUnityPacketHeader();
        Debug.Log("connected...");

    }

    private void Update()
    {

        processReceivedPacket();
    }


    private void processReceivedPacket()
    {

        if (!client.Recevie(ref header))
            return;

        switch (header.PacketType)
        {
            case WpfUnityPacketType.WallInfoPacket:
                processReceivedWallInfo(header as WallInfoPacket);
                break;

            case WpfUnityPacketType.ObjectInfoPacket:
                processReceivedObjectInfo(header as ObjectInfoPacket);
                break;

            case WpfUnityPacketType.FloorInfoPacket:
                processReceivedFloorInfo(header as FloorInfoPacket);
                break;

            case WpfUnityPacketType.CommandPacket:
                processRecivedCommandInfo(header as CommandPacket);
                break;
        }
    }


    private void processReceivedFloorInfo(FloorInfoPacket floorInfo)
    {
        GameObject gameObj;

        switch (floorInfo.Command)
        {
            case WpfUnityCommand.CREATE:
                ObjectGenerationManager.LoadFloor(floorInfo);
                break;

            case WpfUnityCommand.MOVE:
                ObjetControlManager.SelecetdObject.transform.position =
                    new Vector3(floorInfo.PosX, floorInfo.PosY, floorInfo.PosZ);
                break;

            case WpfUnityCommand.SELECT:
                ObjetControlManager.SelecetdObject = GameObject.Find(floorInfo.Name);
                break;


            case WpfUnityCommand.DESELECT:
                ObjetControlManager.SelecetdObject = null;
                break;


            case WpfUnityCommand.REMOVE:
                gameObj = GameObject.Find(floorInfo.Name);
                ObjetControlManager.SelecetdObject = null;
                Destroy(gameObj);
                break;


        }

    }

    private void processReceivedWallInfo(WallInfoPacket wallInfo)
    {
        GameObject gameObj;

        switch (wallInfo.Command)
        {
            case WpfUnityCommand.CREATE:
                ObjectGenerationManager.LoadWall(wallInfo);
                break;


            case WpfUnityCommand.MOVE:
                gameObj = ObjetControlManager.SelecetdObject.transform.gameObject;
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

            case WpfUnityCommand.REMOVE:
                gameObj = GameObject.Find(wallInfo.Name);
                ObjetControlManager.SelecetdObject = null;
                Destroy(gameObj);
                break;


            case WpfUnityCommand.SELECT:
                ObjetControlManager.SelecetdObject = GameObject.Find(wallInfo.Name);
                break;


            case WpfUnityCommand.DESELECT:
                ObjetControlManager.SelecetdObject = null;
                break;
        }
    }

    private void processReceivedObjectInfo(ObjectInfoPacket objectInfo)
    {
        GameObject gameObj;

        switch (objectInfo.Command)
        {
            case WpfUnityCommand.CREATE:
                ObjectGenerationManager.LoadObject(objectInfo);
                break;

            case WpfUnityCommand.MOVE:
                gameObj = GameObject.Find(objectInfo.Name);
                gameObj.transform.position = new Vector3(objectInfo.PosX, objectInfo.PosY, objectInfo.PosZ);
                break;


            case WpfUnityCommand.SELECT:
                ObjetControlManager.SelecetdObject = GameObject.Find(objectInfo.Name);
                break;

            case WpfUnityCommand.REMOVE:
                gameObj = GameObject.Find(objectInfo.Name);
                ObjetControlManager.SelecetdObject = null;
                Destroy(gameObj);
                break;


            case WpfUnityCommand.DESELECT:
                ObjetControlManager.SelecetdObject = null;
                break;

            case WpfUnityCommand.ROTATION:
                float x = ObjetControlManager.SelecetdObject.transform.eulerAngles.x;
                float z = ObjetControlManager.SelecetdObject.transform.eulerAngles.z;
                ObjetControlManager.SelecetdObject.transform.rotation = Quaternion.Euler(x, (float)objectInfo.Rotation, z);
                break;

        }
    }

    private void processRecivedCommandInfo(CommandPacket command)
    {

        switch (command.Command)
        {
            case WpfUnityCommand.ALLCLEAR:
                ObjetControlManager.SelecetdObject = null;
                ClearAllObjects();
                break;
        }
    }

    public void Send(WpfUnityPacket.WpfUnityPacketHeader data)
    {
        client.Send(data);
    }



    private void ClearAllObjects()
    {
        var objects = ParentGameObject.GetComponentsInChildren<Transform>();

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
