using UnityEngine;
using Mirror;

public class AttackSpawnObject : NetworkBehaviour
{
    public float m_DestroyAfter = 5.0f;
    public float m_Force = 1000;

    public Rigidbody m_Rigidbody_AtkObject;

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), m_DestroyAfter);
    }

    private void Start()
    {
        m_Rigidbody_AtkObject.AddForce(transform.forward * m_Force);
    }

    [Server]//Ŭ�󿡼� ȣ����� �ʵ��� ����
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
