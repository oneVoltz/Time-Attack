using Unity.VisualScripting;
using UnityEngine;

public class BossDefault : MonoBehaviour
{
    GameObject player;
    BossAttackSystem attackSystem;
    void Awake()
    {
        if (!player) player = GameObject.Find("Player");
        attackSystem = GetComponent<BossAttackSystem>();
    }
    void Update()
    {
        FacePlayer();
        attackSystem.TryAttack();
    }
    void FacePlayer()
    {
        Vector3 playerDir = (player.transform.position - transform.position) - new Vector3(0, player.transform.position.y - transform.position.y, 0);
        Vector3 newRot = Vector3.RotateTowards(transform.position, playerDir, 10f, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newRot), 10f * Time.deltaTime);
    }
}
