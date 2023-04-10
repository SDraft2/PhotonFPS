using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public const int BULLET_INDEX_NORMAL = 0;
    public const int BULLET_INDEX_BOMB = 1;
    public const int BULLET_INDEX_NL = 2;

    public float damage;
    public float speed;

    public int bulletIdx;
    /*private TrailRenderer trailRenderer;

   private void Start()
   {
       if (bulletIdx == BULLET_INDEX_NORMAL)
           trailRenderer = GetComponentInChildren<TrailRenderer>();
   }

   private void Update()
   {
       if(trailRenderer != null)
           trailRenderer.SetPosition(0, transform.position);
       Debug.Log(transform.position);
    }*/

    private void OnEnable()
    {
        StartCoroutine(DelayDestroy());
    }

    public void Disable()
    {
        ResourcePull.Instance.GetEffect(bulletIdx, transform.position, transform.rotation);
    }

    public void SetBulletIdx(int idx)
    {
        bulletIdx = idx;
    }

    public void OnHit()
    {
        ResourcePull.Instance.DestroyBullet(bulletIdx, this);
    }

    IEnumerator DelayDestroy()
    {
        float timer = 5f;

        while (timer > 0)
        {
            timer -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        ResourcePull.Instance.DestroyBullet(bulletIdx, this);
    }

    private void OnTriggerStay(Collider collider)
    {
        if (bulletIdx == BULLET_INDEX_NORMAL && collider.tag.Equals("Wall"))
        {
            ResourcePull.Instance.DestroyBullet(bulletIdx, this);
        }

        if (collider.tag.Equals("InvisibleWall"))
        {
            ResourcePull.Instance.DestroyBullet(bulletIdx, this);
        }
        
    }
}
