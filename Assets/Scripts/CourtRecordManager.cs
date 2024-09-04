using System;
using System.Collections.Generic;
using TMPro; // For using TextMeshPro components
using UnityEngine;
using UnityEngine.UI;

public class CourtRecordManager : MonoBehaviour
{
    // List to store all evidence items
    public List<Evidence> evidenceList = new List<Evidence>();

    // UI elements for displaying the evidence
    public GameObject courtRecordPanel; // Panel to display the Court Record
    public TextMeshProUGUI evidenceNameText; // Text element for the evidence name
    public Image evidenceImage; // Image element for the evidence sprite
    public TextMeshProUGUI evidenceDescriptionText; // Text element for the evidence description
    public Button nextButton; // Button to navigate to the next evidence
    public Button previousButton; // Button to navigate to the previous evidence
    public Button presentButton; // Button to present the evidence

    private int currentEvidenceIndex = 0; // Tracks the currently displayed evidence index
    private bool isCourtRecordOpen = false; // Tracks if the Court Record is open
    public DialogueManager dialogueManager; // Reference to the DialogueManager

    private void Start()
    {
        courtRecordPanel.SetActive(false); // Initially hide the Court Record panel

        // Add listeners to the navigation buttons
        nextButton.onClick.AddListener(ShowNextEvidence);
        previousButton.onClick.AddListener(ShowPreviousEvidence);
        presentButton.onClick.AddListener(PresentEvidence);
    }

    private void Update()
    {
        // Toggle the Court Record panel with the "E" key
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isCourtRecordOpen)
            {
                ToggleCourtRecord();
                dialogueManager.PauseDialogue(); // Pause the dialogue when opening the Court Record
            }
            else
            {
                ToggleCourtRecord();
                dialogueManager.ResumeDialogue(); // Resume the dialogue when closing the Court Record
            }

        }
    }

    // Toggle the Court Record display
    private void ToggleCourtRecord()
    {
        isCourtRecordOpen = !isCourtRecordOpen; // Toggle the state
        courtRecordPanel.SetActive(isCourtRecordOpen); // Show or hide the Court Record panel

        if (isCourtRecordOpen)
        {
            // Display the first evidence when opening the Court Record
            if (evidenceList.Count > 0)
            {
                currentEvidenceIndex = 0; // Start from the first evidence
                DisplayEvidenceDetails(evidenceList[currentEvidenceIndex]);
            }
        }
    }

    // Show the next evidence in the list
    private void ShowNextEvidence()
    {
        if (evidenceList.Count == 0) return; // Exit if no evidence

        currentEvidenceIndex = (currentEvidenceIndex + 1) % evidenceList.Count; // Loop to the first evidence if at the end
        DisplayEvidenceDetails(evidenceList[currentEvidenceIndex]);
    }

    // Show the previous evidence in the list
    private void ShowPreviousEvidence()
    {
        if (evidenceList.Count == 0) return; // Exit if no evidence

        currentEvidenceIndex = (currentEvidenceIndex - 1 + evidenceList.Count) % evidenceList.Count; // Loop to the last evidence if at the start
        DisplayEvidenceDetails(evidenceList[currentEvidenceIndex]);
    }

    // Method to display the details of the selected evidence
    private void DisplayEvidenceDetails(Evidence evidence)
    {
        evidenceNameText.text = evidence.name; // Set the name text to the evidence's name
        evidenceImage.sprite = evidence.image; // Set the image to the evidence's sprite
        evidenceDescriptionText.text = evidence.description; // Set the description text to the evidence's description
    }

    // Method to add new evidence to the list
    public void AddEvidence(Evidence evidenceToAdd)
    {
        evidenceList.Add(evidenceToAdd); // Add the evidence to the list
    }

    // Method to remove existing evidence from the list
    public void RemoveEvidence(Evidence evidenceToRemove)
    {
        if (evidenceList.Contains(evidenceToRemove)) // Check if the evidence exists in the list
        {
            evidenceList.Remove(evidenceToRemove); // Remove the evidence from the list
        }
    }

    public void PresentEvidence()
    {
        if (isCourtRecordOpen && dialogueManager.isInCrossExamination)
        {
            Evidence selectedEvidence = GetCurrentEvidence();

            if (selectedEvidence != null)
            {
                dialogueManager.PresentEvidence(selectedEvidence);
                Debug.Log("Presenting evidence: " + selectedEvidence.name);

                // Close the Court Record after presenting the evidence
                ToggleCourtRecord(); // This will set isCourtRecordOpen to false and hide the panel
                dialogueManager.ResumeDialogue(); // Optionally, resume dialogue if needed
            }
        }
    }

    public Evidence GetCurrentEvidence()
    {
        if (evidenceList.Count > 0)
        {
            return evidenceList[currentEvidenceIndex];
        }
        return null;
    }
}