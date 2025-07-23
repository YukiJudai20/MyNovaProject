using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class Actor: MonoBehaviour
{
    public List<int> skillIds = new List<int>();
    public int hp;
    public int maxHp;
    public int def;
    public int atk;
    private Animator _animator;

    public Actor(int hp, int maxHp,int mp,int maxMp,int def,int atk)
    {
        this.hp = hp;
        this.maxHp = maxHp;
        this.def = def;
        this.atk = atk;
    }

    public void TakeDamage(int skillDamageParam,int AttackerAtk)
    {
        int damage = skillDamageParam / 100 * AttackerAtk - def;
        hp -=damage;
        _animator.Play("Hurt");
        //播放掉血动画 TODO
        if (hp < 0)
        {
            hp = 0;
            Death();
        }
    }

    public void Death()
    {
        _animator.Play("Death");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            GetComponent<ThirdPersonController>()._playerInput.enabled = false;
        }
    }
}
