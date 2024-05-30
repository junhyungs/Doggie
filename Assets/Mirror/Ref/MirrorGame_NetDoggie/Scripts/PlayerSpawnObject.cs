using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class PlayerSpawnObject : NetworkBehaviour
{
    [Header("Components")]
    public NavMeshAgent NavAgent_Player;
    public Animator Animator_Player;
    public TextMesh TextMesh_HealthBar;
    public TextMesh TextMesh_NetType;
    public Transform Transform_Player;

    [Header("Movement")]
    public float m_RotationSpeed = 100f;

    [Header("Attack")]
    public KeyCode m_AtkKey = KeyCode.Space;
    public GameObject m_Prefab_AtkObject;
    public Transform m_Transform_AtkSpawnPos;

    [Header("Stats Server")]
    [SyncVar] public int m_Health = 4;

    public void Update()
    {
        SetHealthBarOnUpdate(m_Health);

        if (CheckIsFocusedOnUpdate() == false)
            return;

        CheckIsLocalPlayerOnUpdate();
    }

    private void SetHealthBarOnUpdate(int health)
    {
        TextMesh_HealthBar.text = new string('-', health);
    }
    private bool CheckIsFocusedOnUpdate()
    {
        return Application.isPlaying;
    }
    private void CheckIsLocalPlayerOnUpdate()
    {

    }
    //Ŭ�󿡼� ������ ȣ���� ������ ������ ������ �������̵� �¸�
    [Command]
    private void CommandAtk()
    {

    }

    [ClientRpc]
    private void RpcOnAttack()
    {

    }
    //Ŭ�󿡼� ���� �Լ��� ������� �ʵ��� ServerCallbak�� �޾���
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        
    }

}
