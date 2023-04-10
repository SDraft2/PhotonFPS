using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour, IPunObservable
{
    private Animator anim;

    public Camera cam;
    public float speed = 10;

    public MeshRenderer bodyMesh;
    public Transform gunOffset; 
    public GameObject[] gunObj;

    public GameObject bullet;

    public float lerpTime = 5f;

    private float attackDelay = 0.6f;
    private float rotateX;
    private float rotateY;

    private float horizon;
    private float vertical;

    private int charIdx;
    private bool isDeath = false;
    private float runSpeed = 1;
    private GameObject gun;
    private Gun gunScript;
    private Transform fireOffset;
    private bool fireState = true;

    private PhotonView pv;
    private ResourcePull resourcePull;

    private Vector3 currPosit;
    private Quaternion currRot;
    private Quaternion gunRot;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();

        resourcePull = ResourcePull.Instance;

        pv.ObservedComponents[0] = this;
        pv.Synchronization = ViewSynchronization.UnreliableOnChange;

        if (pv.IsMine)
        {
            pv.RPC("RPCInitCharacter", RpcTarget.AllViaServer, GameManager.Instance.LocalCharIdx);
        }
    }

    void Start()
    {
        if (pv.IsMine)
        {
            cam.gameObject.SetActive(true);
            charIdx = GameManager.Instance.LocalCharIdx;
            bodyMesh.material.color = new Color(100/255f, 100/255f, 250/255f);
            pv.RPC("RPCRespawnInvincible", RpcTarget.AllViaServer);
        }
        else
        {
            bodyMesh.material.color = new Color(250/255f, 60/255f, 60/255f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (gun == null)
            return;

            if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(gun.transform.rotation);
        }
        else
        {
            currPosit = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
            gunRot = (Quaternion)stream.ReceiveNext();
        }
    }
    
    void Update()
    {
        if (isDeath)
            return;

        #region LocalPlayer
        if (pv.IsMine)
        {
            if (Input.GetMouseButton(0) && fireState)
            {
                pv.RPC("RPCFire", RpcTarget.AllViaServer, charIdx);
            }
            
            rotateX += Input.GetAxis("Mouse X");
            rotateY += Input.GetAxis("Mouse Y");

            if (rotateX >= 360 || rotateX <= -360)
                rotateX = 0;

            if (rotateY > 90)
                rotateY = 90;

            else if (rotateY < -70)
                rotateY = -70;

            else if (rotateY >= 360 || rotateY <= -360)
                rotateY = 0;

            horizon = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            if (Input.GetButton("Run"))
                runSpeed = 2;
            else
                runSpeed = 1;

            transform.rotation = Quaternion.Euler(0, rotateX, 0);
            cam.transform.rotation = Quaternion.Euler(-rotateY, rotateX, 0);

            if(gun != null)
                gun.transform.rotation = Quaternion.Euler(-rotateY, rotateX, 0);

            transform.Translate(new Vector3(
                horizon,
                0,
                vertical
            ) * speed * runSpeed * Time.deltaTime);

            pv.RPC("RPCSetAnim", RpcTarget.AllViaServer, vertical, horizon);
        }
        #endregion
        #region RemotePlayer
        else
        {
            transform.position = Vector3.Lerp(transform.position, currPosit, lerpTime * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, currRot, lerpTime * Time.deltaTime);
            gun.transform.rotation = Quaternion.Lerp(gun.transform.rotation, gunRot, lerpTime * Time.deltaTime);
        }
        #endregion
    }
    public void OnDeath()
    {
        isDeath = true;
        anim.SetTrigger("isDeath");
    }

    [PunRPC]
    void RPCInitCharacter(int charIdx)
    {
        Debug.Log(charIdx);
        gun = Instantiate(GameManager.Instance.GetGunObject(charIdx), gunOffset) as GameObject;

        gunScript = gun.GetComponent<Gun>();

        fireOffset = gunScript.fireOffset;
        attackDelay = gunScript.attackDelay;
    }

    [PunRPC]
    void RPCFire(int charIdx)
    {
        Debug.Log("RPCFire");

        if (!fireState)
            return;

        float gunDelay = 1;

        switch (charIdx)
        {
            case Bullet.BULLET_INDEX_NORMAL:
                gunDelay = 3;
                break;

            case Bullet.BULLET_INDEX_BOMB:
                gunDelay = 2;
                break;
        }

        gunScript.OnFire(attackDelay/gunDelay);

        fireState = false;
        StartCoroutine(FireDelay());

        Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 500);

        fireOffset.LookAt(hit.point);
        GameObject bulletObj = resourcePull.GetBullet(charIdx, fireOffset.position, fireOffset.rotation);
        bulletObj.GetComponent<Rigidbody>().AddForce(fireOffset.transform.forward * bulletObj.GetComponent<Bullet>().speed);
    }


    IEnumerator FireDelay()
    {
        float timer = attackDelay;
        while (timer > 0)
        {
            timer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        fireState = true;
    }


    [PunRPC]
    void RPCSetAnim(float speed, float speed_horizon)
    {
        anim.SetFloat("speed", speed);
        anim.SetFloat("speed_horizon", speed_horizon);
    }

    [PunRPC]
    void RPCRespawnInvincible()
    {
        StartCoroutine(RespawnEffect(2.5f));
    }

    IEnumerator RespawnEffect(float time)
    {
        MeshRenderer[] mrs = GetComponentsInChildren<MeshRenderer>();

        while(time > 0)
        {
            time -= 0.1f;

            foreach (MeshRenderer mr in mrs)
            {
                mr.enabled = !mr.isVisible;
            }
            yield return new WaitForSeconds(0.1f);
        }
        foreach (MeshRenderer mr in mrs)
        {
            mr.enabled = true;
        }
    }
}
