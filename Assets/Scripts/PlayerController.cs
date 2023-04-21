using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerController : NetworkBehaviour
{
    [Header("Control Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxX;

    private float clickedScreenX; 
    private float clickedPlayerX;

    [Header("Animations")]
    [SerializeField] private Animator animator;
    [SerializeField] private Animator bodyAnimator;
    [SerializeField] private float animatorSpeedMultiplier;
    [SerializeField] private float animatorSpeedLerp;

    [Header("Events")]
    public static Action onBump;

    void Start()
    {
        if (IsOwner)
        {
            animator.Play("Idle");
        }
        
    }

    void Update()
    {
        if (IsOwner)
        {
            ManageControl();
        }
        
    }
    private void ManageControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickedScreenX = Input.mousePosition.x;
            clickedPlayerX = transform.position.x;

            animator.speed = 1;
        }
        else if(Input.GetMouseButton(0))
        {
            float xDiffrence = Input.mousePosition.x - clickedScreenX;
            xDiffrence /= Screen.width;
            xDiffrence *= moveSpeed;

            float newXposition = clickedPlayerX + xDiffrence;

            newXposition = Mathf.Clamp(newXposition, -maxX, maxX); 

            transform.position = new Vector2(newXposition, transform.position.y);

            UpdatePlayerAnimation();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            animator.speed = 1;
            animator.Play("Idle");
        }
    }
    private float previusScreenX;

    private void UpdatePlayerAnimation()
    {
        float xDiffrence = (Input.mousePosition.x - previusScreenX) / Screen.width;

        xDiffrence *= animatorSpeedMultiplier;
        xDiffrence = Mathf.Abs(xDiffrence);

        float targetAnimatorSpeed = Mathf.Lerp(animator.speed, xDiffrence, Time.deltaTime * animatorSpeedLerp);

        animator.speed = targetAnimatorSpeed;
        animator.Play("Run");

        previusScreenX = Input.mousePosition.x;
    }
    public void Bump()
    {
        BumpClientRPC();
    }

    [ClientRpc]
    private void BumpClientRPC()
    {
        bodyAnimator.Play("Bump");
        onBump?.Invoke();
    }
    
}
    