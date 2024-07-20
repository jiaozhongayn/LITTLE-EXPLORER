/*
* @Author: 教忠言
* @Description:
* @Date: 2024年05月12日 星期日 17:05:38
* @Modify:
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int MaxHealth;
    public int CurrentHealth;
    private Character _cc;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
        _cc = GetComponent<Character>();
    }

    public void ApplyDamage(int damage)
    {
        CurrentHealth -= damage;
        Debug.Log($"{gameObject.name}受到了{damage}伤害");
        Debug.Log($"{gameObject.name}剩余{CurrentHealth}生命值");
        ChackHealth();
    }

    public void ChackHealth()
    {
        if (CurrentHealth <= 0)
        {
            _cc.SwitchStateTo(Character.CharacterState.Dead);
        }
    }
}
