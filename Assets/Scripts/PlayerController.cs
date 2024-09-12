using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Header("Control Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxX;

    [SerializeField] private Animator animator;
    [SerializeField] private Animator bodyAnimator;
    [SerializeField] private float animatorSpeedMultiplier;
    [SerializeField] private float animatorSpeedLerp;

    public static Action onBump;

    private float clickedScreenX;
    private float clickedPlayerX;


    private void Start()
    {
        if (IsOwner)
            animator.Play("Idle");
    }
    private void Update()
    {
        if (IsOwner)
            ManageControl();
    }

    private void ManageControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickedScreenX = Input.mousePosition.x;
            clickedPlayerX = transform.position.x;

            animator.speed = 1;

        }
        else if (Input.GetMouseButton(0))
        {
            float xDifference = Input.mousePosition.x - clickedScreenX;
            xDifference /= Screen.width;
            xDifference *= moveSpeed;

            float newXpos = clickedPlayerX + xDifference;

            newXpos = Math.Clamp(newXpos, -maxX, maxX);

            transform.position = new Vector2(newXpos, transform.position.y);
            UpdateAnimation();

            Debug.Log("x difference" + xDifference);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            animator.speed = 1;
            animator.Play("Idle");
        }
    }

    private float previousScreenX;
   
    private void UpdateAnimation()
    {
        float xDiff = (Input.mousePosition.x - previousScreenX) / Screen.width;
        xDiff *= animatorSpeedMultiplier;

        xDiff = Math.Abs(xDiff);

        float targetAnimatorSpeed = Mathf.Lerp(animator.speed, xDiff, Time.deltaTime * animatorSpeedLerp);

        animator.speed = targetAnimatorSpeed;
        animator.Play("Run");
        

        previousScreenX = Input.mousePosition.x;
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
