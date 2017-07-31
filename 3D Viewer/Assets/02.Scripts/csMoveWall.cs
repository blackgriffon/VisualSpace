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

    private GameObject selecetdObject = null;
    public GameObject SelecetdObject
    {
        get
        {
            return selecetdObject;
        }

        set
        {
            if (selecetdObject != null)
            {
                // 선택이 해재되면
                Destroy(selecetdObject.GetComponent<Outline>());
                networkManager.Send(PacketFactory.MakeDeselect(selecetdObject.name));

            }

            selecetdObject = value;

            if (selecetdObject != null)
            {
                // 선택된거가 있으면
                networkManager.Send(PacketFactory.MakeSelect(selecetdObject.name));
                selecetdObject.AddComponent<Outline>();
            }
        }
    }


    float moveStep = 1f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                SelecetdObject = hit.transform.gameObject;
            }
            else
            {
                SelecetdObject = null;
            }
        }


        if (SelecetdObject != null)
        {


            if (Input.GetKeyDown(KeyCode.W))
            {
                SelecetdObject.transform.Translate(Vector3.forward * moveStep);
                Packet.Header header = new Packet.Header();
                header.ObjectType = Packet.PacketType.WallInfo;
                WallInfo wallInfo = new WallInfo();
                wallInfo.Action = WallInfoAction.MOVE3D;
                wallInfo.Name = SelecetdObject.name;
                wallInfo.PosX = SelecetdObject.transform.position.x;
                wallInfo.PosY = SelecetdObject.transform.position.y;
                wallInfo.PosZ = SelecetdObject.transform.position.z;

                wallInfo.ScaleX = SelecetdObject.transform.localScale.x;
                wallInfo.ScaleY = SelecetdObject.transform.localScale.y;
                wallInfo.ScaleZ = SelecetdObject.transform.localScale.z;
                header.Data = wallInfo;
                networkManager.Send(header);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                SelecetdObject.transform.Translate(Vector3.back * moveStep);
                Packet.Header header = new Packet.Header();
                header.ObjectType = Packet.PacketType.WallInfo;
                WallInfo wallInfo = new WallInfo();
                wallInfo.Action = WallInfoAction.MOVE3D;
                wallInfo.Name = SelecetdObject.name;
                wallInfo.PosX = SelecetdObject.transform.position.x;
                wallInfo.PosY = SelecetdObject.transform.position.y;
                wallInfo.PosZ = SelecetdObject.transform.position.z;

                wallInfo.ScaleX = SelecetdObject.transform.localScale.x;
                wallInfo.ScaleY = SelecetdObject.transform.localScale.y;
                wallInfo.ScaleZ = SelecetdObject.transform.localScale.z;
                header.Data = wallInfo;
                networkManager.Send(header);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                SelecetdObject.transform.Translate(Vector3.left * moveStep);
                Packet.Header header = new Packet.Header();
                header.ObjectType = Packet.PacketType.WallInfo;
                WallInfo wallInfo = new WallInfo();
                wallInfo.Action = WallInfoAction.MOVE3D;
                wallInfo.Name = SelecetdObject.name;
                wallInfo.PosX = SelecetdObject.transform.position.x;
                wallInfo.PosY = SelecetdObject.transform.position.y;
                wallInfo.PosZ = SelecetdObject.transform.position.z;

                wallInfo.ScaleX = SelecetdObject.transform.localScale.x;
                wallInfo.ScaleY = SelecetdObject.transform.localScale.y;
                wallInfo.ScaleZ = SelecetdObject.transform.localScale.z;
                header.Data = wallInfo;
                networkManager.Send(header);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                SelecetdObject.transform.Translate(Vector3.right * moveStep);
                Packet.Header header = new Packet.Header();
                header.ObjectType = Packet.PacketType.WallInfo;
                WallInfo wallInfo = new WallInfo();
                wallInfo.Action = WallInfoAction.MOVE3D;
                wallInfo.Name = SelecetdObject.name;
                wallInfo.PosX = SelecetdObject.transform.position.x;
                wallInfo.PosY = SelecetdObject.transform.position.y;
                wallInfo.PosZ = SelecetdObject.transform.position.z;

                wallInfo.ScaleX = SelecetdObject.transform.localScale.x;
                wallInfo.ScaleY = SelecetdObject.transform.localScale.y;
                wallInfo.ScaleZ = SelecetdObject.transform.localScale.z;
                header.Data = wallInfo;
                networkManager.Send(header);
            }


            else if (Input.GetKey(KeyCode.Delete))
            {


                Debug.Log("Delete 키눌림");
                Packet.Header header = new Packet.Header();
                header.ObjectType = Packet.PacketType.WallInfo;
                WallInfo wallInfo = new WallInfo();
                wallInfo.Action = WallInfoAction.REMOVE3D;
                wallInfo.Name = SelecetdObject.gameObject.name;
                header.Data = wallInfo;
                networkManager.Send(header);

                Destroy(hit.transform.gameObject);

            }


        }


    }
}
