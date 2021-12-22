using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using DilmerGames.Core.Singletons;

public class SceneTransitionHandler : NetworkSingleton<SceneTransitionHandler>
{
    static public SceneTransitionHandler sceneTransitionHandler { get; internal set; }

    [SerializeField]
    public string DefaultMainMenu = "XRMultiplayerMain";

    [HideInInspector]
    public delegate void ClientLoadedSceneDelegateHandler(ulong clientId);
    [HideInInspector]
    public event ClientLoadedSceneDelegateHandler OnClientLoadedScene;

    [HideInInspector]
    public delegate void SceneStateChangedDelegateHandler(SceneStates newState);
    [HideInInspector]
    public event SceneStateChangedDelegateHandler OnSceneStateChanged;

    private int m_numberOfClientLoaded;
    
    public bool InitializeAsHost { get; set; }

    private void Update()
    {
        // for testing only
        if(Input.GetKeyDown(KeyCode.H))
        {
            InitializeAsHost = true;
            Initialize();
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            InitializeAsHost = false;
            Initialize();
        }
    }

    /// <summary>
    /// Example scene states
    /// </summary>
    public enum SceneStates
    {
        Init,
        Start,
        Lobby,
        Ingame
    }

    private SceneStates m_SceneState;

    /// <summary>
    /// Awake
    /// If another version exists, destroy it and use the current version
    /// Set our scene state to INIT
    /// </summary>
    private void Awake()
    {
        Debug.Log("Starting SceneTransitionHandler");

        if(sceneTransitionHandler != this && sceneTransitionHandler != null)
        {
            GameObject.Destroy(sceneTransitionHandler.gameObject);
        }
        sceneTransitionHandler = this;
        SetSceneState(SceneStates.Init);
    }

    
    /// <summary>
    /// SetSceneState
    /// Sets the current scene state to help with transitioning.
    /// </summary>
    /// <param name="sceneState"></param>
    public void SetSceneState(SceneStates sceneState)
    {
        Debug.Log("SetSceneState on SceneTransitionHandler");
        m_SceneState = sceneState;
        if(OnSceneStateChanged != null)
        {
            OnSceneStateChanged.Invoke(m_SceneState);
        }
    }

    /// <summary>
    /// GetCurrentSceneState
    /// Returns the current scene state
    /// </summary>
    /// <returns>current scene state</returns>
    public SceneStates GetCurrentSceneState()
    {
        return m_SceneState;
    }

    /// <summary>
    /// Initialize
    /// Loads the default main menu when started (this should always be a component added to the networking manager)
    /// </summary>
    public void Initialize()
    {
        Debug.Log("Initialize on SceneTransitionHandler");
        if (m_SceneState == SceneStates.Init)
        {
            SceneManager.LoadScene(DefaultMainMenu);
        }
    }

    /// <summary>
    /// Switches to a new scene
    /// </summary>
    /// <param name="scenename"></param>
    public void SwitchScene(string scenename)
    {
        if(NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
            NetworkManager.Singleton.SceneManager.LoadScene(scenename, LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadSceneAsync(scenename);
        }
    }
    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        //We are only interested by Client Loaded Scene events
        if (sceneEvent.SceneEventType != SceneEventType.LoadComplete) return;

        m_numberOfClientLoaded += 1;
        OnClientLoadedScene?.Invoke(sceneEvent.ClientId);
    }

    public bool AllClientsAreLoaded()
    {
        return m_numberOfClientLoaded == NetworkManager.Singleton.ConnectedClients.Count;
    }

    /// <summary>
    /// ExitAndLoadStartMenu
    /// This should be invoked upon a user exiting a multiplayer game session.
    /// </summary>
    public void ExitAndLoadStartMenu()
    {
        OnClientLoadedScene = null;
        SetSceneState(SceneStates.Start);
        SceneManager.LoadScene(1);
    }
}