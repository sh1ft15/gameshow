using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterBoxScript : MonoBehaviour
{
    [SerializeField] Text letterLabel;
    bool isVisible;
    string letter;

    void Awake() { 
        isVisible = false;
        letter = ""; 
    }
    
    public void SetLetter(string str){
        letter = str;
        letterLabel.text = str;
    }

    public void ToggleLetter(bool status){
        letterLabel.text = status? letter : "";
        isVisible = status;
    }

    public void SetTextColor(Color color){
        letterLabel.color = color;
    }

    public bool IsLetter(string str){
        return letter.Equals(str);
    }

    public bool IsVisible(){
        return isVisible;
    }

    public string GetLetter() { return letter; }
}
