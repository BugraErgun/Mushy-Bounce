using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class EggManager : NetworkBehaviour
{
    public static EggManager instance;

    [SerializeField] private Egg eggPrefab;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        GameManager.onGameStateChanged += GameStateChangedCallBack;
    }



    public override void OnDestroy()
    {
        base.OnDestroy();

        GameManager.onGameStateChanged -= GameStateChangedCallBack;
    }

    private void GameStateChangedCallBack(GameManager.State state)
    {
        switch (state)
        {
            case GameManager.State.Game:
                SpawnEgg();
                break;
        }
    }

    private void SpawnEgg()
    {
        if (!IsServer)
        {
            return;
        }

        Egg eggInstance = Instantiate(eggPrefab, Vector2.up * 5, Quaternion.identity, transform);
        eggInstance.GetComponent<NetworkObject>().Spawn();
        eggInstance.transform.SetParent(transform); 
    }

    public void ReuseEgg()
    {
        if (!IsServer)
        {
            return;
        }

        if (transform.childCount <= 0)
        {
            return;
        }

        transform.GetChild(0).GetComponent<Egg>().Reuse();
    }
}
