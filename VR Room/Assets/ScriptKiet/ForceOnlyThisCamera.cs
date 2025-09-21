using UnityEngine;
using System.Linq;

public class ForceOnlyThisCamera : MonoBehaviour
{
    [SerializeField] Camera xrCam; // kéo Camera dùng cho kính vào đây

    void Awake()
    {
        var all = FindObjectsOfType<Camera>(true);
        foreach (var c in all)
        {
            c.enabled = (c == xrCam);
            if (c != xrCam) c.gameObject.SetActive(false); // mạnh tay để loại trừ
        }

        // Chốt vài thứ an toàn:
        if (xrCam)
        {
            xrCam.targetTexture = null;
            xrCam.stereoTargetEye = StereoTargetEyeMask.Both;
            xrCam.clearFlags = CameraClearFlags.Skybox; // hoặc SolidColor đen
            xrCam.cullingMask = ~0; // tạm thời bật hết để test
        }
    }
}