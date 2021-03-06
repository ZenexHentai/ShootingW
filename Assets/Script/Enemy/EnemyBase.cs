﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public struct EnemyStates
{
    public int ID;                         //ID
    public string nameing;                 //名前
    public int atackPower;                 //攻撃力
    public int hitPoint;                   //Hp
    public float deylayTime;               //何秒後に打つか
    public float moveTime;                 //移動時間
    public int ReactionID;                 //リアクションID

    public void DataInit(string _d)
    {
        string[] d = _d.Split(',');
        ID = Int32.Parse(d[0]);
        nameing = d[1];
        atackPower = Int32.Parse(d[2]);
        hitPoint = Int32.Parse(d[3]);
        deylayTime = float.Parse(d[4]);
        moveTime = float.Parse(d[5]);
        ReactionID = Int32.Parse(d[6]);
    }
}



public enum AnimationState
{
    NORMAL = 0,     //通常
    SMILE = 1,      //笑顔
    CRY = 3,        //泣く
    ANGER = 2,      //怒り
    SUFFERING = 4,  //苦しむ
    WALK = 6,       //歩き
}


public class EnemyBase : MonoBehaviour
{
    AnimationState animationState;
    SpriteRenderer sprite;

    public Sprite[] walkSprites;
    public Sprite[] waitSprites;
    public Sprite[] atackSprites;
    public Sprite[] damageSprites;



    public EnemyStates states;
    public bool isLive = true;      //生きてるかどうか


    virtual public void DataInit()
    {
        //states.DataInit();
        SpriteInit();
    }

    //エネミーにダメージを与える関数
    virtual public void AddDamage(int _damage)
    {
        states.hitPoint -= _damage;
        if (states.hitPoint <= 0)
        {
            StartCoroutine(Die(() =>
            {
                isLive = false;
                Destroy(gameObject);
            }));
        }
    }

    //ダメージ食らったとき
    virtual public IEnumerator Damage(Action callback = null)
    {

        yield return null;
    }

    //死ぬ
    virtual public IEnumerator Die(Action callback = null)
    {
        float time = 0.5f;

        for (int i = 0; i < damageSprites.Length; i++)
        {
            sprite.sprite = damageSprites[i];
            yield return new WaitForSeconds(time);
        }
        if (callback != null)
        {
            callback();
        }
    }

    //攻撃モーション
    virtual public IEnumerator Atack()
    {
        if (animationState != AnimationState.ANGER)
            yield break;

        animationState = AnimationState.ANGER;
        sprite.sprite = atackSprites[0];
        DataManager.Instans.player.Damage(1);
        yield return new WaitForSeconds(1);
        StartCoroutine(Wait());
    }

    //待機Motion
    virtual public IEnumerator Wait()
    {
        if (animationState != AnimationState.NORMAL)
            yield break;

        animationState = AnimationState.NORMAL;
        sprite.sprite = waitSprites[0];
        yield return new WaitForSeconds(1);
        StartCoroutine(Atack());

    }
    //歩き
    virtual public IEnumerator Walk()
    {
        animationState = AnimationState.WALK;
        float time = 0.2f;
        for (int i = 0; true; i++)
        {

            if (animationState != AnimationState.WALK)
                yield break;
            sprite.sprite = walkSprites[i % walkSprites.Length];
            yield return new WaitForSeconds(time);
        }

    }

    //スポーンされたっときの挙動
    virtual public void SpawnMove(
        GameObject _ponPos,
        GameObject _tergetPos)
    {

        float ti = 1f;
        StartCoroutine(Easing.Tween(ti, (t) =>
        {
            if (states.hitPoint <= 0) return;
            transform.position = Vector3.Lerp(
                _ponPos.transform.position,
                 _tergetPos.transform.position, t);
        }, () =>
        {
            StartCoroutine(Wait());
        }));

    }


    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        StartCoroutine(Walk());
    }

    public void SpriteInit()
    {
        walkSprites = Resources.LoadAll<Sprite>("NotShare/素材/gh/" + states.nameing + "/Walk");
        waitSprites = Resources.LoadAll<Sprite>("NotShare/素材/gh/" + states.nameing + "/Wait");
        atackSprites = Resources.LoadAll<Sprite>("NotShare/素材/gh/" + states.nameing + "/Atack");
        damageSprites = Resources.LoadAll<Sprite>("NotShare/素材/gh/" + states.nameing + "/Damage");

    }




}
