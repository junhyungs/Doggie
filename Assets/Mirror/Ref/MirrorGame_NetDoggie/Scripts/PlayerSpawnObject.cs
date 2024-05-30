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

        //회전
        RotateLocalPlayer();

    }
    private void RotateLocalPlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray,out RaycastHit hit, 100))
        {
            Debug.DrawRay(ray.origin, hit.point);
            Vector3 lookRotate = new Vector3(hit.point.x, Transform_Player.position.y, hit.point.z);
            Transform_Player.LookAt(lookRotate);
        }
    }
    //클라에서 서버로 호출은 하지만 로직의 동작은 서버사이드 온리
    [Command]
    private void CommandAtk()
    {
        GameObject attackObjectForSpawn = Instantiate(m_Prefab_AtkObject, m_Transform_AtkSpawnPos.transform.position,m_Transform_AtkSpawnPos.rotation);
        NetworkServer.Spawn(attackObjectForSpawn); //서버에서도 오브젝트를 스폰하기 위해서

        RpcOnAttack();
    }

    [ClientRpc]
    private void RpcOnAttack()
    {
        Debug.LogWarning($"{this.netId}가 RPC호출함");
        Animator_Player.SetTrigger("Atk");
    }

    //클라에서 다음 함수가 실행되지 않도록 ServerCallbak을 달아줌
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        var atkGenObject = other.GetComponent<>();

        if (atkGenObject == null)
            return;

        m_Health--;

        if(m_Health <= 0)
        {
            NetworkServer.Destroy(this.gameObject);
        }
        
    }

}
