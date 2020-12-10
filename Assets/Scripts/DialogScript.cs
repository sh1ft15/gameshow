using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogScript : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    string letter;

    void Awake(){
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
    }

    public void OnTriggerEnter2D(Collider2D col){
        switch(col.tag){
            case "Cursor":
                spriteRenderer.color = new Color(1, 1, 1);
            break;
        }
    }

    public void OnTriggerExit2D(Collider2D col){
        switch(col.tag){
            case "Cursor":
                spriteRenderer.color = new Color(1, 1, 1, 0.7f);
            break;
        }
    }
}
