using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PlayerStateManager : NetworkBehaviour
{
    [Header("Elements")]
    [SerializeField] private SpriteRenderer[] spriteRenderers;
    [SerializeField] private Collider2D coll;

    public void Enable()
    {
        EnableClientRPC();
    }

    [ClientRpc]
    private void EnableClientRPC()
    {
        coll.enabled = true;
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            Color color = renderer.color;
            color.a = 1f;
            renderer.color = color;

        }
    }
    public void Disable()
    {
        DisableClientRPC();
    }

    [ClientRpc]
    private void DisableClientRPC()
    {
        coll    .enabled = false;
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            Color color = renderer.color;
            color.a = .2f;
            renderer.color = color;

        }
    }
}
