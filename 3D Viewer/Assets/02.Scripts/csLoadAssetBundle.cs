﻿using Packet;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

public class csLoadAssetBundle : MonoBehaviour
{

    AssetBundle assetBundle;
    public GameObject WallModel;
    public Text debugLogText;


    private void Awake()
    {
        StartCoroutine(LoadAssetBundleUsingWebRequest("bundle.unity3d"));
    }


    string assetBundleDirectory = "http://web.visualspace.uy.to/assetBundle/Windows/";

    IEnumerator LoadAssetBundleUsingWebRequest(string assertbundleName)
    {

        while (!Caching.ready)
            yield return null;

        UnityWebRequest request = UnityWebRequest.GetAssetBundle(assetBundleDirectory + assertbundleName, 1, 0);
        yield return request.Send();

        assetBundle = DownloadHandlerAssetBundle.GetContent(request);

    }

    void PrintLog(string str)
    {
        debugLogText.text = str;
    }


    

    public void LoadWall(WallInfo wallInfo)
    {
        GameObject wall = Instantiate<GameObject>(WallModel);
        wall.name = wallInfo.Name;
        wall.transform.position = new Vector3(wallInfo.PosX, wallInfo.PosY, wallInfo.PosZ);
        wall.transform.localScale = new Vector3(wallInfo.ScaleX, wallInfo.ScaleY, wallInfo.ScaleZ);
        wall.transform.parent = this.transform;
        wall.tag = "Wall";

        var components = wall.GetComponentsInChildren<Transform>();



        if (wallInfo.ImageData != null)
        {

            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(wallInfo.ImageData);

            for (int i = 1; i < components.Length; i++)
            {
                components[i].gameObject.GetComponent<Renderer>().material.mainTexture = texture;

            }
        }

        for (int i = 1; i < components.Length; i++)
        {
            if (wallInfo.ScaleX > wallInfo.ScaleZ)
            {
                if (components[i].gameObject.name == "front" || components[i].gameObject.name == "back")
                    components[i].gameObject.GetComponent<Renderer>().material.mainTextureScale = new Vector2(wallInfo.ScaleX, wallInfo.ScaleY);
                else
                    Destroy(components[i].gameObject);
            }
            else
            {

                if (components[i].gameObject.name != "front" && components[i].gameObject.name != "back")
                    components[i].gameObject.GetComponent<Renderer>().material.mainTextureScale = new Vector2(wallInfo.ScaleZ, wallInfo.ScaleY);
                else
                    Destroy(components[i].gameObject);
            }

        }

    }


    public void LoadFloor(FloorInfoPacket floorInfo)
    {

        GameObject gameobj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gameobj.name = floorInfo.Name;


        if (floorInfo.ImageData != null)
        {

            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(floorInfo.ImageData);
            gameobj.GetComponent<Renderer>().material.mainTexture = texture;
            gameobj.GetComponent<Renderer>().material.mainTextureScale = new Vector2(floorInfo.ScaleX, floorInfo.ScaleZ);
        }


        gameobj.transform.position = new Vector3(floorInfo.PosX, floorInfo.PosY, floorInfo.PosZ);
        gameobj.transform.localScale = new Vector3(floorInfo.ScaleX, floorInfo.ScaleY, floorInfo.ScaleZ);
        gameobj.transform.Rotate(new Vector3(0, 180, 0));
        gameobj.transform.parent = this.transform;
        gameobj.tag = "Floor";

    }




    public void LoadObject(ObjectInfoPacket objInfo)
    {
        StartCoroutine(coLoadObject(objInfo));
    }


    public IEnumerator coLoadObject(ObjectInfoPacket objInfo)
    {
        AssetBundleRequest assetPrefeb = assetBundle.LoadAssetAsync<GameObject>(objInfo.AssetBundleName);
        yield return assetPrefeb;


        if (assetPrefeb.isDone)
        {
            GameObject pb = assetPrefeb.asset as GameObject;
            GameObject gameobj = Instantiate(pb, new Vector3(0, 0, 0), pb.transform.rotation);
            gameobj.transform.parent = this.transform;
            gameobj.transform.localPosition = new Vector3(objInfo.PosX, objInfo.PosY, objInfo.PosZ);
            gameobj.name = objInfo.Name;
            float x = gameobj.transform.eulerAngles.x;
            float z = gameobj.transform.eulerAngles.z;
            gameobj.transform.rotation = Quaternion.Euler(x, (float)objInfo.Angle, z);
            gameobj.tag = "Object";

        }
    }

    private void OnApplicationQuit()
    {
        if (assetBundle != null)
            assetBundle.Unload(true);
    }


}
