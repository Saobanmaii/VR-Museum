using UnityEngine;

public class WheelRotator : MonoBehaviour
{
    [Header("Spin Settings")]
    [SerializeField] float spinSpeed = 360f;   // độ/giây
    [SerializeField] float spinDuration = 2f;  // thời gian quay

    bool spinning;

    public void RotateWheel()
    {
        if (!spinning)
            StartCoroutine(SpinCoroutine());
    }

    System.Collections.IEnumerator SpinCoroutine()
    {
        spinning = true;
        float t = 0f;
        while (t < spinDuration)
        {
            float dt = Time.deltaTime;
            transform.Rotate(0f, 0f, spinSpeed * dt, Space.Self);
            t += dt;
            yield return null;
        }
        spinning = false;
    }
}