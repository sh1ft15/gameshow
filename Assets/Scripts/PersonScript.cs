using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonScript : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform character, choice;
    [SerializeField] Text nameTxt, scoreTxt;
    [SerializeField] Image scoreBack;
    [SerializeField] ParticleSystem particle;
    int score = 0;

    public void SetName(string txt){
        nameTxt.text = txt;
    }

    public void UpdateScore(int num = 0){
        score = Mathf.Max(score + num, 0);
        scoreTxt.text = score.ToString();
    }

    public void SetScore(int num = 0){
        score = num;
    }

    public void SetScoreColor(Color color){
        color.a = 0.2f;
        scoreBack.color = color;
    }

    public void PlayAnim(string str){
        animator.Play(str);
    }

    public void TriggerAnim(string str){
        animator.SetTrigger(str);
    }

    public void SetChoice(string str = ""){
        choice.gameObject.SetActive(str.Length > 0);
        choice.GetComponent<ChoiceScript>().SetLetter(str);
    }

    public void PlayParticle(Color color){
        ParticleSystem.MainModule main = particle.main;
        
        main.startColor = color;
        particle.Play();
    }

    public void StopParticle(){
        particle.Stop();
    }
}
