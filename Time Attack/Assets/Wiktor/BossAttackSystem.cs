using System.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics;

public class BossAttackSystem : MonoBehaviour
{
    GameObject player;
    bool canAttack = true;
    int prevAtk = -1, num;
    float atkCooldown = 1.5f;
    void Awake()
    {
        if (!player) player = GameObject.Find("Player");
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
            case 0: Debug.Log("Attack 0"); break;
            case 1: Debug.Log("Attack 1"); break;
            case 2: Debug.Log("Attack 2"); break;
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
}
