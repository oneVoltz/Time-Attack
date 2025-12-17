using System.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics;

public class BossSystem : MonoBehaviour
{
    GameObject player;
    bool canAttack = true;
    int prevAtk = -1;
    float atkCooldown;
    private void Awake()
    {
        player = GameObject.Find("Player");
    }
    void Update()
    {
        FacePlayer();
        if (!canAttack) return;
        StartCoroutine(AttackRoutine());
    }
    void FacePlayer()
    {
        Vector3 rotation = gameObject.transform.rotation.eulerAngles;
        Vector3 lookPoint = (player.transform.position - transform.position) - new Vector3(0f,player.transform.position.y - transform.position.y,0f);
        rotation = Vector3.RotateTowards(transform.forward, lookPoint, 10f, 0f);
        transform.rotation = Quaternion.LookRotation(rotation);
    }
    IEnumerator AttackRoutine()
    {
        canAttack = false;
        int selection = attackRoll();

        switch (selection)
        {
            case 0: Debug.Log("Attack 0"); break;
            case 1: Debug.Log("Attack 1"); break;
        }
        yield return new WaitForSeconds(atkCooldown);
        canAttack = true;
    }
    int attackRoll()
    {
        int num;
        do { num = Random.Range(0, 1); }
        while (num == prevAtk);
        prevAtk = num;
        return num;
    }
}
