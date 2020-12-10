using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TVScript : MonoBehaviour
{
    [SerializeField] Transform topLettersContainer, bottomLettersContainer, dialogBox, introBox;
    [SerializeField] Image overlayImage;
    [SerializeField] PersonsScript personsScript;
    [SerializeField] AudioScript audioScript;
    [SerializeField] EnvScript envScript;
    [SerializeField] LayerMask cursorMask;
    List<string[]> questions, gameSequence1, gameSequence2;
    List<Transform> topLetters, bottomLetters;
    List<string> choices;
    string[] dialogs, dialogSequence1, dialogSequence2, dialogSequence3, sceneSequence, stringPair;
    string titleIntro, round1Intro, round2Intro, round3Intro, creditIntro;
    int maxTxtLength = 40, questionIndex = 0, dialogIndex = 0, sceneIndex = 0;
    bool dialogMode, introMode, gameMode, transitionMode;
    Coroutine transCoroutine = null;

    void Awake(){
        topLetters = new List<Transform>();
        bottomLetters = new List<Transform>();

        foreach(Transform letter in topLettersContainer){ topLetters.Add(letter); }

        foreach(Transform letter in bottomLettersContainer){ bottomLetters.Add(letter); }

        sceneSequence = new string[]{
            "title_intro",
            "dialog_sequence_1",
            "round_intro_1",
            "game_sequence_1",
            "round_intro_2",
            "dialog_sequence_2",
            "game_sequence_2",
            "round_intro_3",
            "dialog_sequence_3",
            "credit_intro"
        };

        titleIntro = "Our Little Gameshow";
        round1Intro = "Round One Start !";
        round2Intro = "Round Two Start !";
        round3Intro = "Round Three ...";
        creditIntro = "Game Over";

        dialogSequence1 = new string[]{
            "Welcome to our little gameshow. We have \nthree guests with us tonight.",
            "The rule is simple, each round present \nyou with a pair of incomplete words.",
            "The participant need to guess the missing \nalphabet to complete one or both of \nthese words.",
            "Each correct guess will grant the \nparticipant one point.",
            "Completing one word out of the pair will \nend the round and move the stage to \nnext level.",
            "Completing both words at the same time \ngrant the participant extra point.",
            "Okay, thats it. Let's start the show!"
        };

        dialogSequence2 = new string[]{
            "Gentlements..",
            "On December 9, 2020. Two bodies were \nfound at the back alley down the \nstreet of Northen Eve, Riverdale.",
        };

        dialogSequence3 = new string[]{
            "Ms. Taylor 36 and her daughter Emily 6, \nwere found with heavy bruises on \nseveral part of their body",
            "Mr.Taylor, did you enjoy our little gameshow?"
        };

        gameSequence1 = new List<string[]>{
            new string[]{"It contradict each other.", "positive", "negative"},
            new string[]{"The purpose of lifting things.", "raise", "lower"},
            new string[]{"Act of exchanging items.", "sell", "buy"},
            new string[]{"Position of an object.", "first", "last"},
            new string[]{"Quantity or mass of things.", "less", "more"},
            new string[]{"Condition of time or object.", "short", "long"},
            new string[]{"Control over things.", "start", "stop"},
            new string[]{"An event that might occur or already occured.", "before", "after"},
            new string[]{"The circle of life.", "birth", "death"}
        };

        gameSequence2 = new List<string[]>{
            new string[]{"What did you told the victims?", "truth", "false"},
            new string[]{"How did you behave toward the victims?", "rough", "brutal"},
            new string[]{"What're the condition of the victims \nbefore the assault?", "injured", "healthy"},
            new string[]{"What type of weapon involved?", "sharp", "blunt"},
            new string[]{"What's the condition of the weapon?", "perfect", "faulty"},
            new string[]{"How far did the victims struggle \nagainst you?", "strong", "docile"},
            new string[]{"How do you feel?", "dismay", "denial"},
        };

        personsScript.InitPersons();
        StartCoroutine(UpdateGameScene(0));
    }

    void Update(){
        if (transitionMode) { return; }

        if (Input.GetMouseButtonUp(0)){
            if (gameMode || dialogMode) { ClickOnTarget(); }
            else if (introMode) { StartCoroutine(UpdateGameScene(1)); }
            else { 
                sceneIndex = 0;
                StartCoroutine(UpdateGameScene(0));
            }
        }
    }

    IEnumerator UpdateGameScene(int increment = 0, bool init = false){
        int temp = sceneIndex + increment;
        string scene = "",
               introTxt = "";

        introMode = dialogMode = gameMode = false;

        if (temp >= sceneSequence.Length) { sceneIndex = 0; }
        else { sceneIndex = temp; }

        scene = sceneSequence[sceneIndex];

        audioScript.PlayClip("move_scene");

        switch(scene){
            case "title_intro":  
                envScript.ChangeScene(0);
                introMode = true;
                UpdateDialog("");
                UpdateIntro(titleIntro, 1, 0.7f);
            break;
            case "dialog_sequence_1":
            case "dialog_sequence_2":
            case "dialog_sequence_3":
                if (transCoroutine == null) { 
                    yield return StartCoroutine(TransitionScene(1));

                    if (scene.Equals("dialog_sequence_1")) {
                        dialogs = dialogSequence1;
                        personsScript.ChangeScene(0);
                        envScript.ChangeScene(0);
                    }
                    else if (scene.Equals("dialog_sequence_2")) {
                        dialogs = dialogSequence2;
                        envScript.ChangeScene(1);
                    }
                    else if (scene.Equals("dialog_sequence_3")) {
                        dialogs = dialogSequence3;
                        personsScript.ChangeScene(1);
                    }

                    dialogMode = true;
                    dialogIndex = 0; 
                    UpdateIntro("");
                    UpdateDialog(dialogs[dialogIndex]);
                    personsScript.HideChoices();
                }
            break;
            case "game_sequence_1": 
            case "game_sequence_2":
                gameMode = true;
                questionIndex = 0;
                questions = scene.Equals("game_sequence_1")? gameSequence1 : gameSequence2;
                UpdateIntro("");
                UpdateQuestion(questionIndex);
            break;
            case "round_intro_1": 
            case "round_intro_2": 
            case "round_intro_3":
            case "credit_intro": 
                if (scene.Equals("round_intro_1")) { introTxt = round1Intro; }
                else if (scene.Equals("round_intro_2")) { introTxt = round2Intro; }
                else if (scene.Equals("round_intro_3")) { introTxt = round3Intro; }
                else if (scene.Equals("credit_intro")) { introTxt = creditIntro; }

                introMode = true;
                UpdateDialog("");
                UpdateIntro(introTxt, 1, 0.7f);
            break;
        }
    }

    IEnumerator TransitionScene(float delay = 1){
        if (transitionMode == false) {
            Color originalColor = new Color(.2f, .2f, .2f, 0.1f),
                  newColor = new Color(.2f, .2f, .2f);
            float t = 0,
                  duration = 0.2f;

            transitionMode = true;

            while (t <= 1.0) {
                t += Time.deltaTime / duration;
                overlayImage.color = Color.Lerp(originalColor, newColor, t);
                yield return null;
            }

            overlayImage.color = newColor;

            yield return new WaitForSeconds(delay);
            t = 0;

            while (t <= 1.0) {
                t += Time.deltaTime / duration;
                overlayImage.color = Color.Lerp(newColor, originalColor, t);
                yield return null;
            }

            transitionMode = false;
            transCoroutine = null;
        }
        
    }

    void UpdateIntro(string str, float barOpacity = 0, float backOpacity = 0){
        Text text = introBox.Find("Text").GetComponent<Text>();

        if (str.Length > 0) {
            Image background = introBox.Find("Image").GetComponent<Image>(),
                  bar = introBox.Find("Bar").GetComponent<Image>();
            Color backColor = background.color,
                  barColor = bar.color;

            backColor.a = backOpacity;
            barColor.a = barOpacity;
            
            text.text = str;
            bar.color = barColor;
            background.color = backColor;
        }

        introBox.gameObject.SetActive(str.Length > 0);
    }

    void TriggerDialogs(){
        if (dialogMode) {
            int temp = dialogIndex + 1;
            
            if (dialogIndex < dialogs.Length) {
                UpdateDialog(dialogs[dialogIndex]);
            }
            
            if (temp > dialogs.Length) { StartCoroutine(UpdateGameScene(1)); }
            else { dialogIndex = temp; }

            audioScript.PlayClip("dialog");
        }
    }

    void ClickOnTarget(){
        Transform obj = GetObjectOnCursor(); 

        if (obj == null) { return; }
        
        switch(obj.tag){
            case "Dialog": if (dialogMode) { TriggerDialogs(); } break;
            case "Choice":
                if (gameMode) {
                    ChoiceScript choiceScript = obj.GetComponent<ChoiceScript>();

                    RevealChoice(choiceScript.GetPerson(), choiceScript.GetLetter());
                }
            break;
        }
    }

    Transform GetObjectOnCursor(){
        Vector3 mouse_post = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(mouse_post.x, mouse_post.y), 
            Vector2.zero, cursorMask);

        return hit.transform ?? null;
    }

    public void UpdateQuestion(int index = 0){
        if (index >= 0 && index < questions.Count){
            string[] strings = questions[index];

            choices = new List<string>();
            stringPair = strings;

            UpdateDialog(strings[0]);
            UpdateIntro("");
            UpdateLetters(topLetters, strings[1]);
            UpdateLetters(bottomLetters, strings[2]);
            HideChoices();

            choices = ShuffleStringList(choices);

            personsScript.UpdateChoices(choices);
        }
        else { UpdateDialog("Something went wrong ..."); }
    }

    public void UpdateLetters(List<Transform> boxes, string str){
        if (str.Length <= boxes.Count){
            for(int i = 0; i < boxes.Count; i++){
                Transform box = boxes[i];
                LetterBoxScript script = box.GetComponent<LetterBoxScript>();

                if (i < str.Length){
                    string temp = str.Substring(i, 1).ToUpper();
                    
                    script.SetTextColor(Color.black);
                    script.SetLetter(temp);
                    script.ToggleLetter(true);
                    box.gameObject.SetActive(true);
                }
                else { 
                    script.SetLetter("");
                    script.ToggleLetter(false);
                    box.gameObject.SetActive(false); 
                }
            }
        }
    }

    public void HideChoices(){
        List<Transform> tempTopLetters = new List<Transform>(topLetters),
                        tempBtmLetters = new List<Transform>(bottomLetters);
        float topCount, btmCount;
        
        tempTopLetters = tempTopLetters.FindAll(tpl => tpl.gameObject.activeSelf);
        tempBtmLetters = tempBtmLetters.FindAll(btl => btl.gameObject.activeSelf);
        topCount = Mathf.Max(0.3f * tempTopLetters.Count, 1);
        btmCount =  Mathf.Max(0.3f * tempBtmLetters.Count, 1);
        tempTopLetters = ShuffleTransformList(tempTopLetters);
        tempBtmLetters = ShuffleTransformList(tempBtmLetters);

        for(int i = 0; i < Mathf.RoundToInt(topCount); i++){
            Transform topLetter = tempTopLetters[i];
            LetterBoxScript script = topLetter.GetComponent<LetterBoxScript>();
            
            BuildChoices(script.GetLetter());
        }

        for(int i = 0; i < Mathf.RoundToInt(btmCount); i++){
            Transform btmLetter = tempBtmLetters[i];
            LetterBoxScript script = btmLetter.GetComponent<LetterBoxScript>();

            BuildChoices(script.GetLetter());
        }

        foreach(string choice in choices){
            foreach(Transform topLetter in topLetters){
                LetterBoxScript script = topLetter.GetComponent<LetterBoxScript>();

                if (script.IsLetter(choice)){ script.ToggleLetter(false); }
            }

            foreach(Transform bottomLetter in bottomLetters){
                LetterBoxScript script = bottomLetter.GetComponent<LetterBoxScript>();

                if (script.IsLetter(choice)){ script.ToggleLetter(false); }
            }
        }
    }

    public void RevealChoice(Transform person, string str){
        int index = choices.FindIndex(choice => choice.Equals(str)),
            score = 0;

        if (index != -1){
            string topString = "",
                   btmString = "";

            foreach(Transform topLetter in topLetters){
                LetterBoxScript script = topLetter.GetComponent<LetterBoxScript>();

                if (!script.IsVisible() && script.IsLetter(str)) {
                    script.ToggleLetter(true);
                    script.SetTextColor(new Color(0, 0, 1));
                    topString += script.GetLetter();
                }
                else {
                    if (script.IsVisible() && topLetter.gameObject.activeSelf){
                        topString += script.GetLetter();
                    }
                }
            }

            foreach(Transform bottomLetter in bottomLetters){
                LetterBoxScript script = bottomLetter.GetComponent<LetterBoxScript>();

                if (!script.IsVisible() && script.IsLetter(str)) {
                    script.ToggleLetter(true);
                    script.SetTextColor(new Color(0, 0, 1));
                    btmString += script.GetLetter();
                }
                else {
                    if (script.IsVisible() && bottomLetter.gameObject.activeSelf){
                        btmString += script.GetLetter();
                    }
                }
            }

            topString = topString.ToLower();
            btmString = btmString.ToLower();

            score = stringPair[1].Equals(topString) && stringPair[2].Equals(btmString)? 2 : 1;

            if (stringPair[1].Equals(topString) || stringPair[2].Equals(btmString)){
                // move to next stage
                StartCoroutine(MoveStage(new bool[]{
                    stringPair[1].Equals(topString), 
                    stringPair[2].Equals(btmString)
                }));

                audioScript.PlayClip("move_stage");
            }
            else {
                choices.RemoveAt(index);
                choices = ShuffleStringList(choices);
                personsScript.UpdateChoices(choices);
                audioScript.PlayClip("correct");
            }
        }
        // wrong answer
        else { 
            score = -1;
            StartCoroutine(WrongAnswer(0.5f)); 
            audioScript.PlayClip("wrong");
        }

        personsScript.StartCoroutine(personsScript.TriggerRate(person, score));
    }

    IEnumerator WrongAnswer(float delay = 1){
        Color defColor = Color.black,
              color = Color.red;

        personsScript.UpdateChoices(new List<string>());

        foreach(Transform topLetter in topLetters){
            topLetter.GetComponent<LetterBoxScript>().SetTextColor(color);
        }

        foreach(Transform bottomLetter in bottomLetters){
            bottomLetter.GetComponent<LetterBoxScript>().SetTextColor(color);
        }

        yield return new WaitForSeconds(delay);

        foreach(Transform topLetter in topLetters){
            topLetter.GetComponent<LetterBoxScript>().SetTextColor(defColor);
        }

        foreach(Transform bottomLetter in bottomLetters){
            bottomLetter.GetComponent<LetterBoxScript>().SetTextColor(defColor);
        }

        choices = ShuffleStringList(choices);
        personsScript.UpdateChoices(choices);
    }

    IEnumerator MoveStage(bool[] stringCompletes, float delay = 1){
        Color topColor = stringCompletes[0]? new Color(0, .5f, 0) : Color.red,
              btmColor = stringCompletes[1]? new Color(0, .5f, 0) : Color.red;

        personsScript.UpdateChoices(new List<string>());

        foreach(Transform topLetter in topLetters){
            topLetter.GetComponent<LetterBoxScript>().SetTextColor(topColor);
        }

        foreach(Transform bottomLetter in bottomLetters){
            bottomLetter.GetComponent<LetterBoxScript>().SetTextColor(btmColor);
        }

        yield return new WaitForSeconds(delay);

        questionIndex += 1;

        if (questionIndex >= questions.Count) { StartCoroutine(UpdateGameScene(1)); }
        else { UpdateQuestion(questionIndex); }
    }

    public void BuildChoices(string str){
        int index = choices.FindIndex(choice => choice.Equals(str));

        if (index == -1) { choices.Add(str); }
    }

    public void UpdateDialog(string str = ""){
        Text text = dialogBox.Find("Canvas/Text").GetComponent<Text>();
        string[] strs = SplitStringByLength(str, maxTxtLength);
        string fullStr = "";

        dialogBox.gameObject.SetActive(str.Length > 0);

        foreach(string tempStr in strs){
            fullStr += tempStr.Trim() + "\n";
        }

        text.text = str;
    }

    public static string[] SplitStringByLength(string value, int length){
        int strLength = value.Length;
        int strCount = (strLength + length - 1) / length;
        string[] result = new string[strCount];

        for (int i = 0; i < strCount; ++i) {
            result[i] = value.Substring(i * length, Mathf.Min(length, strLength));
            strLength -= length;
        }
        return result;
    }

    public List<string> ShuffleStringList(List<string> list){
        for (int i = 0; i < list.Count; i++) {
            string temp = list[i];
            int randomIndex = Random.Range(i, list.Count);

            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }

        return list;
    }

    public List<Transform> ShuffleTransformList(List<Transform> list){
        for (int i = 0; i < list.Count; i++) {
            Transform temp = list[i];
            int randomIndex = Random.Range(i, list.Count);

            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }

        return list;
    }
}
