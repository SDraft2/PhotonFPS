using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform fireOffset;
    public float attackDelay;
    public GameObject fireEffect;

    public void OnFire(float delay)
    {
        StartCoroutine(Fire(delay));
    }

    IEnumerator Fire(float delay)
    {
        fireEffect.SetActive(true);
        yield return new WaitForSeconds(delay);
        fireEffect.SetActive(false);
    }
}
