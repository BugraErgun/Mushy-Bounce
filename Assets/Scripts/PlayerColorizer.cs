using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerColorizer : NetworkBehaviour
{
    [Header("Elements")]
    [SerializeField] private SpriteRenderer[] renderers;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer && IsOwner)
        {
            ColorizeServerRPC(Color.red);
        }
    }

    [ServerRpc]
    private void ColorizeServerRPC(Color color)
    {
        ColorizeClientRPC(color);
    }

    [ClientRpc]
    private void ColorizeClientRPC(Color color)
    {
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.color = color;
        }
    }
}
