using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonsScript : MonoBehaviour
{
    [SerializeField] Transform ratePrefab;
    [SerializeField] List<PersonScript> personScripts;
    List<Person> persons;
    string[] alphabets;

    public void InitPersons(){
        alphabets = new string[26] {"A","B","C","D","E","F","G","H","I","J","K","L","M",
            "N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};

        persons = new List<Person>(){
            new Person{
                name = "Samuel",
                charName = "person_1_idle",
                score = 0,
            },
            new Person{
                name = "Taylor",
                charName = "person_2_idle",
                score = 0,
            },
            new Person{
                name = "Michael",
                charName = "person_3_idle",
                score = 0,
            }
        };


        for(int i = 0; i < persons.Count; i++){
            PersonScript script = personScripts[i];
            Person person = persons[i];

            script.SetName(person.name);
            script.UpdateScore(person.score);
            script.PlayAnim(person.charName);
            script.SetChoice("");

            person.script = script;
            persons[i] = person;
        }
    }

    public void ChangeScene(int state = 0){
        for(int i = 0; i < persons.Count; i++){
            Person person = persons[i];
            PersonScript script = person.script;

            person.score = 0;
            persons[i] = person;
            script.SetScore(0);
            script.UpdateScore();

            if (state == 0){
                script.PlayAnim(person.charName);
                script.SetScoreColor(Color.green);
            }
            else if (state == 1){
                if (!person.name.Equals("Taylor")) { script.TriggerAnim("death"); }
                
                script.SetScoreColor(Color.red);
            }
        }
    }

    public void UpdateChoices(List<string> choices){
        List<Person> tempPersons = new List<Person>(persons);

        tempPersons = ShufflePersonList(tempPersons);
        
        for(int i = 0; i < tempPersons.Count; i++){
            PersonScript script = tempPersons[i].script;
            
            if (i < choices.Count) { script.SetChoice(choices[i]); }
            else { script.SetChoice(alphabets[Random.Range(0, alphabets.Length - 1)]); }
        }
    }

    public void HideChoices(){
        foreach(Person person in persons){
            person.script.SetChoice("");
        }
    }

    public List<Person> ShufflePersonList(List<Person> list){
        for (int i = 0; i < list.Count; i++) {
            Person temp = list[i];
            int randomIndex = Random.Range(i, list.Count);

            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }

        return list;
    }

    public IEnumerator TriggerRate(Transform person, int num = 0){
        int personIndex = persons.FindIndex(p => p.script.transform == person);
       
        if (personIndex != -1){
            Vector3 post = person.position + new Vector3(0, 1.5f);
            Person personAttr = persons[personIndex];
            PersonScript script = personAttr.script;
            Transform rate = Instantiate(ratePrefab, post, Quaternion.identity);
            Text rateLabel = rate.Find("Canvas/Text").GetComponent<Text>();
           
            personAttr.score =  Mathf.Max(personAttr.score + num, 0);
            persons[personIndex] = personAttr;

            rateLabel.color = num >= 0? Color.green : Color.red;
            rateLabel.text = num.ToString();
            script.UpdateScore(num);
            script.PlayParticle(num >= 0? Color.green : Color.red);
            rate.GetComponent<Animator>().SetTrigger("fade");

            yield return new WaitForSeconds(1);
            script.StopParticle();
            Destroy(rate.gameObject);
        }
    }
}

public class Person{
    public string name, charName;
    public int score;
    public PersonScript script;
}
