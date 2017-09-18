using cakeslice;
using WpfUnityPacket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;


public class csObjetControlManager : MonoBehaviour
{

    private Ray ray;
    RaycastHit hit = new RaycastHit();
    public csWpfUnityTCPClientManager networkManager;
    public GameObject controlButtons = null;
    public Canvas UiCanvas = null;
    GraphicRaycaster gr;
    PointerEventData ped;


    public Camera MainCamera;
    private Vector3 InitialCameraPos;
    private Quaternion InitialCameraRoation;
    float moveStep = 0.5f;


    private void Awake()
    {
        controlButtons.SetActive(false);
        gr = UiCanvas.GetComponent<GraphicRaycaster>();
        ped = new PointerEventData(null);


        InitialCameraPos = MainCamera.transform.position;
        InitialCameraRoation = MainCamera.transform.rotation;

    }

    private void Update()
    {
        // 0.마우스 왼쪽이 눌리면
        if (Input.GetMouseButtonDown(0))
        {

            // 1. 버튼이 눌렸는지를 먼저 검사한다.
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>(); // 여기에 히트 된 개체 저장 
            gr.Raycast(ped, results);

            if (processButtonEvent(results))
                return;

            // 1-1.버튼이 눌렸으면 오브젝트가 눌린것이 아니므로 return 한다.



            // 2.버튼이 안눌렸으면 오브젝트가 눌렸는지 검사한다.
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // 2-1.SupportObject는 나중에 움직이면 안되는 오브젝트를 추가할 수 있으니 미리 조건을 걸어놨다.
            if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.transform.gameObject.tag != "SupportObject")
            {

                // 전에 선택된것이 있으면 해제 후
                if (selecetdObject != null)
                {
                    switch (selecetdObject.tag)
                    {
                        case "Wall":
                            networkManager.Send(PacketFactory.MakeDeselect(selecetdObject.name, WpfUnityPacketType.WallInfoPacket));
                            break;

                        case "Object":
                            networkManager.Send(PacketFactory.MakeDeselect(selecetdObject.name, WpfUnityPacketType.ObjectInfoPacket));
                            break;

                        case "Floor":
                            networkManager.Send(PacketFactory.MakeDeselect(selecetdObject.name, WpfUnityPacketType.FloorInfoPacket));
                            break;
                    }

                }


                // 2-2.레이캐스트가 되면, SelecetdObject에 선택된 오브젝트를 대입한다.
                SelecetdObject = hit.transform.gameObject;
                // 테그에 따라서 2D 도면에 선택되었다고 메세지를 보낸다.
                switch (selecetdObject.tag)
                {
                    case "Wall":
                        networkManager.Send(PacketFactory.MakeSelect(selecetdObject.name, WpfUnityPacketType.WallInfoPacket));
                        break;

                    case "Object":
                        networkManager.Send(PacketFactory.MakeSelect(selecetdObject.name, WpfUnityPacketType.ObjectInfoPacket));
                        break;

                    case "Floor":
                        networkManager.Send(PacketFactory.MakeSelect(selecetdObject.name, WpfUnityPacketType.FloorInfoPacket));
                        break;
                }
            }

            else
            {
                // 테그에 따라서 2D 도면에 선택이 해제되었다고 메세지를 보낸다.
                if (selecetdObject != null)
                {
                    switch (selecetdObject.tag)
                    {
                        case "Wall":
                            networkManager.Send(PacketFactory.MakeDeselect(selecetdObject.name, WpfUnityPacketType.WallInfoPacket));
                            break;

                        case "Object":
                            networkManager.Send(PacketFactory.MakeDeselect(selecetdObject.name, WpfUnityPacketType.ObjectInfoPacket));
                            break;

                        case "Floor":
                            networkManager.Send(PacketFactory.MakeDeselect(selecetdObject.name, WpfUnityPacketType.FloorInfoPacket));
                            break;
                    }

                }

                // 2-3.레이캐스트가 안되거나 선택이 해지되었지 때문에, SelecetdObject에 null을 넣은다.
                SelecetdObject = null;

            }
        }
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
            // set이 작동하면 SelecetdObject에 값을 대입하면)
            // 우선 전에 선택된 오브젝트가 있었는지 확인한다.
            if (selecetdObject != null)
            {
                // 선택된 오브젝트가 있었으면
                // 컨트롤 버튼을 비활성화 시켜서 안보이게 한다.
                controlButtons.SetActive(false);

                // 오브젝트들은(ex가구) 자식들의 집합으로 이루워진 경우가 있으므로
                var child = selecetdObject.GetComponentsInChildren<Transform>();

                // 선택되었을때 넣어주었던 cakeslice.Outline 컴포넌트를 제거한다.
                for (int i = 0; i < child.Length; i++)
                {
                    if (child[i].gameObject.GetComponent<cakeslice.Outline>() != null)
                        Destroy(child[i].gameObject.GetComponent<cakeslice.Outline>());
                }

            }


