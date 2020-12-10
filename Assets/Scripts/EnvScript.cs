using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class EnvScript : MonoBehaviour
{
    [SerializeField] SpriteRenderer bgRenderer;
    [SerializeField] Light2D bgLight;
    [SerializeField] GameObject table;
    [SerializeField] List<Sprite> bgSprites;

    void Awake(){
        ChangeScene(0);
    }

    public void ChangeScene(int bgIndex = 0){
        if (bgIndex >= 0 && bgIndex < bgSprites.Count){
            bgRenderer.sprite = bgSprites[bgIndex];
            table.SetActive(bgIndex == 0);
        }
    }

}
