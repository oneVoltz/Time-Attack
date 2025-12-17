using System.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics;

public class BossAttackSystem : MonoBehaviour
{
    GameObject player;
    bool canAttack = true;
    int prevAtk = -1, num;
    float atkCooldown = 1.5f;
    PlayerSystem playerSys;
    void Awake()
    {
        if (!player) player = GameObject.Find("Player");
        if (!playerSys) playerSys = GetComponent<PlayerSystem>();
    }
    public void TryAttack()
    {
        if (!canAttack) return;
        StartCoroutine(AttackRoutine());
    }
    // initiate selection of a move and perform the one returned from attackRoll()
    IEnumerator AttackRoutine()
    {
        canAttack = false;
        int selection = attackRoll();

        // case corresponds to a specific condition given, case 0 checks if it is exactly 0, case 1 checks if it is exactly 1
        // anything after case [condition]: is what is performed. break; exits the function after performing it
        switch (selection)
        {
            case 0: yield return Attack0(); break;
            case 1: yield return Attack1(); break;
            case 2: yield return Attack2(); break;
        }
        yield return new WaitForSeconds(atkCooldown);
        canAttack = true;
    }
    // rolls for a random value within the given range
    int attackRoll()
    {
        while (num == prevAtk)
        {
            num = Random.Range(0, 3);
        }
        prevAtk = num;
        return num;
    }
    IEnumerator Attack0()
    {
        Debug.Log("Attack 0");
        if (player != null) playerSys.Damaged(33);
        else yield break;
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator Attack1()
    {
        Debug.Log("Windup started");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Attack 1");
        if (player != null) playerSys.Damaged(33);
        else yield break;
        Debug.Log("Endlag started");
        yield return new WaitForSeconds(0.2f);
        Debug.Log("Endlag ended");
    }
    IEnumerator Attack2()
    {
        Debug.Log("Attack 2");
        if (player != null) playerSys.Damaged(33);
        else yield break;
        Debug.Log("Endlag started");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Endlag ended");
    }
}
