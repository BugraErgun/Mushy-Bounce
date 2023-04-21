using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerColorizer : NetworkBehaviour
{
    [Header("Elements")]
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer && IsOwner)
        {
            ColorizeServerRpc(Color.red);
        }
    }
    private void Start()
    {

    }
    private void Update()
    {

    }
    [ServerRpc]
    private void ColorizeServerRpc(Color color)
    {
        ColorizeClientRpc(color);       
    }
    [ClientRpc]
    private void ColorizeClientRpc(Color color)
    {
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            renderer.color = color;
        }
    }
}
