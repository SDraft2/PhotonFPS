using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PhotonLobbyManager : MonoBehaviourPunCallbacks
{
    public enum Menu
    {
        MenuConnect,
        MenuLobby,
        MenuRoom
    }

    [Header("StatusText")]
    [Tooltip("test")]
    [SerializeField]
    private Text statusTxt;

    [Header("기본 오브젝트")]
    [Tooltip("방 리스트의 부모로 사용할 스크롤뷰의 컨텐츠")]
    [SerializeField]
    private Transform roomListContentObj;
    [Tooltip("로비에서 스크롤뷰 안에 넣을 방 리스트 프리팹 오브젝트")]
    [SerializeField]
    private GameObject roomObj;

    [Header("접속할때 패널")]
    [Tooltip("서버 접속패널")]
    [SerializeField]
    private GameObject connectPanel;
    [Tooltip("닉네임 입력하는 필드")]
    [SerializeField]
    private InputField nickNameInput;
    [Tooltip("접속하기 버튼")]
    [SerializeField]
    private Button connBtn;

    [Header("로비에 점속했을때 패널")]
    [Tooltip("로비 패널")]
    [SerializeField]
    private GameObject lobbyPanel;
    [Tooltip("로비에서 닉네임 보여줄 텍스트")]
    [SerializeField]
    private Text lob_nickNameTxt;

    [Header("방만들기창")]
    [Tooltip("방만들기 패널")]
    [SerializeField]
    private GameObject createRoomPanel;
    [Tooltip("방제를 입력하는 필드")]
    [SerializeField]
    private InputField createRoomInput;
    [Tooltip("방 최대인원 선택하는 드랍다운(걍 만들어봄)")]
    [SerializeField]
    private Dropdown createRoomMaxPlayerDropdown;
    [Tooltip("방 공개로 설정할지 비공개로 설정할지 결정하는 토글")]
    [SerializeField]
    private Toggle createRoomHideToggle;

    [Header("방접속창")]
    [Tooltip("방제로 접속하는 패널")]
    [SerializeField]
    private GameObject joinRoomPanel;
    [Tooltip("방제로 접속하고 싶을때 입력하는 필드")]
    [SerializeField]
    private InputField joinRoomInput;

    [Header("방에 접속했을때 패널")]
    [Tooltip("방 패널")]
    [SerializeField]
    private GameObject roomPanel;
    [Tooltip("방제를 표시해줄 텍스트")]
    [SerializeField]
    private Text roomNameTxt;
    [Tooltip("게임준비/시작버튼")]
    [SerializeField]
    private Button roomReadyBtn;
    [Tooltip("각각 플레이어의 정보창")]
    [SerializeField]
    private GameObject playerListParentObj;

    private int localPlayerIdx;
    private int readyPlayerCount;
    private bool isReadyGame = false;
    private Image readyBtnImg;

    private PlayerList[] playerListObjs;

    private UIManager uiMgr;
    Menu nowMenu;

    void Awake()
    {
        //Screen.SetResolution(960, 540, false);
        readyBtnImg = roomReadyBtn.GetComponent<Image>();
    }

    private void Start()
    {
        uiMgr = UIManager.Instance;

        playerListObjs = playerListParentObj.GetComponentsInChildren<PlayerList>();
        photonView.ObservedComponents[0] = this;
    }

    private void Update()
    {
        statusTxt.text = PhotonNetwork.NetworkClientState.ToString();
    }

    public void Connect()
    {
        if (string.IsNullOrEmpty(nickNameInput.text))
        {
            ShowErrPanel("닉네임을\n입력해주세요");
            return;
        }

        PhotonNetwork.ConnectUsingSettings();
    }


    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public void SingleRoom()
    {
        PhotonNetwork.CreateRoom("single", new RoomOptions
        {
            MaxPlayers = 1,
            IsVisible = false
        });
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(createRoomInput.text))
        {
            ShowErrPanel("방제목을 입력해주세요");
            return;
        }

        bool isSuccess = PhotonNetwork.CreateRoom(createRoomInput.text, new RoomOptions
        {
            MaxPlayers = (byte)(createRoomMaxPlayerDropdown.value + 2),
            IsVisible = createRoomHideToggle.isOn
        });

        if (!isSuccess)
        {
            ShowErrPanel("잠시만 기다려주세요");
        }
    }

    public void JoinRoom()
    {
        if (string.IsNullOrEmpty(joinRoomInput.text))
        {
            ShowErrPanel("방제목을 입력해주세요");
            return;
        }
        PhotonNetwork.JoinRoom(joinRoomInput.text);
    }
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void ReadyGame()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.MaxPlayers-1 == readyPlayerCount)
            {
                photonView.RPC("RPCGameStart", RpcTarget.AllViaServer);
            }
            else
            {
                uiMgr.ShowErrUi("모든 플레이어가 준비되지 않았습니다");
            }
        }
        else
        {
            isReadyGame = !isReadyGame;
            photonView.RPC("OnReady", RpcTarget.AllViaServer, localPlayerIdx, isReadyGame);
            photonView.RPC("OnReadyInMaster", RpcTarget.MasterClient, isReadyGame);

            if (isReadyGame)
            {
                readyBtnImg.color = new Color(250/255f, 175/255f, 175/255f);
            }
            else
            {
                readyBtnImg.color = new Color(1, 1, 1);
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        print("서버접속성공");
        PhotonNetwork.LocalPlayer.NickName = nickNameInput.text;
        connBtn.interactable = false;
        PhotonNetwork.JoinLobby();
    }

    public override void OnCreatedRoom()
    {
        print("방만들기성공");
        ShowMenu(Menu.MenuRoom);
    }

    public override void OnJoinedRoom()
    {
        print("방 개수 : " + PhotonNetwork.CountOfRooms);
        print("방참가성공");
        ShowMenu(Menu.MenuRoom);
    }


    public override void OnLeftRoom()
    {
        print("방에서나감");
        ShowMenu(Menu.MenuLobby);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ResetPlayerList();
        UpdatePlayerList();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print("방만들기실패");
        ShowErrPanel(message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("방참가실패");
        ShowErrPanel(message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions d;

        bool isSuccess = PhotonNetwork.CreateRoom(PhotonNetwork.LocalPlayer.NickName + System.DateTime.Now.ToString(("HH:mm")), new RoomOptions
        {
            MaxPlayers = 4,
            IsVisible = true
        });

        if (!isSuccess)
        {
            ShowErrPanel("잠시만 기다려주세요");
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        print("연결끊김");
        connBtn.interactable = true;
        ShowErrPanel(cause.ToString());
        ShowMenu(Menu.MenuConnect);
    }

    public override void OnJoinedLobby()
    {
        print("로비접속성공");
        ShowMenu(Menu.MenuLobby);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo room = roomList[i];
            int roomIdx = 0;
            bool roomCondition = room.IsVisible && room.PlayerCount != 0 && room.MaxPlayers != room.PlayerCount;

            while (roomIdx < roomListContentObj.childCount)
            {
                RoomList roomListScript = roomListContentObj.GetChild(roomIdx).GetComponent<RoomList>();

                if (room.Name.Equals(roomListScript.roomNameTxt.text))
                {
                    if (roomCondition)
                    {
                        roomListScript.SetData(room);
                        roomCondition = false;
                    }
                    else
                        Destroy(roomListContentObj.GetChild(roomIdx).gameObject);

                }
                roomIdx++;
            }
            if (roomCondition)
            {
                AddRoom(room);
            }
        }
        print("방목록갱신");
    }

    void AddRoom(RoomInfo room)
    {
        GameObject empRoomObj = Instantiate(roomObj) as GameObject;
        empRoomObj.GetComponent<RoomList>().SetData(room);
        empRoomObj.GetComponent<RoomList>().SetLobbyMgr(this);
        empRoomObj.transform.SetParent(roomListContentObj);
    }

    void ResetPlayerList()
    {
        for (int i = 0; i < playerListObjs.Length; i++)
        {
            playerListObjs[i].GetComponent<PlayerList>().SetDisabled();
        }
    }

    void UpdatePlayerList()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            playerListObjs[i].GetComponent<PlayerList>().SetPlayerData(((Player)PhotonNetwork.PlayerList.GetValue(i)));
        }
    }

    void ShowMenu(Menu menu)
    {
        connectPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);

        nowMenu = menu;

        switch (menu)
        {
            case Menu.MenuConnect:
                connectPanel.SetActive(true);
                break;

            case Menu.MenuLobby:
                lobbyPanel.SetActive(true);
                lob_nickNameTxt.text = PhotonNetwork.LocalPlayer.NickName;
                break;
            case Menu.MenuRoom:
                roomNameTxt.text = PhotonNetwork.CurrentRoom.Name;
                localPlayerIdx = PhotonNetwork.CurrentRoom.PlayerCount - 1;

                if (PhotonNetwork.IsMasterClient)
                    roomReadyBtn.GetComponentInChildren<Text>().text = "게임시작";
                else
                    roomReadyBtn.GetComponentInChildren<Text>().text = "게임준비";

                for (int i = 0; i < playerListObjs.Length; i++)
                {
                    playerListObjs[i].GetComponent<Image>().color = new Color(1, 1, 1);
                }

                for (int i = PhotonNetwork.CurrentRoom.MaxPlayers; i < playerListObjs.Length; i++)
                {
                    playerListObjs[i].GetComponent<Image>().color = new Color(175/255f, 175/255f, 175/255f);
                }

                roomPanel.SetActive(true);
                UpdatePlayerList();
                break;
        }

    }
    public void ShowCreateRoomPanel(bool flag)
    {
        createRoomPanel.SetActive(flag);
    }
    public void ShowJoinRoomPanel(bool flag)
    {
        joinRoomPanel.SetActive(flag);
    }

    private void ShowErrPanel(string errStr)
    {
        uiMgr.ShowErrUi(errStr);
    }

    [ContextMenu("정보")]
    void Info()
    {
        if (PhotonNetwork.InRoom)
        {
            print("현재 방 이름 : " + PhotonNetwork.CurrentRoom.Name);
            print("현재 방 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("현재 방 최대인원수 : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playerStr = "방에 있는 플레이어 목록 : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
            print(playerStr);
        }
        else
        {
            print("접속한 인원 수 : " + PhotonNetwork.CountOfPlayers);
            print("방 개수 : " + PhotonNetwork.CountOfRooms);
            print("모든 방에 있는 인원 수 : " + PhotonNetwork.CountOfPlayersInRooms);
            print("로비에 있는지? : " + PhotonNetwork.InLobby);
            print("연결됐는지? : " + PhotonNetwork.IsConnected);
        }
    }

    [PunRPC]
    void OnReady(int playerIdx, bool isReady)
    {
        playerListObjs[playerIdx].OnReady(isReady);
    }


    [PunRPC]
    void OnReadyInMaster(bool isReady)
    {
        if (isReady)
        {
            readyPlayerCount++;
            if (readyPlayerCount == PhotonNetwork.CurrentRoom.PlayerCount-1)
            {
                readyBtnImg.color = new Color(1, 1, 1);
            }
        }
        else
        {
            readyPlayerCount--;
        }

        Debug.Log(readyPlayerCount);
    }

    [PunRPC]
    void RPCGameStart()
    {
        LoadSceneManager.LoadScene(LoadSceneManager.SceneType.Game);
    }
}

