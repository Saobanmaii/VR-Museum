using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class RandomSound : MonoBehaviour
{
    [SerializeField] float minDelay = 20f;   // 1 phút
    [SerializeField] float maxDelay = 50f;  // 2 phút

    AudioSource audioSrc;

    void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        StartCoroutine(PlayRandomLoop());
    }

    IEnumerator PlayRandomLoop()
    {
        while (true)
        {
            float wait = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(wait);

            if (audioSrc != null && audioSrc.clip != null)
            {
                audioSrc.Play();
            }
        }
    }
}