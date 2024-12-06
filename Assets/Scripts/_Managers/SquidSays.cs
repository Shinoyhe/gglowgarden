using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public enum FlowerColor {R, G, B};
// Manager for the Simon Says clone
public class SquidSays : MonoBehaviour
{
    const int NUM_COLORS = 3;
    
    enum SquidMessages {START, FAIL, ROUND, GAME, WHOLE};
    
    [Header("Logic")]
    [SerializeField] List<int> roundLengths = new List<int>{3, 5, 7};
    public List<GameObject> flowers = new List<GameObject>();
    [SerializeField] GameObject gglowOrb;
    [SerializeField] ChatNode squidDialogue;
    [SerializeField] TextAsset midDialogue;
    [SerializeField] TextAsset gameEndDialogue;
    [SerializeField] TextAsset trueEndDialogue;
    [SerializeField] WallBreaker gate;
    [SerializeField] UnityEvent endCallbacks;
    
    [Header("Display")]
    [SerializeField] TextMeshProUGUI roundText;
    [SerializeField] TextMeshProUGUI colorText;
    
    [Header("Popups")]
    [SerializeField] TextAsset startPopup1;
    [SerializeField] TextAsset failPopup1;
    [SerializeField] TextAsset roundWinPopup1;
    [SerializeField] TextAsset gameWinPopup1;
    [SerializeField] GameObject wholeWinPopup;
    
    int _game = 0;
    int _round = 0;
    bool _gameWon = false;
    string _colorDisplay = "";
    bool _reading = false;
    bool _started = false;
    TextAsset _currDialogue;
    
    List<int> _roundColors;
    List<int> _selectedColors;
    
    UIManager uiManager;
    ChatNode tempNode;
    
    // Start is called before the first frame update
    void Start()
    {
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        NewGame();
        
        squidDialogue.callback.AddListener(KillNode);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void NewGame(bool mess = false){
        if(_game >= roundLengths.Count){
            _gameWon = true;
            WinGame();
            Display(SquidMessages.WHOLE);
            return;
        }
        _game++;
        _round = 0;
        NewSequence();
        if (mess){
            Display(SquidMessages.GAME);
        }
    }
    
    void NewSequence(){
        if (_round >= roundLengths[_game-1]){
            NewGame(true);
            return;
        }
        _round++;
        _roundColors = new List<int>();
        for (int i=0; i<_round; i++){
            _roundColors.Add(UnityEngine.Random.Range(0, NUM_COLORS));
        }
        _selectedColors = new List<int>(_roundColors);
        if(_round > 1){
            Display(SquidMessages.ROUND);
        }
        roundText.text = $"Game: {_game}/{roundLengths.Count}\n Round: {_round}/{roundLengths[_game-1]}";
        squidDialogue.InjectText("[Name,Bill] "+GetSequence());
    }
    
    void ResetSequence(){
        _colorDisplay = "";
        colorText.text = _colorDisplay;
        _selectedColors = new List<int>(_roundColors);
    }
    
    string Color2String(FlowerColor fcol){
        switch (fcol)
        {
            case FlowerColor.R:
                return "<color=\"red\">red</color>";
            case FlowerColor.G:
                return "<color=\"green\">green</color>";
            case FlowerColor.B:
                return "<color=\"blue\">blue</color>";
            default:
                return "<color=\"red\">red</color>";
        }
    }
    
    string FoolColor2String(FlowerColor fcol){
        string randColor = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1, 1, 1).ToHexString();
        switch (fcol)
        {
            case FlowerColor.R:
                return $"<color=#{randColor}>red</color>";
            case FlowerColor.G:
                return $"<color=#{randColor}>green</color>";
            case FlowerColor.B:
                return $"<color=#{randColor}>blue</color>";
            default:
                return $"<color=#{randColor}>red</color>";
        }
    }
    
    string GetSequence(){
        Func<FlowerColor, string> ColorFunc = (_game>=3 && (_round < 3 || _round == 5)) ? FoolColor2String : Color2String;
        string displayText = "The sequence is: ";
        for (int i = 0; i<_roundColors.Count-1; i++){
            displayText += ColorFunc((FlowerColor)_roundColors[i])+", ";
        }
        displayText += ColorFunc((FlowerColor)_roundColors[_roundColors.Count-1])+"!";
        return displayText;
    }
    
    void Display(SquidMessages message){
        if (_reading) return;
        
        _currDialogue = squidDialogue.conversation;
        
        if (_gameWon){
            Instantiate(wholeWinPopup);
            return;
        }
        else if(message == SquidMessages.START){
            squidDialogue.conversation = startPopup1;
            squidDialogue.CreatePopups();
        }
        else if(message == SquidMessages.FAIL){
            squidDialogue.conversation = failPopup1;
        }
        else if (message == SquidMessages.ROUND){
            squidDialogue.conversation = roundWinPopup1;
        }
        else if (message == SquidMessages.GAME){
            squidDialogue.conversation = gameWinPopup1;
        }
        else if (message == SquidMessages.WHOLE){
            Instantiate(wholeWinPopup);
            return;
        }
        
        if(message != SquidMessages.START) squidDialogue.InjectText("[Name,Bill] "+GetSequence());
        squidDialogue.Interact();
        _reading = true;
    }
    
    void WinGame(){
        squidDialogue.conversation = gameEndDialogue;
        squidDialogue.CreatePopups();
        gglowOrb.SetActive(true);
    }
    
    IEnumerator SetFirstDialogueC(){
        yield return new WaitForSeconds(3f);
        squidDialogue.InjectText("[Name,Bill] "+GetSequence());
    }
    
    public void SetFirstDialogue(){
        StartCoroutine(SetFirstDialogueC());
    }
    
    public void EndGame(){
        squidDialogue.conversation = trueEndDialogue;
        squidDialogue.CreatePopups();
        squidDialogue.callback = endCallbacks;
        squidDialogue.Interact();
        gate?.BreakWall();
        foreach (GameObject flower in flowers){
            flower.layer = 0;
        }
    }
    
    public void KillNode(){
        if (_reading){
            _reading = false;
            squidDialogue.conversation = _currDialogue;
            squidDialogue.InjectText("[Name,Bill] "+GetSequence());
        }
        if (!_gameWon){
            ResetSequence();
        }
    }
    
    public void GetFlower(FlowerColor fc){
        if (_reading) return;
        if (!_started){
            Display(SquidMessages.START);
            return;
        }
        else if (_gameWon){
            Display(SquidMessages.WHOLE);
        }
        else if(_selectedColors[0] == (int)fc){
            SoundManager.Instance.PlaySquidSaysSFX();
            _colorDisplay += Color2String((FlowerColor)_selectedColors[0])+" ";
            _selectedColors.RemoveAt(0);
            // If end of sequence create new sequence
            if (_selectedColors.Count == 0){
                _colorDisplay = "";
                NewSequence();
            }
            colorText.text = _colorDisplay;
        }
        else {
            // Reset
            ResetSequence();
            Display(SquidMessages.FAIL);
        }
    }
    
    public void SetStart(){
        if (!_started && squidDialogue.conversation != startPopup1) {
            _started = true;
            squidDialogue.conversation = midDialogue;
            squidDialogue.InjectText("[Name,Bill] "+GetSequence());
        }
    }
}
