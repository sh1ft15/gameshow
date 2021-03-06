﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioScript : MonoBehaviour
{
    [SerializeField] AudioSource sfxSource;
    [SerializeField] Button muteButton;
    [SerializeField] List<Sprite> sprites;
    [SerializeField] List<AudioClip> clips;
    bool isMuted = false;
    
    void Awake(){
        muteButton.onClick.AddListener(() => {
            isMuted = !isMuted;
            muteButton.GetComponent<Image>().sprite = sprites[isMuted? 1 : 0];
            AudioListener.volume = isMuted? 0 : 1;
        });
    }   

    public AudioClip GetClip(string name){
        return clips.Find(clip => clip.name.Equals(name));
    }

    public void PlayClip(string name){
        sfxSource.clip = GetClip(name);
        sfxSource.Play();
    }
}
