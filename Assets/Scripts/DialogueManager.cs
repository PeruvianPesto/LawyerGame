using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.UI;
using System;

public class DialogueManager : MonoBehaviour
{
    public TextAsset inkJSON; // Holds the Ink file containing the dialogue
    public GameObject textBox; // UI element where dialogue text is displayed
    public GameObject customButton; // Template for creating choice buttons
    public GameObject optionPanel; // Panel containing choice buttons
    public bool isTalking = false; // Flag indicating if dialogue is currently happening

    static Story story; // Reference to the Ink story object
    [SerializeField] private TextMeshProUGUI nametag; // TextMeshPro component for the character's name
    [SerializeField] private TextMeshProUGUI message; // TextMeshPro component for the dialogue text
    List<string> tags; // List to store tags parsed from the Ink story
    static Choice choiceSelected; // Currently selected choice by the player

    void Start()
    {
        // Initialize the story with the Ink file
        story = new Story(inkJSON.text);

        // Initialize the tags list and choiceSelected variable
        tags = new List<string>();
        choiceSelected = null;
    }

    private void Update()
    {
        // Listen for mouse click input to continue the dialogue
        if (Input.GetMouseButtonDown(0))
        {
            if (story.canContinue)
            {
                // Set the nametag and advance the dialogue
                nametag.text = "Lawyer";
                AdvanceDialogue();

                if (story.currentChoices.Count != 0)
                {
                    StartCoroutine(ShowChoices());
                }
            }
            else
            {
                FinishDialogue();
            }
        }
    }

    private void FinishDialogue()
    {
        Debug.Log("End of conversation!");
    }

    void AdvanceDialogue()
    {
        // Continue the story and display the next sentence
        string currentSentence = story.Continue();
        ParseTags(); // Parse any tags in the current dialogue line
        StopAllCoroutines(); // Stop any currently running coroutine
        StartCoroutine(TypeSentence(currentSentence)); // Display the dialogue with typing effect
    }

    IEnumerator TypeSentence(string sentence)
    {
        message.text = ""; // Clear the message box

        // Type out each letter one by one
        foreach (char letter in sentence.ToCharArray())
        {
            message.text += letter;
            yield return null;
        }

        // Get reference to the character currently speaking
        CharacterScript tempSpeaker = GameObject.FindObjectOfType<CharacterScript>();

        // Set the character's animation to idle after they finish talking
        if (tempSpeaker.isTalking)
        {
            SetAnimation("idle");
        }
        yield return null;
    }

    IEnumerator ShowChoices()
    {
        // Display choices for the player to select
        Debug.Log("There are choices need to be made here!");
        List<Choice> _choices = story.currentChoices;

        // Clear existing buttons first to prevent duplication
        foreach (Transform child in optionPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Create buttons for each available choice
        for (int i = 0; i < _choices.Count; i++)
        {
            GameObject temp = Instantiate(customButton, optionPanel.transform);
            temp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _choices[i].text;
            temp.AddComponent<Selectable>(); // Add the Selectable component to handle button actions
            temp.GetComponent<Selectable>().element = _choices[i];
            temp.GetComponent<Button>().onClick.AddListener(() => { temp.GetComponent<Selectable>().Decide(); });
        }

        optionPanel.SetActive(true); // Display the choices panel

        // Wait until a choice is made
        yield return new WaitUntil(() => choiceSelected != null);

        AdvanceFromDecision(); // Continue the story after the choice is made
    }

    public static void SetDecision(object element)
    {
        // Set the choice that the player made
        choiceSelected = (Choice)element;
        story.ChooseChoiceIndex(choiceSelected.index);
    }

    void AdvanceFromDecision()
    {
        // Hide the choices panel and destroy all choice buttons
        optionPanel.SetActive(false);
        for (int i = 0; i < optionPanel.transform.childCount; i++)
        {
            Destroy(optionPanel.transform.GetChild(i).gameObject);
        }
        choiceSelected = null; // Reset the selected choice
        AdvanceDialogue(); // Continue the dialogue
    }

    void ParseTags()
    {
        // Parse and handle tags from the Ink story
        tags = story.currentTags;
        foreach (string t in tags)
        {
            string prefix = t.Split(' ')[0];
            string param = t.Split(' ')[1];

            switch (prefix.ToLower())
            {
                case "anim":
                    SetAnimation(param); // Set animation based on the tag
                    break;
                case "color":
                    SetTextColor(param); // Set text color based on the tag
                    break;
            }
        }
    }

    void SetTextColor(string _color)
    {
        // Change the text color based on the provided parameter
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
        // Trigger animations on the character
        CharacterScript cs = GameObject.FindObjectOfType<CharacterScript>();
        cs.PlayAnimation(_name);
    }
}
