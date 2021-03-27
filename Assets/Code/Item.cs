using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CreateAssetMenu(menuName = "Game/Inventor Item", fileName = "WorldItem.asset")]
[System.Serializable]

public class Item : ScriptableObject {


    [Space]
    [Header("All:")]
    [Header("=========================")]
    public int horSize;
    public Sprite sprite;
    public enum type { weapon, bullets, pants, torso, head, eat, use};
    public type ItemType;

    public GameObject dropPrefab;

    [Space]
    [Header("Weapon:")]
    [Header("=========================")]
    public bool canReload;
    public bool secondAttack;
    public float dmg, maxdmg, dist;
    public float relTime, secondAttackDopTime;
    public float shootPrevTime, shootOutTime;
    public string idleAnim, attackAnim, reloadAnim, sattackAnim;
    public int weight;
    public GameObject leftPrefab, rightPrefab;
    public RuntimeAnimatorController runtimeAnimator;
    public int bulletsMax, bulletsIn, bulletSub;
    public Item bullets;
    public float attackSpeed = 1;
    public float attackPitch = 1;

    [Space]
    [Header("Clothes:")]
    [Header("=========================")]
    public float armor;
    [Space]
    [Header("Eat:")]
    [Header("=========================")]
    public float hunger;
    public float water, rad, heal;
    [Space]

    [Header("Use:")]
    [Header("=========================")]
    public bool useDestroy;
    public bool arrowBullet;
    public bool spawndecal = true;

    [Header("Rig:")]
    [Header("=========================")]
    public string RidleAnim;
    public string RattackAnim, RreloadAnim, RsattackAnim, RmoveAnim;


    [Header("Sound:")]
    [Header("=========================")]
    public AudioClip shooSound;
    public AudioClip shooSound2;
    public static Item Clone(Item i)
    {
        return new Item() { attackAnim = i.attackAnim, bulletsIn = i.bulletsIn, name = i.name, bulletsMax = i.bulletsMax, bulletSub = i.bulletSub, canReload = i.canReload, dmg = i.dmg, horSize = i.horSize, idleAnim = i.idleAnim, ItemType = i.ItemType, leftPrefab = i.leftPrefab, reloadAnim = i.reloadAnim, relTime = i.relTime, rightPrefab = i.rightPrefab, runtimeAnimator = i.runtimeAnimator, shootPrevTime = i.shootPrevTime, shootOutTime = i.shootOutTime, sprite = i.sprite, bullets = i.bullets, secondAttack = i.secondAttack, sattackAnim = i.sattackAnim, dist = i.dist, dropPrefab = i.dropPrefab, heal = i.heal, hunger = i.hunger, maxdmg = i.maxdmg, rad = i.rad, secondAttackDopTime = i.secondAttackDopTime, water = i.water, weight = i.weight, useDestroy = i.useDestroy, spawndecal = i.spawndecal, arrowBullet = i.arrowBullet, armor = i.armor, shooSound = i.shooSound, RattackAnim = i.RattackAnim, RidleAnim = i.RidleAnim, RmoveAnim = i.RmoveAnim, RreloadAnim = i.RreloadAnim, RsattackAnim = i.RsattackAnim, shooSound2 = i.shooSound2, attackPitch = i.attackPitch, attackSpeed = i.attackSpeed};
    }
}