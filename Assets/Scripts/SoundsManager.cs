using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    [SerializeField] private AudioSource bumpSound;


    private void Start()
    {
        PlayerController.onBump += PlayBump;
    }

    private void OnDestroy()
    {
        PlayerController.onBump -= PlayBump;
    }
    public void PlayBump()
    {
        bumpSound.Play();
    }
}
