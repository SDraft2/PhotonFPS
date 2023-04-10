using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePull : MonoBehaviour
{

    public GameObject normalBullet;
    public GameObject bombBullet;

    public GameObject normalEff;
    public GameObject bombEff;



    Queue<GameObject>[] bulletQueue;

    Queue<GameObject> normalBulletObjQueue;
    Queue<GameObject> bombBulletObjQueue;


    Queue<GameObject>[] effQueue;

    Queue<GameObject> normalEffObjQueue;
    Queue<GameObject> bombEffObjQueue;

    private static ResourcePull instance;

    public static ResourcePull Instance { get => instance; set => instance = value; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        bulletQueue = new Queue<GameObject>[3];
        effQueue = new Queue<GameObject>[3];

        normalBulletObjQueue = new Queue<GameObject>();
        bombBulletObjQueue = new Queue<GameObject>();

        normalEffObjQueue = new Queue<GameObject>();
        bombEffObjQueue = new Queue<GameObject>();

        bulletQueue[Bullet.BULLET_INDEX_NORMAL] = normalBulletObjQueue;
        bulletQueue[Bullet.BULLET_INDEX_BOMB] = bombBulletObjQueue;

        effQueue[Bullet.BULLET_INDEX_NORMAL] = normalEffObjQueue;
        effQueue[Bullet.BULLET_INDEX_BOMB] = bombEffObjQueue;

        

        for (int idx = 0; idx < 3; idx++)
        {
            switch (idx)
            {
                case 0:
                    CreateBullet(Bullet.BULLET_INDEX_NORMAL, PhotonNetwork.CurrentRoom.MaxPlayers * 60, normalBullet, normalEff);
                    break;
                case 1:
                    CreateBullet(Bullet.BULLET_INDEX_BOMB, PhotonNetwork.CurrentRoom.MaxPlayers * 30, bombBullet, bombEff);
                    break;
                default:
                    //CreateBullet(idx, 100, null, null);
                    break;
            }
        }
    }

    public void CreateBullet(int bulletIdx, int num, GameObject bulletObj, GameObject effObj)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject bullet = Instantiate(bulletObj, Vector3.zero, Quaternion.identity) as GameObject;
            bullet.SetActive(false);
            bullet.GetComponent<Bullet>().SetBulletIdx(bulletIdx);

            GameObject eff = Instantiate(effObj, Vector3.zero, Quaternion.identity) as GameObject;
            eff.SetActive(false);
            eff.GetComponent<Effect>().SetBulletIdx(bulletIdx);

            bullet.transform.SetParent(transform);
            eff.transform.SetParent(transform);

            bulletQueue[bulletIdx].Enqueue(bullet);
            effQueue[bulletIdx].Enqueue(eff);
        }
    }

    public GameObject GetBullet(int bulletIdx, Vector3 pos, Quaternion rotate)
    {
        GameObject bullet = bulletQueue[bulletIdx].Dequeue();
        bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
        bullet.transform.position = pos;
        bullet.transform.rotation = rotate;
        bullet.SetActive(true);
        return bullet;
    }

    public void DestroyBullet(int bulletIdx, Bullet bullet)
    {
        bullet.Disable();
        bullet.gameObject.SetActive(false);
        bulletQueue[bulletIdx].Enqueue(bullet.gameObject);
    }

    public GameObject GetEffect(int bulletIdx, Vector3 pos, Quaternion rotate)
    {
        GameObject eff = effQueue[bulletIdx].Dequeue();
        eff.transform.position = pos;
        eff.transform.rotation = rotate;
        eff.SetActive(true);
        return eff;
    }

    public void DestroyEff(int bulletIdx, GameObject eff)
    {
        eff.SetActive(false);
        effQueue[bulletIdx].Enqueue(eff);
    }
}
