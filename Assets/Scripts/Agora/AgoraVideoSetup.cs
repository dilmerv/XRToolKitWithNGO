using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif

public class AgoraVideoSetup : MonoBehaviour
{
    public enum ChannelActions
    {
        JOIN,
        LEAVE
    }

    [SerializeField]
    private Button joinChannelButton;

    [SerializeField]
    private string appId = "your_appid";

    [SerializeField]
    private string channelName = "your_channel";

    [SerializeField]
    private string token = "your_token"; // this is for demo purposes we must never expose a token

    private bool settingsReady;

#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new ArrayList();
#endif

    void Awake()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        permissionList.Add(Permission.Microphone);
        permissionList.Add(Permission.Camera);
#endif
        // keep this alive across scenes
        DontDestroyOnLoad(gameObject);   
    }

    void Start()
    {
        if(string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(channelName))
            settingsReady = false;
        else
            settingsReady = true;

        var joinButtonImage = joinChannelButton.GetComponent<Image>();
        joinButtonImage.color = Color.green;

        // join channel logic
        joinChannelButton.onClick.AddListener(() =>
        {
            var buttonText = joinChannelButton
                .GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText.text.Contains($"{ChannelActions.JOIN}"))
            {
                StartAgora();
                buttonText.text = $"{ChannelActions.LEAVE} CHANNEL";
                joinButtonImage.color = Color.yellow;
            }
            else
            {
                AgoraUnityVideo.Instance.Leave();
                buttonText.text = $"{ChannelActions.JOIN} CHANNEL";
                joinButtonImage.color = Color.white;
            }
        });
    }

    private void StartAgora()
    {
        if (settingsReady)
        {
            CheckPermissions();
            AgoraUnityVideo.Instance.LoadEngine(appId, token);
            AgoraUnityVideo.Instance.Join(channelName);
        }
        else
            Logger.Instance.LogError("Agora [appId] or [channelName] need to be added");
    }

    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKey(KeyCode.A)) StartAgora();
#endif
    }

    private void CheckPermissions()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        foreach (string permission in permissionList)
        {
            if (!Permission.HasUserAuthorizedPermission(permission))
            {
                Permission.RequestUserPermission(permission);
            }
        }
#endif
    }

    void OnApplicationPause(bool paused)
    {
        AgoraUnityVideo.Instance.EnableVideo(paused);
    }

    void OnApplicationQuit()
    {
        AgoraUnityVideo.Instance.UnloadEngine();
    }
}
