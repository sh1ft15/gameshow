using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorScript : MonoBehaviour
{
    // Update is called once per frame
    void Update(){
        Vector2 post = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        transform.position = post;
    }
}
