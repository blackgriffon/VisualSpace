using cakeslice;
using Packet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CustomEditor() 애트리뷰트를 사용해서 어떤 타입을 커스터마이즈 할 것인지를 명시해 주어야 한다.
//[CustomEditor(typeof(Movement))]
public class csMoveWall : MonoBehaviour
{


    private Ray ray;
    RaycastHit hit = new RaycastHit();
    csTCPClientManager networkManager;

    public float Speed = 100;


    private void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<csTCPClientManager>();
    }


    bool bFocus = true;
    private void OnApplicationFocus(bool focus)
    {

        if (!focus)
            bFocus = false;

    }



    // 게임 오브젝트가 선택되었을 때, 씬 윈도우에 표시되는 UI나 여러 표식들은 이곳에서 그려줍니다.
    //public void OnSceneGUI()
    //{

    //    if (hit.transform != null)
    //    {
    //        Movement _movement = hit.transform as Movement;
    //        // 타겟 지점에 붉은색의 큐브 생성
    //        Handles.color = Color.red;
    //        Handles.CubeCap(0, _movement._targetPosition, Quaternion.identity, 1.0f);

    //        // 타겟 지점과 게임 오브젝트를 녹색 줄로 이어주겠습니다.
    //        Handles.color = Color.green;
    //        Handles.DrawLine(_movement.gameObject.transform.position, _movement._targetPosition);

    //        // UI
    //        Handles.BeginGUI();
    //        {
    //            // 타겟 지점으로 게임 오브젝트를 이동시켜 주는 버튼 표시
    //            if (_movement.gameObject.transform.position != _movement._targetPosition)
    //            {
    //                // 3D 좌표를 2D 좌표로 변경해서 큐브보다 약간 더 위에 출력시켜 준다.
    //                Vector2 button_positoin = HandleUtility.WorldToGUIPoint(_movement._targetPosition);
    //                Rect button_rect = new Rect(button_positoin.x, button_positoin.y - 40, 200, 20);
    //                if (GUI.Button(button_rect, "Move To Target Position"))
    //                    _movement.gameObject.transform.position = _movement._targetPosition;
    //            }
    //        }
    //        Handles.EndGUI();
    //    }
    //}



    GameObject selecetdObject;
    
    float moveStep = 1f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 포커스가 아닌상태였다가 클릭했다면
            //if(!bFocus)
            //{
            //    bFocus = true;
            //    return;
            //}
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (selecetdObject != null)
            {
                Destroy(selecetdObject.GetComponent<Outline>());
                selecetdObject = null;
            }


            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                Debug.Log(hit.transform.gameObject.name);
                selecetdObject = hit.transform.gameObject;
                selecetdObject.AddComponent<Outline>();
            }

        }


        if (selecetdObject != null)
        {


            if (Input.GetKeyDown(KeyCode.W))
            {
                selecetdObject.transform.Translate(Vector3.forward * moveStep);
                Packet.Header header = new Packet.Header();
                header.ObjectType = Packet.PacketType.WallInfo;
                WallInfo wallInfo = new WallInfo();
                wallInfo.Action = WallInfoAction.MOVE3D;
                wallInfo.Name = selecetdObject.name;
                wallInfo.PosX = selecetdObject.transform.position.x;
                wallInfo.PosY = selecetdObject.transform.position.y;
                wallInfo.PosZ = selecetdObject.transform.position.z;

                wallInfo.ScaleX = selecetdObject.transform.localScale.x;
                wallInfo.ScaleY = selecetdObject.transform.localScale.y;
                wallInfo.ScaleZ = selecetdObject.transform.localScale.z;
                header.Data = wallInfo;
                networkManager.Send(header);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                selecetdObject.transform.Translate(Vector3.back * moveStep);
                Packet.Header header = new Packet.Header();
                header.ObjectType = Packet.PacketType.WallInfo;
                WallInfo wallInfo = new WallInfo();
                wallInfo.Action = WallInfoAction.MOVE3D;
                wallInfo.Name = selecetdObject.name;
                wallInfo.PosX = selecetdObject.transform.position.x;
                wallInfo.PosY = selecetdObject.transform.position.y;
                wallInfo.PosZ = selecetdObject.transform.position.z;

                wallInfo.ScaleX = selecetdObject.transform.localScale.x;
                wallInfo.ScaleY = selecetdObject.transform.localScale.y;
                wallInfo.ScaleZ = selecetdObject.transform.localScale.z;
                header.Data = wallInfo;
                networkManager.Send(header);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                selecetdObject.transform.Translate(Vector3.left * moveStep);
                Packet.Header header = new Packet.Header();
                header.ObjectType = Packet.PacketType.WallInfo;
                WallInfo wallInfo = new WallInfo();
                wallInfo.Action = WallInfoAction.MOVE3D;
                wallInfo.Name = selecetdObject.name;
                wallInfo.PosX = selecetdObject.transform.position.x;
                wallInfo.PosY = selecetdObject.transform.position.y;
                wallInfo.PosZ = selecetdObject.transform.position.z;

                wallInfo.ScaleX = selecetdObject.transform.localScale.x;
                wallInfo.ScaleY = selecetdObject.transform.localScale.y;
                wallInfo.ScaleZ = selecetdObject.transform.localScale.z;
                header.Data = wallInfo;
                networkManager.Send(header);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                selecetdObject.transform.Translate(Vector3.right * moveStep);
                Packet.Header header = new Packet.Header();
                header.ObjectType = Packet.PacketType.WallInfo;
                WallInfo wallInfo = new WallInfo();
                wallInfo.Action = WallInfoAction.MOVE3D;
                wallInfo.Name = selecetdObject.name;
                wallInfo.PosX = selecetdObject.transform.position.x;
                wallInfo.PosY = selecetdObject.transform.position.y;
                wallInfo.PosZ = selecetdObject.transform.position.z;

                wallInfo.ScaleX = selecetdObject.transform.localScale.x;
                wallInfo.ScaleY = selecetdObject.transform.localScale.y;
                wallInfo.ScaleZ = selecetdObject.transform.localScale.z;
                header.Data = wallInfo;
                networkManager.Send(header);
            }


            // 1번 눌릴때 마다 1씩 이동
            //float v = Input.GetAxis("Vertical");
            //float h = Input.GetAxis("Horizontal");

            //if (v != 0 || h != 0)
            //{

            //    hit.transform.gameObject.transform.Translate(h * Vector3.right * Speed * Time.deltaTime);
            //    hit.transform.gameObject.transform.Translate(v * Vector3.forward * Speed * Time.deltaTime);
            //    // Packet.Header header = new Packet.Header();
            //    Packet.Header header = new Packet.Header();
            //    header.ObjectType = Packet.PacketType.WallInfo;
            //    WallInfo wallInfo = new WallInfo();
            //    wallInfo.Action = WallInfoAction.MOVE3D;
            //    wallInfo.Name = hit.transform.gameObject.name;
            //    wallInfo.PosX = hit.transform.gameObject.transform.position.x;
            //    wallInfo.PosY = hit.transform.gameObject.transform.position.y;
            //    wallInfo.PosZ = hit.transform.gameObject.transform.position.z;

            //    wallInfo.ScaleX = hit.transform.gameObject.transform.localScale.x;
            //    wallInfo.ScaleY = hit.transform.gameObject.transform.localScale.y;
            //    wallInfo.ScaleZ = hit.transform.gameObject.transform.localScale.z;

            //    header.Data = wallInfo;

            //    networkManager.Send(header);
            //}

            else if (Input.GetKey(KeyCode.Delete))
            {


                Debug.Log("Delete 키눌림");
                Packet.Header header = new Packet.Header();
                header.ObjectType = Packet.PacketType.WallInfo;
                WallInfo wallInfo = new WallInfo();
                wallInfo.Action = WallInfoAction.REMOVE3D;
                wallInfo.Name = selecetdObject.gameObject.name;
                header.Data = wallInfo;
                networkManager.Send(header);

                Destroy(hit.transform.gameObject);
        
            }


        }


    }
}
