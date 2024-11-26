using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FlowerColor {R, G, B};
// Manager for the Simon Says clone
public class SquidSays : MonoBehaviour, IInteractable
{
    const int NUM_COLORS = 3;
    
    [SerializeField] List<int> roundLengths = new List<int>{3, 5, 7};
    [SerializeField][TextArea(1,5)] string startMessage = "I'm definitely the same squid as the one over there. You gotta pass 3 games of simon says but with flowers.\n";
    [SerializeField][TextArea(1,5)] string failMessage = "No, no, no! That's the wrong flower! (or a bug...)\n";
    [SerializeField][TextArea(1,5)] string roundWinMessage = "You did it!\nNext,";
    [SerializeField][TextArea(1,5)] string gameWinMessage = "That's one game down. For the next game";
    [SerializeField][TextArea(1,5)] string wholeWinMessage = "Congratulations cat! You did it! You're amazing!\nNext is the maze!";
    [SerializeField] WallBreaker gate;
    [SerializeField] TempUI tempUI;
    
    int _game = 0;
    int _round = 0;
    bool _gameWon = false;
    string _colorDisplay = "";
    
    List<int> _roundColors;
    List<int> _selectedColors;
    
    // Start is called before the first frame update
    void Start()
    {
        NewGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void NewGame(string mess = ""){
        if(_game >= roundLengths.Count){
            _gameWon = true;
            WinGame();
            Display(wholeWinMessage);
            return;
        }
        _game++;
        _round = 0;
        NewSequence();
        if (mess != ""){
            string newMessage = mess + $" You're on Game {_game}, Round {_round}.";
            // Debug.Log(newMessage);
            Display(newMessage);
        }
    }
    
    void NewSequence(){
        if (_round >= roundLengths[_game-1]){
            NewGame(gameWinMessage);
            return;
        }
        _round++;
        _roundColors = new List<int>();
        for (int i=0; i<_round; i++){
            _roundColors.Add(Random.Range(0, NUM_COLORS));
        }
        _selectedColors = new List<int>(_roundColors);
        if(_round > 1){
            Display(roundWinMessage);
        }
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
        string displayText = " The sequence is: ";
        for (int i = 0; i<_roundColors.Count-1; i++){
            displayText += Color2String((FlowerColor)_roundColors[i])+", ";
        }
        displayText += Color2String((FlowerColor)_roundColors[_roundColors.Count-1])+"!";
        return displayText;
    }
    
    void Display(string message){
        if (_gameWon){
            tempUI.DisplayText(wholeWinMessage);
        }
        else{
            tempUI.DisplayText(message+GetSequence());
        }
    }
    
    void WinGame(){
        gate?.BreakWall();
    }
    
    public void GetFlower(FlowerColor fc){
        if(_selectedColors[0] == (int)fc){
            _colorDisplay += Color2String((FlowerColor)_selectedColors[0])+"\n";
            _selectedColors.RemoveAt(0);
            // If end of sequence create new sequence
            if (_selectedColors.Count == 0){
                _colorDisplay = "";
                NewSequence();
            }
            tempUI.SetUITextDisplay(_colorDisplay);
        }
        else {
            // Reset
            _colorDisplay = "";
            tempUI.SetUITextDisplay(_colorDisplay);
            _selectedColors = new List<int>(_roundColors);
            Display(failMessage);
        }
    }
    
    public void Interact(){
        Display(startMessage+$" You're on Game {_game}, Round {_round}.");
    }
}
