using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public int bulletIdx;

    private void OnEnable()
    {
        StartCoroutine(DelayDisable(5));
    }

    public void SetBulletIdx(int idx)
    {
        bulletIdx = idx;
    }
    IEnumerator DelayDisable(float time)
    {
        float timer = time;
        while (timer > 0)
        {
            timer -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        ResourcePull.Instance.DestroyEff(bulletIdx, gameObject);
    }
}
