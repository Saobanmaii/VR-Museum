using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    [SerializeField] string playerTag = "Player";
    [SerializeField] WelcomeUIController welcomeUI;
    [SerializeField] bool disableAfterEnter = true;

    bool fired;

        private void OnTriggerEnter(Collider other)
    {
        if (fired) return;
        if (!other.CompareTag(playerTag)) return;

        fired = true;
        if (welcomeUI) welcomeUI.ShowWelcome();
        else Debug.LogWarning("[ZoneTrigger] Chưa gán WelcomeUIController!");

        if (disableAfterEnter) gameObject.SetActive(false);
    }
}