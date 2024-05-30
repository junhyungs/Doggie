using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class NetSpawnPlayerObject : NetworkBehaviour
{
    [Header("Components")]
    public NavMeshAgent m_NaveAgent_Player;
    public Animator m_Animator_Player;
    public TextMesh m_TextMesh_HealthBar;
    public TextMesh m_TextMesh_NetType;
    public Transform m_Transform_Player;

    [Header("Movement")]
    public float m_RotationSpeed = 100f;

    [Header("Attack")]
    public KeyCode m_AtkKey = KeyCode.Space;
    public GameObject m_AttackObject;
    public Transform m_Transform_AtkSpawnPos;

    [Header("Stats Server")]
    [SyncVar] public int m_Health = 4;

    private float Horizontal;
    private float Vertical;
    private Vector3 forward;
    
    //StartUpdate-------------------------------------------------------------------------
    public void Update()
    {
        string netType = isClient ? "Å¬¶ó" : "Å¬¶ó¾Æ´Ô";

        m_TextMesh_NetType.text = this.isLocalPlayer ? $"[·ÎÄÃ/{netType}]" : $"[·ÎÄÃ¾Æ´Ô/{netType}]{this.netId}";

        SetHealthBarOnUpdate(m_Health);

        if(CheckIsForcusedOnUpdate() == false)
        {
            return;
        }

        CheckIsLocalPlayerOnUpdate();
    }

    private void SetHealthBarOnUpdate(int health)
    {
        m_TextMesh_HealthBar.text = health.ToString();
    }
    private bool CheckIsForcusedOnUpdate()
    {
        return Application.isFocused;
    }
    private void CheckIsLocalPlayerOnUpdate()
    {
        if (this.isLocalPlayer == false)
            return;

        Horizontal = Input.GetAxis("Horizontal");
        transform.Rotate(0, Horizontal * m_RotationSpeed * Time.deltaTime, 0);

        Vertical = Input.GetAxis("Vertical");
        forward = transform.TransformDirection(Vector3.forward);
        m_NaveAgent_Player.velocity = forward * Mathf.Max(Vertical, 0) * m_NaveAgent_Player.speed;
        m_Animator_Player.SetBool("Moving", m_NaveAgent_Player.velocity != Vector3.zero);

        if (Input.GetKeyDown(m_AtkKey))
        {
            CommandAttack();
        }

        RotateLocalPlayer();
    }

    private void RotateLocalPlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            Debug.DrawRay(ray.origin, hit.point, Color.blue);
            Vector3 lookRotate = new Vector3(hit.point.x, m_Transform_Player.position.y, hit.point.z);
            m_Transform_Player.LookAt(lookRotate);
        }
    }
    //EndUpdate---------------------------------------------------------------------------

    [Command]
    private void CommandAttack()
    {
        GameObject attackObjectForSpawn = Instantiate(m_AttackObject, m_Transform_AtkSpawnPos.position, m_Transform_AtkSpawnPos.rotation);
        NetworkServer.Spawn(attackObjectForSpawn);

        RpcOnAttack();
    }
    [ClientRpc]
    private void RpcOnAttack()
    {
        Debug.Log($"{this.netId}°¡ ¿È");
        m_Animator_Player.SetTrigger("Atk");
    }
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        SpawnObject attackObject = other.GetComponent<SpawnObject>();

        if (attackObject == null)
            return;

        m_Health--;

        if(m_Health <= 0)
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }



}
