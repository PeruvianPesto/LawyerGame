using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using Ink.Runtime;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextAsset inkJSON;
    public GameObject textBox;
    public GameObject customButton;
    public GameObject optionPanel;
    public bool isTalking = false;

    static Story story;
    TextMeshProUGUI nametag; // Use TextMeshProUGUI for TextMeshPro components
    TextMeshProUGUI message; // Use TextMeshProUGUI for TextMeshPro components
    List<string> tags;
    static Choice choiceSelected;

    void Start()
    {
        story = new Story(inkJSON.text);
        nametag = textBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        message = textBox.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        tags = new List<string>();
        choiceSelected = null;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (story.canContinue)
            {
                nametag.text = "Lawyer";
                AdvanceDialogue();
            }
        }
    }

    private void AdvanceDialogue()
    {
        string currentSentence = story.Continue();
        ParseTags();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentSentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        message.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            message.text += letter;
            yield return null;
        }

        CharacterScript tempSpeaker = GameObject.FindObjectOfType<CharacterScript>();
        if (tempSpeaker.isTalking)
        {
            SetAnimation("idle");
        }
        yield return null;
    }

    IEnumerator ShowChoices()
    {
        Debug.Log("There are choices need to be made here!");
        List<Choice> _choices = story.currentChoices;

        for (int i = 0; i < _choices.Count; i++)
        {
            GameObject temp = Instantiate(customButton, optionPanel.transform);
            temp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _choices[i].text;
            temp.AddComponent<Selectable>();
            temp.GetComponent<Selectable>().element = _choices[i];
            temp.GetComponent<Button>().onClick.AddListener(() => { temp.GetComponent<Selectable>().Decide(); });
        }

        optionPanel.SetActive(true);

        yield return new WaitUntil(() => { return choiceSelected != null; });

        AdvanceFromDecision();
    }

    public static void SetDecision(object element)
    {
        choiceSelected = (Choice)element;
        story.ChooseChoiceIndex(choiceSelected.index);
    }

    void AdvanceFromDecision()
    {
        optionPanel.SetActive(false);
        for (int i = 0; i < optionPanel.transform.childCount; i++)
        {
            Destroy(optionPanel.transform.GetChild(i).gameObject);
        }
        choiceSelected = null;
        AdvanceDialogue();
    }

    void ParseTags()
    {
        tags = story.currentTags;
        foreach (string t in tags)
        {
            string prefix = t.Split(' ')[0];
            string param = t.Split(' ')[1];

            switch (prefix.ToLower())
            {
                case "anim":
                    SetAnimation(param);
                    break;
                case "color":
                    SetTextColor(param);
                    break;
            }
        }
    }

    void SetTextColor(string _color)
    {
        switch (_color)
        {
            case "red":
                message.color = Color.red;
                break;
            case "blue":
                message.color = Color.cyan;
                break;
            case "green":
                message.color = Color.green;
                break;
            case "white":
                message.color = Color.white;
                break;
            default:
                Debug.Log($"{_color} is not available as a text color");
                break;
        }
    }

    void SetAnimation(string _name)
    {
        CharacterScript cs = GameObject.FindObjectOfType<CharacterScript>();
        cs.PlayAnimation(_name);
    }
}
