using UnityEngine;

public class BillboardFaceCamera : MonoBehaviour
{
    [SerializeField] Camera targetCamera;
    [SerializeField] bool yawOnly = true;
    [SerializeField] float smooth = 12f;

    // bật cái này nếu thấy UI đang quay lưng
    [SerializeField] bool flip180 = true;

    void LateUpdate()
    {
        if (!targetCamera)
        {
            if (Camera.main) targetCamera = Camera.main;
            else return;
        }

        Vector3 toCam = targetCamera.transform.position - transform.position;

        if (yawOnly)
        {
            toCam.y = 0f;
            if (toCam.sqrMagnitude < 0.0001f) return;

            Quaternion look = Quaternion.LookRotation(toCam.normalized, Vector3.up);

            if (flip180)
                look = look * Quaternion.AngleAxis(180f, Vector3.up); // xoay ngược theo trục Y

            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * smooth);
        }
        else
        {
            // luôn nhìn theo hướng camera
            Quaternion look = Quaternion.LookRotation(targetCamera.transform.forward, targetCamera.transform.up);

            if (flip180)
                look = look * Quaternion.AngleAxis(180f, targetCamera.transform.up); // xoay ngược theo "up" của camera

            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * smooth);
        }
    }
}