using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnZone : MonoBehaviour
{
    public bool isEnterPlayer = false;

    public bool IsEnterPlayer { get => isEnterPlayer; set => isEnterPlayer = value; }

    private void OnTriggerEnter(Collider other)
    { 
        if(other.CompareTag("Player"))
        {
            IsEnterPlayer = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsEnterPlayer = false;
        }
    }
}
