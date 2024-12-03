using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum FlowerColor {R, G, B};
// Manager for the Simon Says clone
public class SquidSays : MonoBehaviour
{
    const int NUM_COLORS = 3;
    
    enum SquidMessages {FAIL, ROUND, GAME, WHOLE};
    
    [Header("Logic")]
    [SerializeField] List<int> roundLengths = new List<int>{3, 5, 7};
    [SerializeField] ChatNode squidDialogue;
    [SerializeField] TextAsset endDialogue;
    
    [Header("Display")]
    [SerializeField] TextMeshProUGUI roundText;
    [SerializeField] TextMeshProUGUI colorText;
    
    [Header("Popups")]
    [SerializeField] GameObject failPopup;
    [SerializeField] GameObject roundWinPopup;
    [SerializeField] GameObject gameWinPopup;
    [SerializeField] GameObject wholeWinPopup;
    
    [Header("Old")]
    [SerializeField][TextArea(1,5)] string startMessage = "I'm definitely the same squid as the one over there. You gotta pass 3 games of simon says but with flowers.\n";
    [SerializeField][TextArea(1,5)] string failMessage = "No, no, no! That's the wrong flower! (or a bug...)\n";
    [SerializeField][TextArea(1,5)] string roundWinMessage = "You did it!\nNext,";
    [SerializeField][TextArea(1,5)] string gameWinMessage = "That's one game down. For the next game";
    [SerializeField][TextArea(1,5)] string wholeWinMessage = "Congratulations cat! You did it! You're amazing!\nNext is the maze!";
    [SerializeField] WallBreaker gate;
    
    int _game = 0;
    int _round = 0;
    bool _gameWon = false;
    string _colorDisplay = "";
    bool _reading = false;
    
    List<int> _roundColors;
    List<int> _selectedColors;
    
    UIManager uiManager;
    ChatNode tempNode;
    
    // Start is called before the first frame update
    void Start()
    {
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        NewGame();
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
            _roundColors.Add(Random.Range(0, NUM_COLORS));
        }
        _selectedColors = new List<int>(_roundColors);
        if(_round > 1){
            Display(SquidMessages.ROUND);
        }
        roundText.text = $"Game: {_game}/{roundLengths.Count}\n Round: {_round}/{roundLengths[_game-1]}";
        squidDialogue.InjectText("[Name,Bill] "+GetSequence());
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
    
    string GetSequence(){
        string displayText = "The sequence is: ";
        for (int i = 0; i<_roundColors.Count-1; i++){
            displayText += Color2String((FlowerColor)_roundColors[i])+", ";
        }
        displayText += Color2String((FlowerColor)_roundColors[_roundColors.Count-1])+"!";
        return displayText;
    }
    
    void Display(SquidMessages message){
        if (_reading) return;
        
        if (_gameWon){
            Instantiate(wholeWinPopup);
            return;
            // uiManager.DisplayText(wholeWinMessage);
        }
        else if(message == SquidMessages.FAIL){
            tempNode = Instantiate(failPopup, transform.position, transform.rotation).GetComponent<ChatNode>();
        }
        else if (message == SquidMessages.ROUND){
            tempNode = Instantiate(roundWinPopup, transform.position, transform.rotation).GetComponent<ChatNode>();
        }
        else if (message == SquidMessages.GAME){
            tempNode = Instantiate(gameWinPopup, transform.position, transform.rotation).GetComponent<ChatNode>();
        }
        else if (message == SquidMessages.WHOLE){
            Instantiate(wholeWinPopup);
            return;
        }
        // else{
        //     uiManager.DisplayText(message+GetSequence());
        // }
        
        StartCoroutine(WaitToInteract());
        _reading = true;
    }
    
    IEnumerator WaitToInteract(){
        yield return null;
        tempNode.InjectText("[Name,Bill] "+GetSequence());
        tempNode.callback.AddListener(KillNode);
        tempNode.Interact();
    }
    
    void WinGame(){
        squidDialogue.conversation = endDialogue;
        squidDialogue.CreatePopups();
        gate?.BreakWall();
    }
    
    public void KillNode(){
        Destroy(tempNode.gameObject);
        tempNode = null;
        _reading = false;
    }
    
    public void GetFlower(FlowerColor fc){
        if (_reading) return;
        if (_gameWon){
            Display(SquidMessages.WHOLE);
        }
        else if(_selectedColors[0] == (int)fc){
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
            _colorDisplay = "";
            colorText.text = _colorDisplay;
            _selectedColors = new List<int>(_roundColors);
            Display(SquidMessages.FAIL);
        }
    }
}
