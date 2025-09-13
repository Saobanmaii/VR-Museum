using UnityEngine;

public class TriggerAllAnimators : MonoBehaviour
{
    [SerializeField] private Animator[] animators; // sẽ hiện array để kéo vào
    [SerializeField] private string triggerName = "hoanthanh";

    public void PlayAll()
    {
        foreach (var anim in animators)
        {
            if (anim != null)
                anim.SetTrigger(triggerName);
        }
    }
}