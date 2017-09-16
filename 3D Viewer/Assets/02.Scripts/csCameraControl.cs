using UnityEngine;
using System.Collections;

public class csCameraControl : MonoBehaviour
{

    public float turnSpeed = 4.0f;      // 카메라 회전속도
    public float panSpeed = 4.0f;      // 카메라 수평회전속도
    public float zoomSpeed = 4.0f;      // 카메라 확대 축소(전진 후진) 속도

    private Vector3 mouseOrigin;   // 마우스 드래그시 커서 위치
    private bool isPanning;      // 카메라 수평회전
    private bool isRotating;   // 카메라 회전
    private bool isZooming;      // 카메라 줌인

    
    void Update()
    {

        // Get the left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            // 초기값
            mouseOrigin = Input.mousePosition;
            isRotating = true;
        }

        // 마우스 오른쪽 클릭시 이벤트
        if (Input.GetMouseButtonDown(1))
        {
            // 초기값
            mouseOrigin = Input.mousePosition;
            isPanning = true;
        }

        // 마우스 휠
        if (Input.GetMouseButtonDown(2))
        {
            // 초기값
            mouseOrigin = Input.mousePosition;
            isZooming = true;
        }

        // 버튼 누를시 움직임 정지(다른부위들)
        if (!Input.GetMouseButton(0)) isRotating = false;
        if (!Input.GetMouseButton(1)) isPanning = false;
        if (!Input.GetMouseButton(2)) isZooming = false;

        // x축, y축 카메라 회전
        if (isRotating)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            transform.RotateAround(transform.position, transform.right, -pos.y * turnSpeed);
            transform.RotateAround(transform.position, Vector3.up, pos.x * turnSpeed);
        }

        // 카메라 수평이동
        if (isPanning)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);
            transform.Translate(move, Space.Self);
        }

        // z축기준으로 선행이동
        if (isZooming)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            Vector3 move = pos.y * zoomSpeed * transform.forward;
            transform.Translate(move, Space.World);
        }


    }


}