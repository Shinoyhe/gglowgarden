using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.SceneManagement;

public enum LetterSFX
{
    A = 0,
    E = 1,
    I = 2,
    O = 3,
    U = 4,
    PUNCTUATION = 5
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
    public EventInstance currentPlaying;
    protected PLAYBACK_STATE playbackState;
    
    private EventInstance currentLetterSFX;

    // Regions here to collapse code
    #region VOLUME CONTROL
    [Header("Volumes (sliders)")]
    [Range(0f, 1f)]
    public float masterVolume = 0.5f;
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.5f;

    // [Header("Bus Paths")]
    private string masterBusPath = "bus:/";
    private string musicBusPath = "bus:/Music";
    private string sfxBusPath = "bus:/SFX";

    //BUSES
    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;
    #endregion

    #region OST PATHS
    [Tooltip("FMOD Event Path for the menu music")]
    private string masterOST = "event:/Music/MainMusic";
    #endregion

    #region SFX PATHS
    // Letter SFX Paths
    private string letterSFX = "event:/SFX/Letters";
    // Misc SFX Paths
    private string gglowOrbSFX = "event:/SFX/gglowOrb";
    private string squidSaysSFX = "event:/SFX/squidSays";
    private string squidInteractSFX = "event:/SFX/squidInteract";
    private string collectFlowerSFX = "event:/SFX/CollectFlower";
    #endregion

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        
        masterBus = RuntimeManager.GetBus(masterBusPath);
        musicBus = RuntimeManager.GetBus(musicBusPath);
        sfxBus = RuntimeManager.GetBus(sfxBusPath);
        // SceneManager.sceneLoaded += (_, _) => PlayMasterOST();
        SceneManager.sceneUnloaded += (_) => StopCurrentSong();
        
        if (!RuntimeManager.HasBankLoaded("Master"))
        {
            RuntimeManager.LoadBank("Master");
            Debug.Log("Master Bank Loaded");
        }
    }
    
    private void Start() {
        PlayMasterOST();
    }

    // Update is called once per frame
    void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        sfxBus.setVolume(sfxVolume);
    }

    /// <summary>
    /// Play any sfx or other sound that isn't music.
    /// </summary>
    /// <param name="path">The path to the sound.</param>
    public void Play(string path)
    {
        FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(path, out _);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogWarning("FMOD event path does not exist: " + path);
            return;
        }

        EventInstance instance = RuntimeManager.CreateInstance(path);
        instance.start();
        instance.release();
    }

    /// <summary>
    /// Plays an SFX setting a given parameter to a given value
    /// </summary>
    /// <param name="path">The path to the sound</param>
    /// <param name="parameter">The the name of the parameter</param>
    /// <param name="value">The value to set the parameter at</param>
    public void PlayModified(string path, string parameter, float value)
    {
        FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(path, out _);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogWarning("FMOD event path does not exist: " + path);
            return;
        }

        EventInstance instance = RuntimeManager.CreateInstance(path);
        instance.setParameterByName(parameter, value);
        instance.start();
        instance.release();
    }
    
    public void PlayLetterModified(string path, string parameter, float value, float pitch = 0f)
    {
        if (currentLetterSFX.isValid()){
            currentLetterSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        
        FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(path, out _);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogWarning("FMOD event path does not exist: " + path);
            return;
        }

        currentLetterSFX = RuntimeManager.CreateInstance(path);
        currentLetterSFX.setParameterByName(parameter, value);
        currentLetterSFX.setPitch(pitch);
        currentLetterSFX.start();
        currentLetterSFX.release();
    }

    /// <summary>
    /// Play a song from the OST.
    /// </summary>
    /// <param name="path">The path to the song.</param>
    public void PlayOST(string path)
    {
        Debug.Log("[Audio Manager] Playing Song: " + path);
        if (currentPlaying.isValid())
        {
            Debug.Log("Current playing is valid");

            StartCoroutine(RestOfPlayOST(path));
        }
        else
        {
            FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(path, out _);
            if (result != FMOD.RESULT.OK)
            {
                Debug.LogWarning("[Audio Manager] FMOD SONG event path does not exist: " + path);
                return;
            }

            EventInstance song = RuntimeManager.CreateInstance(masterOST);
            currentPlaying = song;
            song.start();
            song.release();
        }
    }

    public IEnumerator RestOfPlayOST(string path)
    {
        Debug.Log("Stopping" + currentPlaying + " (rest of Play OST)");
        currentPlaying.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        currentPlaying.getPlaybackState(out playbackState);
        while (playbackState != PLAYBACK_STATE.STOPPED)
        {
            currentPlaying.getPlaybackState(out playbackState);
            yield return null;
        }

        FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(path, out _);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogWarning("[Audio Manager] FMOD SONG event path does not exist: " + path);
        }
        else
        {
            yield return new WaitForSeconds(1);
            EventInstance song = RuntimeManager.CreateInstance(path);
            currentPlaying = song;
            song.start();
            song.release();
        }
    }

    /// <summary>
    /// Stops the song currently playing
    /// </summary>
    public void StopCurrentSong()
    {
        StartCoroutine(StopCurrentSongRoutine());
    }

    public IEnumerator StopCurrentSongRoutine()
    {
        Debug.Log("Stopping" + currentPlaying + " (rest of Play OST)");
        currentPlaying.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        currentPlaying.getPlaybackState(out playbackState);
        while (playbackState != PLAYBACK_STATE.STOPPED)
        {
            currentPlaying.getPlaybackState(out playbackState);
            yield return null;
        }
    }

    #region OST Functions
    public void PlayMasterOST()
    {
        PlayOST(masterOST);
    }
    #endregion

    #region SFX Functions
    public void PlayLetterTrack(LetterSFX letterTrack, float pitch)
    {
        PlayLetterModified(letterSFX, "Letter", (int)letterTrack, pitch);
    }

    public void PlaygglowOrbSFX()
    {
        Play(gglowOrbSFX);
    }
    
    public void PlaySquidSaysSFX()
    {
        Play(squidSaysSFX);
    }

    public void PlaySquidInteractSFX()
    {
        Play(squidInteractSFX);
    }
    
    public void PlayCollectFlowerSFX(){
        Play(collectFlowerSFX);
    }

    /// <summary>
    /// Plays the select sfx
    /// </summary>
    // public void PlayUISelectSFX()
    // {
    //     Play(selectUISFX);
    //     Play(selectUISFX);
    // }

    /// <summary>
    /// Plays the slide sfx
    /// </summary>
    // public void PlayUIHoverSFX()
    // {
    //     Play(hoverUISFX);
    //     Play(hoverUISFX);
    // }
    #endregion
}
