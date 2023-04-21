using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private AudioSource bumpSounce;

    void Start()
    {
        PlayerController.onBump += PlayBumpSound;
    }

    private void OnDestroy()
    {
        PlayerController.onBump -= PlayBumpSound;

    }
    public void PlayBumpSound()
    {
        bumpSounce.Play();
    }
}
