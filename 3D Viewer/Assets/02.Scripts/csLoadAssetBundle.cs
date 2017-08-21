using Packet;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class csLoadAssetBundle : MonoBehaviour
{

    AssetBundle assetBundle;
   


    private void Awake()
    {

        StartCoroutine(LoadAssetBundle("bundle.unity3d"));
        
    }
    //#if UNITY_EDITOR
    //string assetBundleDirectory = "C:\\Users\\TAEWOO\\Desktop\\VisualSpace\\3D Viewer\\Assets\\AssetBundles\\";
    //#else
    //    string assetBundleDirectory = System.Environment.CurrentDirectory+ "\\AssetBundles\\";
    //#endif

    string assetBundleDirectory = "http://web.visualspace.uy.to/assetBundle/";


    public IEnumerator LoadAssetBundle(string assertbundleName)
    {
        //using (WWW www = new WWW("file:///" + assetBundleDirectory + assertbundleName))
        using (WWW www = new WWW(assetBundleDirectory + assertbundleName))
        {
            yield return www;
            assetBundle = www.assetBundle;
        }
    }


    public void LoadGameObject(ObjectInfoPacket objInfo)
    {
        StartCoroutine(coLoadGameObject(objInfo));
    }


    public IEnumerator coLoadGameObject(ObjectInfoPacket objInfo)
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
            gameobj.tag = "Object";


        }
    }


    public IEnumerator LoadGameObject(string gameObjectName)
    {
        AssetBundleRequest assetPrefeb = assetBundle.LoadAssetAsync<GameObject>(gameObjectName);
        yield return assetPrefeb;

        if (assetPrefeb.isDone)
        {
            GameObject pb = assetPrefeb.asset as GameObject;
            Instantiate(pb,
            new Vector3(Random.Range(0, 20), Random.Range(0, 20), Random.Range(0, 20)),
            pb.transform.rotation).transform.parent = this.transform;
        }
    }

    //void OnGUI()
    //{
    //    // Make a background box
    //    GUI.Box(new Rect(10, 10, 100, 90), "bed Menu");

    //    // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
    //    if (GUI.Button(new Rect(20, 40, 80, 20), "bed1"))
    //    {
    //        StartCoroutine(LoadGameObject("bed_1"));
    //    }

    //    // Make the second button.
    //    if (GUI.Button(new Rect(20, 70, 80, 20), "bed2"))
    //    {

    //        StartCoroutine(LoadGameObject("bed_2"));
    //    }
    //}

}
