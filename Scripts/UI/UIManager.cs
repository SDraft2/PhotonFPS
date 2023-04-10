using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("캔버스")]
    [Tooltip("캔버스 오브젝트")]
    [SerializeField]
    private GameObject canvas;

    [Header("에러패널부분")]
    [Tooltip("에러 패널")]
    [SerializeField]
    private GameObject errPanel;
    [Tooltip("에러 내용 텍스트")]
    [SerializeField]
    private Text errMsgTxt;
    [Tooltip("에러 내용 버튼")]
    [SerializeField]
    private Button errBtn;

    [Header("페이드 처리부분")]
    [Tooltip("페이드효과에 쓰일 검정색 이미지")]
    public GameObject fadeImgObj;

    [Header("로딩효과 처리부분")]
    [Tooltip("로딩패널 오브젝트")]
    public GameObject loadingObj;

    private Image fadeImg;

    private const float FADE_TIME = 0.1f;

    private static UIManager instance = null;
    public static UIManager Instance { get => instance; set => instance = value; }

    public void Awake()
    {
        fadeImg = fadeImgObj.GetComponent<Image>();
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (instance == null)
            instance = this;
        else
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        canvas.SetActive(false);
        errPanel.SetActive(false);
        ShowLoadingEffect(false);
        canvas.SetActive(true);
        FadeIn(null);
    }

    public void ShowErrUi(string msg)
    {
        errPanel.SetActive(true);
        errMsgTxt.text = msg;
    }
    public void ShowErrUi(string msg, System.Action action)
    {
        errPanel.SetActive(true);
        errMsgTxt.text = msg;

        errBtn.onClick.AddListener(delegate {
            errBtn.onClick.RemoveAllListeners();
            action.Invoke();
        });
    }
    public void ShowErrUi<T1>(string msg, System.Action<T1> action, T1 obj)
    {
        errPanel.SetActive(true);
        errMsgTxt.text = msg;

        errBtn.onClick.AddListener(delegate {
            errBtn.onClick.RemoveAllListeners();
            action.Invoke(obj);
        });
    }

    public void TryInLoading(System.Action action)
    {
        loadingObj.SetActive(true);
        action.Invoke();
        loadingObj.SetActive(false);
    }
    public void TryInLoading<T1>(System.Action<T1> action, T1 obj)
    {
        loadingObj.SetActive(true);
        action.Invoke(obj);
        loadingObj.SetActive(false);
    }

    public void TryInLoading<T1, T2>(System.Action<T1, T2> action, T1 obj, T2 obj2)
    {
        loadingObj.SetActive(true);
        action.Invoke(obj, obj2);
        loadingObj.SetActive(false);
    }

    public void ShowLoadingEffect(bool enable)
    {
        loadingObj.SetActive(enable);
    }

    //In이 밝아지는거
    public void FadeIn(System.Action func)
    {
        fadeImgObj.SetActive(true);
        StartCoroutine(FadeInPlay(func));
    }
    IEnumerator FadeInPlay(System.Action func)
    {
        float fadeTimer = FADE_TIME;
        Color fadeImgColor = fadeImg.color;

        while (fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime;

            fadeImgColor.a = Mathf.Lerp(0, 1, fadeTimer / FADE_TIME);
            fadeImg.color = fadeImgColor;
            yield return null;
        }
        func?.Invoke();
        fadeImgObj.SetActive(false);
    }

    //Out이 어두워지는거
    public void FadeOut(System.Action func)
    {
        fadeImgObj.SetActive(true);
        StartCoroutine(FadeOutPlay(func));
    }
    IEnumerator FadeOutPlay(System.Action func)
    {
        float fadeTimer = FADE_TIME;
        Color fadeImgColor = fadeImg.color;

        while (fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime;

            fadeImgColor.a = Mathf.Lerp(1, 0, fadeTimer / FADE_TIME);
            fadeImg.color = fadeImgColor;
            yield return null;
        }
        func?.Invoke();
    }

}

