using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class Egg : MonoBehaviour
{
    [Header("Physics Settings")]
    [SerializeField] private float bounceVel;
    private Rigidbody2D rb;
    private bool isAlive = true;
    private float gravity;


    [Header("Events")]
    public static Action onHit;
    public static Action onFellInWater;



    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isAlive = true;

        gravity = rb.gravityScale;
        rb.gravityScale = 0;
        
        StartCoroutine("WaitAndFall");
    }
    IEnumerator WaitAndFall()
    {
        yield return new WaitForSeconds(2f);
        rb.gravityScale = gravity;

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAlive)
        {
            return;
        }
        if (collision.collider.TryGetComponent(out PlayerController playerController))
        {
            Bounce(collision.GetContact(0).normal);
            playerController.Bump();
            onHit?.Invoke();
        }
    }
    private void Bounce(Vector2 normal)
    {
        rb.velocity = normal * bounceVel;
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isAlive)
        {
            return;
        }
        if (collider.CompareTag("Water"))
        {
            isAlive = false;
            onFellInWater?.Invoke();
        }
    }
    public void Reuse()
    {
        transform.position = Vector2.up * 5;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.gravityScale = 0;

        isAlive = true;

        StartCoroutine("WaitAndFall");
    }
}
