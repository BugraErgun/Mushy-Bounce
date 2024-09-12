using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Egg : MonoBehaviour
{

    [Header("Physics Settings")]
    [SerializeField] private float bounceVel;

    public static Action onHit;
    public static Action onFellInWater;

    private float gravityScale;

    private Rigidbody2D rb;

    private bool isAlive = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isAlive = true;

        rb.gravityScale = 0;

        if (GameManager.Instance.connectedPlayers >=3)
        StartCoroutine("WaitAndFall");
    }

    private void Update()
    {
        if (transform.position.y <= -100)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator WaitAndFall()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log(2);

        rb.gravityScale = 1;
    }
    private void Bounce(Vector2 normal)
    {
        rb.velocity = normal * bounceVel;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAlive)
        {
            return;
        }

        if (collision.tag == "Water")
        {
            isAlive = false;
            onFellInWater?.Invoke();
        }
    }

    public void Reuse()
    {
        transform.position = Vector3.up * 5;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.gravityScale = 0;

        isAlive = true;

        StartCoroutine(WaitAndFall());
    }
}
