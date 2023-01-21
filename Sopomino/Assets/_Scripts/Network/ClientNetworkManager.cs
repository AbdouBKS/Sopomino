using System;
using UnityEngine;
using Unity.Netcode;

public class ClientNetworkManager : NetworkBehaviour {

    public static ClientNetworkManager Sigleton;

    [SerializeField]
    private GameObject canvasesPrefab;

    public void Awake() {
        Sigleton = this;
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        if (!IsOwner) return;

        var canvases = Instantiate(canvasesPrefab);

        canvases.SetActive(true);
    }
}
