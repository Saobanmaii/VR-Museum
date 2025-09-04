using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    [SerializeField] string playerTag = "Player";
    [SerializeField] WelcomeUIController welcomeUI;
    [SerializeField] bool disableAfterEnter = true;

    bool fired;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (fired) return; 
            Debug.Log("abcd");
            fired = true;
            if (welcomeUI)
                welcomeUI.ShowWelcome();
            if (disableAfterEnter)
                gameObject.SetActive(false);
        }
    }
}