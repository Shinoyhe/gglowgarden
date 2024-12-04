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
    public EventInstance movementSFX;
    protected PLAYBACK_STATE playbackState;
    protected PLAYBACK_STATE movementState;
    
    private EventInstance currentLetterSFX;

    // Scream Distance
    // public Vector2 screamDistance = new Vector2(10, 50);

    // Regions here to collapse code
    #region VOLUME CONTROL
    [Header("Volumes (sliders)")]
    [Range(0f, 1f)]
    public float masterVolume = 0.5f;
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.5f;
    // [Range(0f, 1f)]
    // public float movementsfxVolume = 0.3f;

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
        SceneManager.sceneLoaded += (_, _) => PlayMasterOST();
        SceneManager.sceneUnloaded += (_) => StopCurrentSong();
        
        if (!RuntimeManager.HasBankLoaded("Master"))
        {
            RuntimeManager.LoadBank("Master");
            Debug.Log("Master Bank Loaded");
        }
    }
    
    // private void Start() {
    //     PlayMasterOST();
    // }

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

    // private void SwitchSong(string path)
    // {
    //     string newMusic = path.Split("/").Last().ToLower();
    //     MusicTrack track = MusicTrack.MENU;
    //     switch (newMusic)
    //     {
    //         case "menu":
    //             track = MusicTrack.MENU;
    //             break;
    //         case "village":
    //             track = MusicTrack.VILLAGE;
    //             break;
    //         case "crypt":
    //             track = MusicTrack.CRYPT;
    //             break;
    //         case "dungeon":
    //             track = MusicTrack.DUNGEON;
    //             break;
    //         case "chase":
    //             track = MusicTrack.CHASE;
    //             break;
    //         case "ending":
    //             track = MusicTrack.ENDING;
    //             break;
    //         default:
    //             track = MusicTrack.ENDING;
    //             break;
    //     }

    //     currentPlaying.setParameterByName("GAME SCENE", (float)track);
    //     currentTrack = track;
    //     Debug.Log("Played: " + track);
    // }

    /// <summary>
    /// Stops the song currently playing
    /// </summary>
    public void StopCurrentSong()
    {
        // MusicTrack track = MusicTrack.STOP;
        // currentPlaying.setParameterByName("SHOP", 0);
        // currentPlaying.setParameterByName("GAME SCENE", (float)track);
        // currentTrack = track;
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
/* 
    public void PlayVillageOST()
    {
        PlayOST(villageOST);
    }

    public void PlayCryptKeeperOST()
    {
        PlayOST(cryptOST);
    }
    
    public void PlayShopOST(){
        SetShopOST(true, 3f);
    }
    
    public void StopShopOST(){
        SetShopOST(false, 3f);
    }

    /// <summary>
    /// Transitions between Village OST and Shop OST
    /// </summary>
    /// <param name="play">Whether to play or stop the OST</param>
    /// <param name="transitionTime">The transition time</param>
    public void SetShopOST(bool play, float transitionTime)
    {
        if (currentTrack == MusicTrack.VILLAGE)
        {
            StartCoroutine(TransitionToShopOST(play, transitionTime));
        }
    }

    private IEnumerator TransitionToShopOST(bool play, float transitionTime)
    {
        float start = play ? 0 : 1;
        float goal = play ? 1 : 0;
        float tTime = 0;

        while (tTime < transitionTime)
        {
            tTime += Time.deltaTime;
            float transition = Mathf.Lerp(start, goal, tTime / transitionTime);
            currentPlaying.setParameterByName("SHOP", transition);
            yield return null;
        }
    }

    /// <summary>
    /// Plays the dungeon ost
    /// </summary>
    public void PlayDungeonOST()
    {
        PlayOST(dungeonOST);
    }

    /// <summary>
    /// Plays the chase ost
    /// </summary>
    public void PlayChaseOST()
    {
        PlayOST(chaseOST);
    }

    public void PlayEndingOST()
    {
        PlayOST(endingOST);
    } */
    #endregion

    #region SFX Functions
    public void PlayLetterTrack(LetterSFX letterTrack, float pitch)
    {
        // float value;
        // switch (letterTrack)
        // {
        //     case LetterSFX.A:
        //         value = 0;
        //         break;
        //     case LetterSFX.E:
        //         value = 1;
        //         break;
        //     case LetterSFX.I:
        //         value = 2;
        //         break;
        //     case LetterSFX.O:
        //         value = 3;
        //         break;
        //     case LetterSFX.U:
        //         value = 4;
        //         break;
        //     case LetterSFX.PUNCTUATION:
        //         value = 5;
        //         break;
        //     default:
        //         return;
        // }
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
    
    /// <summary>
    /// Plays a scream in the given emitter
    /// </summary>
    /// <param name="emitter">The emitter to play the scream at</param>
    // public void PlayScream(StudioEventEmitter emitter)
    // {
    //     emitter.EventReference = RuntimeManager.PathToEventReference(monsterScream);
    //     emitter.ResetEvent();
    //     emitter.OverrideAttenuation = true;
    //     emitter.OverrideMinDistance = screamDistance.x;
    //     emitter.OverrideMaxDistance = screamDistance.y;
    //     emitter.Play();
    // }

    private IEnumerator FadeOut()
    {
        movementSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        movementSFX.getPlaybackState(out movementState);
        while (movementState != PLAYBACK_STATE.STOPPED)
        {
            movementSFX.getPlaybackState(out movementState);
            yield return null;
        }
    }

    // public void PlayLowStaminaSFX()
    // {
    //     if (movementSFX.isValid())
    //     {
    //         StartCoroutine(FadeOut());
    //     }
    //     FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(lowStaminaSFX, out _);
    //     if (result != FMOD.RESULT.OK)
    //     {
    //         Debug.LogWarning("FMOD event path does not exist: " + lowStaminaSFX);
    //         return;
    //     }

    //     movementSFX = RuntimeManager.CreateInstance(lowStaminaSFX);
    //     movementSFX.setVolume(sfxVolume);
    //     movementSFX.start();

    // }

    // public void PlayStepSFX(float running = 0.0f)
    // {
    //     if (movementSFX.isValid())
    //     {
    //         StartCoroutine(FadeOut());
    //     }
    //     FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(playerStepSFX, out _);
    //     if (result != FMOD.RESULT.OK)
    //     {
    //         Debug.LogWarning("FMOD event path does not exist: " + playerStepSFX);
    //         return;
    //     }
    //     movementSFX = RuntimeManager.CreateInstance(playerStepSFX);
    //     movementSFX.setParameterByName("running", running);
    //     movementSFX.setVolume(movementsfxVolume);
    //     movementSFX.start();
    // }

    // public void StopStepSound(bool immediate)
    // {
    //     if (movementSFX.isValid() && !immediate)
    //     {
    //         StartCoroutine(FadeOut());
    //     }
    //     else if (movementSFX.isValid())
    //     {
    //         movementSFX.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    //         movementSFX.release();
    //     }
    // }


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
