using System.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics;

public class BossSystem : MonoBehaviour
{
    GameObject player;
    bool canAttack = true;
    private void Awake()
    {
        player = GameObject.Find("Player");
    }
    void Update()
    {
        FacePlayer();
    }
    void FacePlayer()
    {
        Vector3 rotation = gameObject.transform.rotation.eulerAngles;
        Vector3 lookPoint = (player.transform.position - transform.position) - new Vector3(0f,player.transform.position.y - transform.position.y,0f);
        rotation = Vector3.RotateTowards(transform.forward, lookPoint, 10f, 0f);
        transform.rotation = Quaternion.LookRotation(rotation);
    }
}
