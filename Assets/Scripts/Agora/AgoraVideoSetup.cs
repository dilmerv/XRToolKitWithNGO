using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DilmerGames.Core.Singletons;
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif

public class AgoraVideoSetup : Singleton<AgoraVideoSetup>
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

    private TextMeshProUGUI joinChannelButtonText;

    private Image joinChannelButtonImage;

#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new ArrayList();
#endif

    void Awake()
    {
        joinChannelButtonText = joinChannelButton
                .GetComponentInChildren<TextMeshProUGUI>();

        joinChannelButtonImage = joinChannelButton.GetComponent<Image>();
        joinChannelButtonImage.color = Color.green;

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

        // join channel logic
        joinChannelButton.onClick.AddListener(() =>
        {
            if (joinChannelButtonText.text.Contains($"{ChannelActions.JOIN}"))
            {
                StartAgora();
            }
            else
            {
                LeaveAgora();
            }
        });
    }

    public uint GetAgoraUserId() => AgoraUnityVideo.Instance
        .LocalUserId;

    public void StartAgora()
    {
        if (settingsReady)
        {
            CheckPermissions();
            AgoraUnityVideo.Instance.LoadEngine(appId, token);
            AgoraUnityVideo.Instance.Join(channelName);

            joinChannelButtonText.text = $"{ChannelActions.LEAVE} CHANNEL";
            var joinButtonImage = joinChannelButton.GetComponent<Image>();
            joinButtonImage.color = Color.yellow;
        }
        else
            Logger.Instance.LogError("Agora [appId] or [channelName] need to be added");
    }

    public void LeaveAgora()
    {
        AgoraUnityVideo.Instance.Leave();
        joinChannelButtonText.text = $"{ChannelActions.JOIN} CHANNEL";
        joinChannelButtonImage.color = Color.white;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.S)) StartAgora();
        if (Input.GetKey(KeyCode.L)) LeaveAgora();
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
