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

        //�����÷��̾��� ȸ��
        float Horizontal = Input.GetAxis("Horizontal");
        transform.Rotate(0, Horizontal * m_RotationSpeed * Time.deltaTime, 0);

        //�����÷��̾��� �̵�
        float Vertical = Input.GetAxis("Vertical");
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        NavAgent_Player.velocity = forward * Mathf.Max(Vertical, 0) * NavAgent_Player.speed;
        Animator_Player.SetBool("Moving", NavAgent_Player.velocity != Vector3.zero);

        //����
        if (Input.GetKeyDown(m_AtkKey))
        {
            CommandAtk();
        }

        //ȸ��
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
    //Ŭ�󿡼� ������ ȣ���� ������ ������ ������ �������̵� �¸�
    [Command]
    private void CommandAtk()
    {
        GameObject attackObjectForSpawn = Instantiate(m_Prefab_AtkObject, m_Transform_AtkSpawnPos.transform.position,m_Transform_AtkSpawnPos.rotation);
        NetworkServer.Spawn(attackObjectForSpawn); //���������� ������Ʈ�� �����ϱ� ���ؼ�

        RpcOnAttack();
    }

    [ClientRpc]
    private void RpcOnAttack()
    {
        Debug.LogWarning($"{this.netId}�� RPCȣ����");
        Animator_Player.SetTrigger("Atk");
    }

    //Ŭ�󿡼� ���� �Լ��� ������� �ʵ��� ServerCallbak�� �޾���
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
