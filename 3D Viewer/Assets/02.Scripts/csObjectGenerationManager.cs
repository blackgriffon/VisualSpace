using WpfUnityPacket;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;



public class csObjectGenerationManager : MonoBehaviour
{

    public GameObject WallModel;
    public Text debugLogText;
    public GameObject ParentGameObject;
    private Dictionary<string, AssetBundle> assetBundles;

    //.unity3d
    private int extensionLen = 8;


    private void Awake()
    {



        StartCoroutine(LoadAllAssetBundle("Windows"));
        //StartCoroutine(LoadAssetBundleUsingWebRequest("bundle.unity3d"));
    }


    string assetBundleDirectory = "http://web.visualspace.uy.to/assetBundle/Windows/";


    IEnumerator LoadAllAssetBundle(string folderName)
    {

        assetBundles = new Dictionary<string, AssetBundle>();

        // manifest를 이용해서 모든 에셋번들의 정보를 가져온다.
        var manifestRequest = UnityWebRequest.GetAssetBundle(assetBundleDirectory + folderName);
        yield return manifestRequest.Send();
        var manifestBundle = DownloadHandlerAssetBundle.GetContent(manifestRequest);

        if (manifestBundle == null)
        {
            Debug.LogWarning("Could not load asset bundle manifest.");
            yield break;
        }

        var op =  manifestBundle.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
        yield return op;
        var manifest = op.asset as AssetBundleManifest;


        // manifest의 있는 이름정보를 이용해서 모든 에셋을 로드한다.
        foreach (string bundleName in manifest.GetAllAssetBundles())
        {
            var request = UnityWebRequest.GetAssetBundle(assetBundleDirectory + bundleName, 1, 0);
            yield return request.Send();
            var assetBundle = DownloadHandlerAssetBundle.GetContent(request);
            assetBundles.Add(bundleName.Substring(0, bundleName.Length - extensionLen), assetBundle);

            Debug.Log(bundleName.Substring(0, bundleName.Length - extensionLen));
            
        }


        // manifest를 언로드한다.
        manifestBundle.Unload(true);

    }

    //IEnumerator LoadAssetBundleUsingWebRequest(string assertbundleName)
    //{

    //    while (!Caching.ready)
    //        yield return null;

    //    UnityWebRequest request = UnityWebRequest.GetAssetBundle(assetBundleDirectory + assertbundleName, 1, 0);
    //    yield return request.Send();

    //    AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(request);

    //}

    void PrintLog(string str)
    {
        debugLogText.text = str;

    }


    

    public void LoadWall(WallInfoPacket wallInfo)
    {
        GameObject wall = Instantiate<GameObject>(WallModel);
        wall.name = wallInfo.Name;
        wall.transform.position = new Vector3(wallInfo.PosX, wallInfo.PosY, wallInfo.PosZ);
        wall.transform.localScale = new Vector3(wallInfo.ScaleX, wallInfo.ScaleY, wallInfo.ScaleZ);
        wall.transform.parent = ParentGameObject.transform;
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
        gameobj.transform.parent = ParentGameObject.transform;
        gameobj.tag = "Floor";

    }




    public void LoadObject(ObjectInfoPacket objInfo)
    {
        StartCoroutine(coLoadObject(objInfo));
    }


    public IEnumerator coLoadObject(ObjectInfoPacket objInfo)
    {
        // todo assetBundles["bundle"] ==> assetBundles[objInfo.AssetBundleName]
        AssetBundleRequest assetPrefeb = assetBundles["bundle"].LoadAssetAsync<GameObject>(objInfo.ObjectName);
        yield return assetPrefeb;


        if (assetPrefeb.isDone)
        {
            GameObject pb = assetPrefeb.asset as GameObject;
            GameObject gameobj = Instantiate(pb, new Vector3(0, 0, 0), pb.transform.rotation);
            gameobj.transform.parent = ParentGameObject.transform;
            gameobj.transform.localPosition = new Vector3(objInfo.PosX, objInfo.PosY, objInfo.PosZ);
            gameobj.name = objInfo.Name;
            float x = gameobj.transform.eulerAngles.x;
            float z = gameobj.transform.eulerAngles.z;
            gameobj.transform.rotation = Quaternion.Euler(x, (float)objInfo.Rotation, z);
            gameobj.tag = "Object";

        }
    }

    private void OnApplicationQuit()
    {
        UnloadAllAssetBundle();
    }

    private void UnloadAllAssetBundle()
    {
        if (assetBundles != null)
        {
            foreach (var ab in assetBundles)
            {
                ab.Value.Unload(true);
            }

            assetBundles.Clear();
        }


        
    }
}
