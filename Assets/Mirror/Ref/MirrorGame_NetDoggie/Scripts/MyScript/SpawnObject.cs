using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnObject : NetworkBehaviour
{
    public float m_DestroyAfterTime = 5.0f;
    public float m_MoveForce = 1000f;

    public Rigidbody m_Rigidbody_Object;

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), m_DestroyAfterTime);
    }

    private void Start()
    {
        m_Rigidbody_Object.AddForce(transform.forward * m_MoveForce);
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(this.gameObject);
    }

    [ServerCallback]

    private void OnTriggerEnter(Collider other)
    {
        DestroySelf();
    }
}
