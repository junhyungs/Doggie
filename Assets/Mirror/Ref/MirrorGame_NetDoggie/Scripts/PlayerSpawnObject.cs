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
    //클라에서 서버로 호출은 하지만 로직의 동작은 서버사이드 온리
    [Command]
    private void CommandAtk()
    {

    }

    [ClientRpc]
    private void RpcOnAttack()
    {

    }
    //클라에서 다음 함수가 실행되지 않도록 ServerCallbak을 달아줌
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        
    }

}
