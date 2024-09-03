using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class DialogueManager : MonoBehaviour
{
    public TextAsset inkJSON; // Holds the Ink file containing the dialogue
    public GameObject textBox; // UI element where dialogue text is displayed
    public GameObject customButton; // Template for creating choice buttons
    public GameObject optionPanel; // Panel containing choice buttons
    public bool isTalking = false; // Flag indicating if dialogue is currently happening
    private Transform lastFocusedCharacter;

    public string expectedEvidenceName;
    public bool isInCrossExamination = false;

    static Story story; // Reference to the Ink story object
    [SerializeField] private TextMeshProUGUI nametag; // TextMeshPro component for the character's name
    [SerializeField] private TextMeshProUGUI message; // TextMeshPro component for the dialogue text
    List<string> tags; // List to store tags parsed from the Ink story
    static Choice choiceSelected; // Currently selected choice by the player
    [SerializeField] private float typingSpeed = 0.05f; // Adjust this to control typing speed
    public CourtRecordManager courtRecordManager; // Reference to CourtRecordManager

    private bool isPaused = false; // Flag to track if the dialogue is paused

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
        // Check if the dialogue is paused; if so, don't continue processing updates
        if (isPaused) return;

        // Listen for mouse click input to continue the dialogue
        if (Input.GetMouseButtonDown(0))
        {
            if (story.canContinue)
            {
                // Set the nametag and advance the dialogue
                //nametag.text = "Lawyer";
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
        if (story.canContinue)
        {
            string currentSentence = story.Continue();
            ParseTags();
            StopAllCoroutines();
            StartCoroutine(TypeSentence(currentSentence));
        }
        else if (story.currentChoices.Count > 0)
        {
            StartCoroutine(ShowChoices());
        }
        else
        {
            FinishDialogue();
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        message.text = ""; // Clear the message box

        // Type out each letter one by one
        foreach (char letter in sentence.ToCharArray())
        {
            message.text += letter;
            yield return new WaitForSeconds(typingSpeed);
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
            string[] splitTag = t.Split(' ');
            string prefix = splitTag[0].ToLower();
            string param = splitTag.Length > 1 ? splitTag[1] : "";

            switch (prefix)
            {
                case "anim":
                    SetAnimation(param); // Set animation based on the tag
                    break;
                case "color":
                    SetTextColor(param); // Set text color based on the tag
                    break;
                case "addingevidence":
                    HandleAddingEvidence(splitTag); // Handle adding evidence based on the tag
                    break;
                case "speaker":
                    SetSpeakerName(param); // Set speaker name based on the tag
                    break;
                case "cross_examination_start":
                    HandleCrossExaminationTrigger();
                    Debug.Log("Cross-examination started!");
                    break;
                case "correct_evidence":
                    HandleCrossExamination(param); // Start cross-examination based on the tag
                    break;
            }
        }
    }

    private void HandleCrossExaminationTrigger()
    {
        isInCrossExamination = true;
        Debug.Log("Cross-examination mode active.");
    }

    private void SetSpeakerName(string name)
    {
        nametag.text = name;

        // Handle the case where the #speaker tag is followed by a blank
        if (string.IsNullOrWhiteSpace(name))
        {
            if (lastFocusedCharacter != null)
            {
                // Focus on the last character the camera was focused on
                CameraFocus cameraFocus = Camera.main.GetComponent<CameraFocus>();
                if (cameraFocus != null)
                {
                    cameraFocus.FocusOnCharacter(lastFocusedCharacter);
                }
            }
            return;
        }

        // Find the character GameObject by name
        GameObject speakerObject = GameObject.Find(name);
        if (speakerObject != null)
        {
            // Update last focused character
            lastFocusedCharacter = speakerObject.transform;

            // Set the camera to focus on the current speaker
            CameraFocus cameraFocus = Camera.main.GetComponent<CameraFocus>();
            if (cameraFocus != null)
            {
                cameraFocus.FocusOnCharacter(speakerObject.transform);
            }
        }
        else
        {
            Debug.LogWarning($"Speaker GameObject with the name '{name}' not found.");
        }
    }

    private void HandleAddingEvidence(string[] splitTag)
    {
        if (splitTag.Length >= 4)
        {
            string evidenceName = splitTag[1];
            string evidenceImagePath = splitTag[2].Trim('\"'); // Remove surrounding quotes
            string evidenceDescription = string.Join(" ", splitTag, 3, splitTag.Length - 3);

            Sprite evidenceSprite = Resources.Load<Sprite>(evidenceImagePath);

            if (evidenceSprite == null)
            {
                Debug.LogWarning($"Could not load evidence sprite at path: {evidenceImagePath}");
                return;
            }

            Evidence newEvidence = new Evidence(evidenceName, evidenceSprite, evidenceDescription);
            courtRecordManager.AddEvidence(newEvidence);
        }
        else
        {
            Debug.LogWarning("Invalid tag format for adding evidence");
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

    void SetAnimation(string animationName)
    {
        // Assuming each character has a CharacterScript attached to their GameObject
        CharacterScript currentSpeaker = GameObject.FindObjectOfType<CharacterScript>();
        currentSpeaker.PlayAnimation(animationName);

        // Ensure the camera focuses on the character only if they are talking
        if (currentSpeaker.isTalking)
        {
            // Set the camera to focus on the current speaker
            CameraFocus cameraFocus = Camera.main.GetComponent<CameraFocus>();
            if (cameraFocus != null)
            {
                cameraFocus.FocusOnCharacter(currentSpeaker.transform);
            }
        }
    }

    // Method to pause the dialogue
    public void PauseDialogue()
    {
        isPaused = true; // Set the pause flag
        textBox.SetActive(false); // Hide the dialogue text box
        optionPanel.SetActive(false); // Hide the option panel
    }

    // Method to resume the dialogue
    public void ResumeDialogue()
    {
        isPaused = false; // Clear the pause flag
        textBox.SetActive(true); // Show the dialogue text box
        if (story.currentChoices.Count > 0) // Check if there are choices to show
        {
            optionPanel.SetActive(true); // Show the option panel
        }
    }

    private void HandleCrossExamination(string evidenceName)
    {
        expectedEvidenceName = evidenceName;
    }

    public void StartCrossExamination()
    {
        isInCrossExamination = true;
        Debug.Log("Cross-examination started!");

        story.ChoosePathString("CrossExaminationStart");
        StartCoroutine(RepeatCrossExaminationDialogue());
    }


    private IEnumerator RepeatCrossExaminationDialogue()
    {
        while (isInCrossExamination)
        {
            if (story.canContinue)
            {
                string currentSentence = story.Continue();
                StopAllCoroutines();
                StartCoroutine(TypeSentence(currentSentence));

                yield return new WaitUntil(() => !isInCrossExamination);
            }
            else
            {
                yield return new WaitForSeconds(2f);
            }

            story.ChoosePathString("CrossExaminationStart");
        }
    }


    public void PresentEvidence(Evidence evidence)
    {
        if (isInCrossExamination)
        {
            Debug.Log($"Presented Evidence: {evidence.name}");
            Debug.Log($"Expected Evidence: {expectedEvidenceName}");

            if (isCorrectEvidence(evidence))
            {
                ContinueCrossExamination();
            }
            else
            {
                HandleIncorrectEvidence();
            }
        }
    }

    private void HandleIncorrectEvidence()
    {
        Debug.Log("Incorrect evidence presented.");
        story.ChoosePathString("IncorrectAnswer");
        AdvanceDialogue();
        StartCoroutine(WaitAndLoopBackToCrossExamination());
    }

    private IEnumerator WaitAndLoopBackToCrossExamination()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Returning to cross-examination start.");
        story.ChoosePathString("CrossExaminationStart");
    }


    private void ContinueCrossExamination()
    {
        Debug.Log("Correct evidence presented.");
        isInCrossExamination = false;
        story.ChoosePathString("CorrectAnswer");
        AdvanceDialogue();
    }

    private bool isCorrectEvidence(Evidence evidence)
    {
        if (evidence == null)
        {
            Debug.LogWarning("No evidence provided for comparison.");
            return false;
        }

        bool isCorrect = evidence.name.Equals(expectedEvidenceName, StringComparison.OrdinalIgnoreCase);

        Debug.Log($"Evidence check: {evidence.name} vs {expectedEvidenceName} - Correct: {isCorrect}");

        return isCorrect;
    }

}
