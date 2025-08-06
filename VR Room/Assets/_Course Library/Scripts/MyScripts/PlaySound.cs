using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
    // Start is called before the first frame update
    {
    public AudioSource audioSource;  // Gắn AudioSource sẵn trong Inspector
    public AudioClip clipToPlay;     // Kéo file .wav/.mp3 vào đây

    public void PlaySoundd()
    {
        if (audioSource != null && clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
    }
}
