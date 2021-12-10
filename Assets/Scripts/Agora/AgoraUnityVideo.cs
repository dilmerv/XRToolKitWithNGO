using agora_gaming_rtc;
using DilmerGames.Core.Singletons;
using UnityEngine;
using UnityEngine.UI;

public class AgoraUnityVideo : Singleton<AgoraUnityVideo>
{
    private IRtcEngine mRtcEngine;

    private string token;

    private int lastError;

    private uint localUserId;

    public uint LocalUserId
    {
        get => localUserId;
    }

    public void LoadEngine(string appId, string token = null)
    {
        Logger.Instance.LogInfo("Loading Engine initialization");

        this.token = token;

        if (mRtcEngine != null)
        {
            Logger.Instance.LogInfo("Engine exists. Please unload it first!");
            return;
        }

        mRtcEngine = IRtcEngine.GetEngine(appId);
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
    }

    public void Join(string channel)
    {
        Logger.Instance.LogInfo($"Calling join(channel = {channel})");

        if (mRtcEngine == null) return;

        // set callbacks (optional)
        mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
        mRtcEngine.OnUserJoined = OnUserJoined;
        mRtcEngine.OnUserOffline = OnUserOffline;
        mRtcEngine.OnWarning = (int warn, string msg) =>
        {
            Logger.Instance.LogWarning($"Warning code:{warn} msg:{IRtcEngine.GetErrorDescription(warn)}");
        };

        mRtcEngine.OnError = HandleError;
        mRtcEngine.EnableVideo();

        // allow camera output callback
        mRtcEngine.EnableVideoObserver();

        // join channel
        /*  This API Assumes the use of a test-mode AppID
             mRtcEngine.JoinChannel(channel, null, 0);
        */

        /*  This API Accepts AppID with token; by default omiting info and use 0 as the local user id */
        mRtcEngine.JoinChannelByKey(channelKey: token, channelName: channel);
    }

    public void Leave()
    {
        Logger.Instance.LogInfo("Leaving channel");
        if (mRtcEngine == null) return;

        mRtcEngine.LeaveChannel();

        // deregister video frame observers in native-c code
        mRtcEngine.DisableVideoObserver();

        GameObject go = GameObject.Find($"{localUserId}");
        if (go != null) Destroy(go);
    }

    // unload agora engine
    public void UnloadEngine()
    {
        Logger.Instance.LogInfo("Calling unloadEngine");
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();  // Place this call in ApplicationQuit
            mRtcEngine = null;
        }
    }

    public void EnableVideo(bool pauseVideo)
    {
        if (mRtcEngine != null)
        {
            if (!pauseVideo)
                mRtcEngine.EnableVideo();
            else
                mRtcEngine.DisableVideo();
        }
    }

    // Implement engine callbacks
    private void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        localUserId = uid;
        Logger.Instance.LogInfo($"OnJoinChannelSuccess: uid = {uid}");
        Logger.Instance.LogInfo($"SDK Version : {IRtcEngine.GetSdkVersion()}");

        GameObject childVideo = GetChildVideoLocation(uid);
        MakeImageVideoSurface(childVideo);
    }

    // When a remote user joined, this delegate will be called. Typically
    // create a GameObject to render video on it
    private void OnUserJoined(uint uid, int elapsed)
    {
        Logger.Instance.LogInfo($"onUserJoined: uid = {uid} elapsed = {elapsed}");

        GameObject childVideo = GetChildVideoLocation(uid);

        // create a GameObject and assign to this new user
        VideoSurface videoSurface = MakeImageVideoSurface(childVideo);

        if (videoSurface != null)
        {
            // configure videoSurface
            videoSurface.SetForUser(uid);
            videoSurface.SetEnable(true);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
        }
    }

    private static GameObject GetChildVideoLocation(uint uid)
    {
        // find a game object to render video stream from 'uid'
        GameObject go = GameObject.Find("Videos");
        GameObject childVideo = go.transform.Find($"{uid}")?.gameObject;

        if (childVideo == null)
        {
            childVideo = new GameObject($"{uid}");
            childVideo.transform.parent = go.transform;
        }

        return childVideo;
    }

    public VideoSurface MakeImageVideoSurface(GameObject go)
    {
        go.AddComponent<RawImage>();
        go.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        var rectTransform = go.GetComponent<RectTransform>();
        
        rectTransform.sizeDelta = new Vector2(60.0f, 50.0f);
        rectTransform.localPosition = new Vector3(rectTransform.position.x,
            rectTransform.position.y, 0);

        rectTransform.localRotation = new Quaternion(0, rectTransform.localRotation.y,
            -180.0f, rectTransform.localRotation.w);

        return go.AddComponent<VideoSurface>();
    }

    // When remote user is offline, this delegate will be called. Typically
    // delete the GameObject for this user
    private void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        Logger.Instance.LogInfo($"OnUserOffline: uid = {uid} reason = {reason}");
        GameObject go = GameObject.Find(uid.ToString());
        if (go != null) Destroy(go);
    }

    private void HandleError(int error, string msg)
    {
        if (error == lastError) return;

        if (string.IsNullOrEmpty(msg))
        {
            msg = string.Format($"Error code:{error} msg:{IRtcEngine.GetErrorDescription(error)}");
        }

        switch (error)
        {
            case 101:
                msg += "\nPlease make sure your AppId is valid and it does not require a certificate for this demo.";
                break;
        }

        Logger.Instance.LogError(msg);
        lastError = error;
    }
}