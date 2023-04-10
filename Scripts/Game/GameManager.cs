using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("게임영역")]
    public GameObject respawnZoneParentObject;
    public GameObject[] gunObjs;

    [Header("UI영역")]
    public GameObject hitImg;
    public Slider hpBar;

    public GameObject chooseCharPanel;
    public Button[] chooseCharBtns;

    private static GameManager instance;
    private RespawnZone[] respawnZones;
    private List<GameObject> playerObjList;

    private int localCharIdx = -1;

    bool test = false;

    public static GameManager Instance { get => instance; set => instance = value; }
    public int LocalCharIdx { get => localCharIdx; set => localCharIdx = value; }

    public void Start()
    {
        respawnZones = respawnZoneParentObject.GetComponentsInChildren<RespawnZone>();

    }
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        playerObjList = new List<GameObject>();

        for (int i=0 ; i<chooseCharBtns.Length ; i++)
        {
            Button chooseCharBtn = chooseCharBtns[i];
            int idx = i;

            chooseCharBtn.onClick.AddListener(delegate{
                ChooseCharBtnInit();
                chooseCharBtn.image.color = new Color(1f, 185/255f, 185/255f);
                localCharIdx = idx;
            });
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
            test = !test;

        if (test)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;

    }


    public Vector3 GetRespawnPosition()
    {
        int idx = UnityEngine.Random.Range(0, respawnZones.Length - 1);

        for (int i = 0; i < respawnZones.Length; i++)
        {
            if (!respawnZones[idx].IsEnterPlayer)
            {
                return respawnZones[idx].transform.position;
            }
            else if (i == idx)
                continue;

            if (!respawnZones[i].IsEnterPlayer)
            {
                return respawnZones[idx].transform.position;
            }
            else if (i == respawnZones.Length - 1)
            {
                return respawnZones[idx].transform.position;
            }
        }

        return respawnZones[0].transform.position;
    }

    public GameObject GetGunObject(int charIdx)
    {
        return gunObjs[charIdx];
    }

    public string GetGunName(int charIdx)
    {
        if (charIdx == 0)
        {
            return "NormalGun";
        }
        else if (charIdx == 1)
        {
            return "BombGun";
        }
        else if (charIdx == 2)
        {
            return "BombGun";
        }

        return "BombGun";
    }

    public void ChooseCharBtnInit()
    {
        for (int j = 0; j < chooseCharBtns.Length; j++)
        {
            Button btnObj = chooseCharBtns[j];
            btnObj.image.color = new Color(1f, 1f, 1f);
        }
    }

    public void OnChooseCharSubmit()
    {
        if (localCharIdx == -1)
            return;

        Camera.main.gameObject.SetActive(false);
        chooseCharPanel.SetActive(false);
        playerObjList.Add(PhotonNetwork.Instantiate("Player", GetRespawnPosition(), Quaternion.identity));
    }

    public void SetHpBarMaxValue(float value)
    {
        hpBar.maxValue = value;
    }

    public void OnChangedHP(float hp)
    {
        if (hpBar.value > hp)
            StartCoroutine(HitImg());
        //HIT판정

        hpBar.value = hp;
    }
     
    

    IEnumerator HitImg()
    {
        hitImg.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        hitImg.SetActive(false);
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print("플레이어등장");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

    }
}