            // SelecetdObject에 들어온 값을 대입한다.
            selecetdObject = value;




            // 선택된거가 있으면
            if (selecetdObject != null)
            {
                // 컨트롤 버튼을 활성화 시켜서 보이게 한다.
                controlButtons.SetActive(true);


                // cakeslice.Outline 컴포넌트를를 MeshRenderer가 있는 자식들에서 모두 추가한다.
                // cakeslice.Outline를 추가하면 자동적으로 선택된것 처럼 보인다.
                var child = selecetdObject.GetComponentsInChildren<Transform>();

                for (int i = 0; i < child.Length; i++)
                {
                    if (child[i].gameObject.GetComponent<MeshRenderer>() != null)
                        child[i].gameObject.AddComponent<cakeslice.Outline>();
                }

            }

        }
    }







    IEnumerator coCheckInput()
    {
        while (true)
        {

            // 0.마우스 왼쪽이 눌리면
            if (Input.GetMouseButtonDown(0))
            {

                // 1. 버튼이 눌렸는지를 먼저 검사한다.
                ped.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>(); // 여기에 히트 된 개체 저장 
                gr.Raycast(ped, results);

                if (processButtonEvent(results))
                {
                    // 1-1.버튼이 눌렸으면 오브젝트가 눌린것이 아니므로 contiue를 한다.
                    yield return null;
                    continue;
                }

                // 2.버튼이 안눌렸으면 오브젝트가 눌렸는지 검사한다.
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // 2-1.SupportObject는 나중에 움직이면 안되는 오브젝트를 추가할 수 있으니 미리 조건을 걸어놨다.
                if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.transform.gameObject.tag != "SupportObject")
                {

                    // 전에 선택된것이 있으면 해제 후
                    if (selecetdObject != null)
                    {
                        switch (selecetdObject.tag)
                        {
                            case "Wall":
                                networkManager.Send(PacketFactory.MakeDeselect(selecetdObject.name, WpfUnityPacketType.WallInfoPacket));
                                break;

                            case "Object":
                                networkManager.Send(PacketFactory.MakeDeselect(selecetdObject.name, WpfUnityPacketType.ObjectInfoPacket));
                                break;

                            case "Floor":
                                networkManager.Send(PacketFactory.MakeDeselect(selecetdObject.name, WpfUnityPacketType.FloorInfoPacket));
                                break;
                        }

                    }


                    // 2-2.레이캐스트가 되면, SelecetdObject에 선택된 오브젝트를 대입한다.
                    SelecetdObject = hit.transform.gameObject;
                    // 테그에 따라서 2D 도면에 선택되었다고 메세지를 보낸다.
                    switch (selecetdObject.tag)
                    {
                        case "Wall":
                            networkManager.Send(PacketFactory.MakeSelect(selecetdObject.name, WpfUnityPacketType.WallInfoPacket));
                            break;

                        case "Object":
                            networkManager.Send(PacketFactory.MakeSelect(selecetdObject.name, WpfUnityPacketType.ObjectInfoPacket));
                            break;

                        case "Floor":
                            networkManager.Send(PacketFactory.MakeSelect(selecetdObject.name, WpfUnityPacketType.FloorInfoPacket));
                            break;
                    }
                }

                else
                {
                    // 테그에 따라서 2D 도면에 선택이 해제되었다고 메세지를 보낸다.
                    if (selecetdObject != null)
                    {
                        switch (selecetdObject.tag)
                        {
                            case "Wall":
                                networkManager.Send(PacketFactory.MakeDeselect(selecetdObject.name, WpfUnityPacketType.WallInfoPacket));
                                break;

                            case "Object":
                                networkManager.Send(PacketFactory.MakeDeselect(selecetdObject.name, WpfUnityPacketType.ObjectInfoPacket));
                                break;

                            case "Floor":
                                networkManager.Send(PacketFactory.MakeDeselect(selecetdObject.name, WpfUnityPacketType.FloorInfoPacket));
                                break;
                        }

                    }

                    // 2-3.레이캐스트가 안되거나 선택이 해지되었지 때문에, SelecetdObject에 null을 넣은다.
                    SelecetdObject = null;

                }
            }



            // 오브젝트가 선택되었을때만 이동할 수 있도록한다.(현재는 안쓰임)
            //if (SelecetdObject != null)
            //{


            //    if (Input.GetKeyDown(KeyCode.W))
            //    {
            //        MoveBack();
            //    }
            //    else if (Input.GetKeyDown(KeyCode.S))
            //    {
            //        MoveForword();
            //    }
            //    else if (Input.GetKeyDown(KeyCode.A))
            //    {
            //        MoveLeft();
            //    }
            //    else if (Input.GetKeyDown(KeyCode.D))
            //    {
            //        MoveRight();
            //    }


            //    else if (Input.GetKey(KeyCode.Delete))
            //    {

            //        DeleteObject();

            //    }


            //}
            yield return null;
        }

    }


    bool processButtonEvent(List<RaycastResult> results)
    {
        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.CompareTag("Button"))
            {
                onButtonClickEvent(results[i].gameObject.name);
                return true;
            }
        }

        return false;
    }

    private void onButtonClickEvent(string name)
    {
        switch (name)
        {
            case "delete":
                DeleteObject();
                break;

            case "back":
                MoveBack();
                break;

            case "forward":
                MoveForword();
                break;

            case "right":
                MoveRight();
                break;

            case "left":
                MoveLeft();
                break;

            case "CameraPosReset":
                ResetPosition();
                break;


        }
    }

    private void ResetPosition()
    {
        MainCamera.transform.position = InitialCameraPos;
        MainCamera.transform.rotation = InitialCameraRoation;
    }

    public void MoveObject(Vector3 moveValue)
    {
        SelecetdObject.transform.position += moveValue;
        //SelecetdObject.transform.Translate(Vector3.left * moveStep);


        switch (SelecetdObject.tag)
        {
            case "Wall":


                WallInfoPacket wallInfo = new WallInfoPacket();
                wallInfo.Command = WpfUnityCommand.MOVE3D;
                wallInfo.Name = SelecetdObject.name;
                wallInfo.PosX = SelecetdObject.transform.position.x;
                wallInfo.PosY = SelecetdObject.transform.position.y;
                wallInfo.PosZ = SelecetdObject.transform.position.z;

                wallInfo.ScaleX = SelecetdObject.transform.localScale.x;
                wallInfo.ScaleY = SelecetdObject.transform.localScale.y;
                wallInfo.ScaleZ = SelecetdObject.transform.localScale.z;
                networkManager.Send(wallInfo);
                break;

            case "Object":

                ObjectInfoPacket objectInfo = new ObjectInfoPacket();
                objectInfo.Command = WpfUnityCommand.MOVE3D;
                objectInfo.Name = SelecetdObject.name;
                objectInfo.PosX = SelecetdObject.transform.position.x;
                objectInfo.PosY = SelecetdObject.transform.position.y;
                objectInfo.PosZ = SelecetdObject.transform.position.z;
                networkManager.Send(objectInfo);
                break;


            case "Floor":
                FloorInfoPacket floorInfo = new FloorInfoPacket();
                floorInfo.Command = WpfUnityCommand.MOVE3D;
                floorInfo.Name = SelecetdObject.name;
                floorInfo.PosX = SelecetdObject.transform.position.x;
                floorInfo.PosY = SelecetdObject.transform.position.y;
                floorInfo.PosZ = SelecetdObject.transform.position.z;

                floorInfo.ScaleX = SelecetdObject.transform.localScale.x;
                floorInfo.ScaleY = SelecetdObject.transform.localScale.y;
                floorInfo.ScaleZ = SelecetdObject.transform.localScale.z;
                networkManager.Send(floorInfo);
                break;


        }
    }


    public void MoveLeft()
    {
        MoveObject(Vector3.left * moveStep);
    }


    public void MoveBack()
    {
        MoveObject(Vector3.forward * moveStep);
    }

    public void MoveRight()
    {
        MoveObject(Vector3.right * moveStep);
    }

    public void MoveForword()
    {

        MoveObject(Vector3.back * moveStep);
    }

    public void DeleteObject()
    {
        WpfUnityPacketHeader header = null;
        switch (SelecetdObject.tag)
        {


            case "Wall":
                WallInfoPacket wallInfo = new WallInfoPacket();
                wallInfo.Command = WpfUnityCommand.REMOVE3D;
                wallInfo.Name = SelecetdObject.gameObject.name;
                header = wallInfo;
                break;

            case "Object":
                ObjectInfoPacket objectInfo = new ObjectInfoPacket();
                objectInfo.Command = WpfUnityCommand.REMOVE3D;
                objectInfo.Name = SelecetdObject.gameObject.name;
                header = objectInfo;
                break;

            case "Floor":
                FloorInfoPacket floorInfo = new FloorInfoPacket();
                floorInfo.Command = WpfUnityCommand.REMOVE3D;
                floorInfo.Name = SelecetdObject.gameObject.name;
                header = floorInfo;
                break;
        }


        networkManager.Send(header);
        Destroy(SelecetdObject.gameObject);
        SelecetdObject = null;

    }
}
