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
        return Application.isFocused;
    }
    private void CheckIsLocalPlayerOnUpdate()
    {
        if (this.isLocalPlayer == false)
            return;

        //로컬플레이어의 회전
        float Horizontal = Input.GetAxis("Horizontal");
        transform.Rotate(0, Horizontal * m_RotationSpeed * Time.deltaTime, 0);

        //로컬플레이어의 이동
        float Vertical = Input.GetAxis("Vertical");
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        NavAgent_Player.velocity = forward * Mathf.Max(Vertical, 0) * NavAgent_Player.speed;
        Animator_Player.SetBool("Moving", NavAgent_Player.velocity != Vector3.zero);

        //공격
        if (Input.GetKeyDown(m_AtkKey))
        {
            CommandAtk();
        }


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
