using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceScript : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Text text;
    string letter;

    void Awake(){
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        text.color = new Color(0, 0, 0, 0.7f);
    }

    public void OnTriggerEnter2D(Collider2D col){
        switch(col.tag){
            case "Cursor":
                spriteRenderer.color = new Color(1, 1, 1);
                text.color = new Color(0, 0, 0);
            break;
        }
    }

    public void OnTriggerExit2D(Collider2D col){
        switch(col.tag){
            case "Cursor":
                spriteRenderer.color = new Color(1, 1, 1, 0.5f);
                text.color = new Color(0, 0, 0, 0.7f);
            break;
        }
    }

    public void SetLetter(string str){
        letter = str;
        text.text = letter;
    }

    public string GetLetter() { return letter; }

    public Transform GetPerson() { return transform.parent; }
}
