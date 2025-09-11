using UnityEngine;

public class LightActivator : MonoBehaviour
{
    [SerializeField] GameObject targetLight;  
    [SerializeField] float activeTime = 5f;   

    public void ActivateLight()
    {
        if (targetLight == null) return;
        StopAllCoroutines();
        StartCoroutine(ActiveCoroutine());
    }

    System.Collections.IEnumerator ActiveCoroutine()
    {
        targetLight.SetActive(true);
        yield return new WaitForSeconds(activeTime);
        targetLight.SetActive(false);
    }
}
