using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SphereCollider))]
public class LampTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Light coreLight;       // ánh sáng chính
    [SerializeField] Light fillLight;       // ánh sáng phụ
    [SerializeField] ParticleSystem fireFX; // hiệu ứng lửa

    [Header("Settings")]
    [SerializeField] string playerTag = "Player";
    [SerializeField] float fadeDuration = 1.5f;

    // giá trị sáng tối đa
    float coreTargetIntensity = 1.5f;
    float fillTargetIntensity = 0.6f;

    // cache
    float coreInitIntensity, fillInitIntensity;
    SphereCollider trigger;

    void Awake()
    {
        trigger = GetComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 3f; // vùng kích hoạt

        // lưu giá trị
        coreInitIntensity = coreTargetIntensity;
        fillInitIntensity = fillTargetIntensity;

        // khởi tạo tắt
        if (coreLight) coreLight.intensity = 0f;
        if (fillLight) fillLight.intensity = 0f;
        if (fireFX) fireFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (coreLight)
            coreLight.DOIntensity(coreInitIntensity, fadeDuration);

        if (fillLight)
            fillLight.DOIntensity(fillInitIntensity, fadeDuration);

        if (fireFX && !fireFX.isPlaying)
            fireFX.Play();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (coreLight)
            coreLight.DOIntensity(0f, fadeDuration);

        if (fillLight)
            fillLight.DOIntensity(0f, fadeDuration);

        if (fireFX && fireFX.isPlaying)
            fireFX.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}