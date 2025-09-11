using UnityEngine;

public class TaskCompleteHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;   // Kéo Animator vào đây
    [SerializeField] private AudioSource audioSource; // Kéo AudioSource vào đây
    [SerializeField] private AudioClip completeClip;  // Kéo file âm thanh vào đây

    [Header("Animator Settings")]
    [SerializeField] private string triggerName = "hoanthanh"; // tên trigger trong Animator

    // Hàm này bạn gọi từ event khi nhiệm vụ hoàn thành
    public void OnTaskCompleted()
    {
        // Gọi anim
        if (animator)
            animator.SetTrigger(triggerName);

        // Phát âm thanh
        if (audioSource && completeClip)
            audioSource.PlayOneShot(completeClip);
    }
}
