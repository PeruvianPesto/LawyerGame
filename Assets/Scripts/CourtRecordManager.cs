using System;
using System.Collections;
using System.Collections.Generic;
using TMPro; // For using TextMeshPro components
using UnityEngine;
using UnityEngine.UI;

public class CourtRecordManager : MonoBehaviour
{
    // List to store all evidence items
    public List<Evidence> evidenceList = new List<Evidence>();

    // UI elements for displaying the evidence
    public GameObject evidencePanel; // Panel to display the evidence details
    public GameObject evidenceButtonPrefab; // Prefab for evidence buttons
    public Transform evidenceButtonContainer; // Container to hold evidence buttons
    public TextMeshProUGUI evidenceNameText; // Text element for the evidence name
    public Image evidenceImage; // Image element for the evidence sprite
    public TextMeshProUGUI evidenceDescriptionText; // Text element for the evidence description

    private void Start()
    {
        // Display the initial list of evidence when the script starts
        DisplayEvidenceList();
    }

    // Method to add new evidence to the list
    public void AddEvidence(Evidence evidenceToAdd)
    {
        evidenceList.Add(evidenceToAdd); // Add the evidence to the list
        DisplayEvidenceList(); // Refresh the evidence display
    }

    // Method to remove existing evidence from the list
    public void RemoveEvidence(Evidence evidenceToRemove)
    {
        if (evidenceList.Contains(evidenceToRemove)) // Check if the evidence exists in the list
        {
            evidenceList.Remove(evidenceToRemove); // Remove the evidence from the list
            DisplayEvidenceList(); // Refresh the evidence display
        }
    }

    // Method to display the list of evidence as buttons in the UI
    private void DisplayEvidenceList()
    {
        // Clear existing buttons in the evidence container
        foreach (Transform child in evidenceButtonContainer)
        {
            Destroy(child.gameObject); // Destroy each existing button
        }

        // Create a new button for each evidence item in the list
        foreach (Evidence evidence in evidenceList)
        {
            GameObject button = Instantiate(evidenceButtonPrefab, evidenceButtonContainer); // Instantiate the button prefab
            button.GetComponentInChildren<TextMeshProUGUI>().text = evidence.name; // Set the button text to the evidence name
            button.GetComponent<Button>().onClick.AddListener(() => DisplayEvidenceDetails(evidence)); // Add a click listener to show evidence details
        }
    }

    // Method to display the details of the selected evidence
    private void DisplayEvidenceDetails(Evidence evidence)
    {
        evidenceNameText.text = evidence.name; // Set the name text to the evidence's name
        evidenceImage.sprite = evidence.image; // Set the image to the evidence's sprite
        evidenceDescriptionText.text = evidence.description; // Set the description text to the evidence's description
    }
}