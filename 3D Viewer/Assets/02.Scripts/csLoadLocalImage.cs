using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csLoadLocalImage : MonoBehaviour
{

    private void Awake()
    {
        WWW www;

        string temp_url = "file://"; /// www 를 쓰기 위해선 uri 경로 앞에 file:// 붙어야함
        string url = @"C:\Users\TAEWOO\Desktop\1.jpg"; // 사진이 있는 경로
        www = new WWW(temp_url + url);
        Texture2D textuer = www.texture; // 받아온 이미지를 texture에 넣고 사용


        for (int i = 0; i < 3; i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject cube3 = GameObject.CreatePrimitive(PrimitiveType.Cube);


            cube2.transform.parent = cube.transform;
            cube3.transform.parent = cube.transform;
            cube.transform.localScale = new Vector3(0.3f, 1, 1);
            cube2.transform.localScale = new Vector3(0.01f, 1, 1);
            cube3.transform.localScale = new Vector3(0.01f, 1, 1);

            cube2.GetComponent<Renderer>().material.mainTexture = textuer;
            cube3.GetComponent<Renderer>().material.mainTexture = textuer;


            cube2.transform.localPosition = new Vector3(-0.5f, 0, 0);
            cube3.transform.localPosition = new Vector3(0.5f, 0, 0);
            //cube2.transform.Rotate(new Vector3(1, 0, 0), 180);
            //cube3.transform.Rotate(new Vector3(1, 0, 0), 180);
        }



    }

}
