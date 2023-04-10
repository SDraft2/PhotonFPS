using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPScript : MonoBehaviour
{
    public float maxHP;
    public Slider hpPlayerBar;

    public float currentHP;

    private PhotonView pv;
    private PlayerCtrl playerCtrl;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        playerCtrl = GetComponent<PlayerCtrl>();
    }

    void Start()
    {
        GameManager.Instance.SetHpBarMaxValue(maxHP);
        pv.RPC("RPCHpInit", RpcTarget.AllViaServer);
    }

    public void OnDeath()
    {
        currentHP = maxHP;
        playerCtrl.OnDeath();

        if(pv.IsMine)
        {
            StartCoroutine(RespawnProcess());
        }
    }


    void ChangeHp(float hp)
    {
        hpPlayerBar.value = hp;

        if (pv.IsMine)
        {
            GameManager.Instance.OnChangedHP(hp);
        }
    }
    void ChangeHp(float oldHP, float newHP)
    {
        ChangeHp(newHP);
    }

    IEnumerator RespawnProcess()
    {
        yield return null;
    }

    [PunRPC]
    void RPCHpInit()
    {
        currentHP = maxHP;

        if (pv.IsMine)
            hpPlayerBar.gameObject.SetActive(false);
        else
            hpPlayerBar.gameObject.SetActive(true);

        hpPlayerBar.maxValue = maxHP;
        hpPlayerBar.value = maxHP;

        ChangeHp(currentHP);
    }


    [PunRPC]
    void RPCTakeDamage(float damage)
    {
        float oldHp = currentHP;

        if (currentHP - damage <= 0)
        {
            currentHP = maxHP;
            OnDeath();
        }
        else
            currentHP -= damage;

        ChangeHp(currentHP);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Untagged"))
            return;

        GameObject obj = other.gameObject;

        if (obj.tag.Equals("Bullet"))
        {
            Bullet bullet = obj.GetComponent<Bullet>();
            float damage = bullet.damage;
            bullet.OnHit();
            Debug.Log("Hit" + damage);

            if (PhotonNetwork.IsMasterClient)
            {
                pv.RPC("RPCTakeDamage", RpcTarget.AllViaServer, damage);
            }
        }
    }

}
