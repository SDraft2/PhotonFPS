using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomList : MonoBehaviour, IPointerClickHandler
{
    public Text roomNameTxt;
    public Text playerNumTxt;
    private PhotonLobbyManager lobbyMgr;

    private void Start()
    {
        GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }

    public void SetData(Photon.Realtime.RoomInfo room)
    {
        roomNameTxt.text = room.Name;
        playerNumTxt.text = room.PlayerCount + "/" + room.MaxPlayers;
    }

    public void SetLobbyMgr(PhotonLobbyManager lobbyMgr)
    {
        this.lobbyMgr = lobbyMgr;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        lobbyMgr.JoinRoom(roomNameTxt.text);
    }
}
