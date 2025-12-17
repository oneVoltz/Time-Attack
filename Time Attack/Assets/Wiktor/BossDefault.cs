using Unity.VisualScripting;
using UnityEngine;

public class BossDefault : MonoBehaviour
{
    GameObject player;
    GameObject rotateCheck;
    BossAttackSystem attackSystem;
    void Awake()
    {
        if (!player) player = GameObject.Find("Player");
        if (!attackSystem) attackSystem = GetComponent<BossAttackSystem>();
        if (!rotateCheck) rotateCheck = GameObject.Find("RotateCheck");
    }
    void Update()
    {
        FacePlayer();
        attackSystem.TryAttack();
    }
    void FacePlayer()
    {
        if (player != null)
        {
            Vector3 playerDir = (player.transform.position - transform.position) - new Vector3(0, player.transform.position.y - transform.position.y, 0);
            Vector3 newRot = Vector3.RotateTowards(transform.position, playerDir, 10f, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newRot), 5f * Time.deltaTime);
            rotateCheck.transform.rotation = Quaternion.LookRotation(playerDir);
            Debug.Log(rotateCheck.transform.rotation.y / transform.rotation.y);
        }
    }
}