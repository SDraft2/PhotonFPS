using UnityEngine;
using UnityEngine.UI;

public class PlayerList : MonoBehaviour
{
    public GameObject contentPanel;
    public Text nicknameTxt;

    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    private void OnEnable()
    {
        nicknameTxt.text = "";
    }

    public void SetPlayerData(Photon.Realtime.Player player)
    {
        nicknameTxt.text = player.NickName;
        contentPanel.SetActive(true);
    }

    public void SetDisabled()
    {
        img.color = new Color(1, 1, 1);
        contentPanel.SetActive(false);
    }

    public void OnReady(bool flag)
    {
        if (flag)
        {
            img.color = new Color(250/255f, 200/255f, 200/255f);
        }
        else
        {
            img.color = new Color(1, 1, 1);
        }
    }
}
