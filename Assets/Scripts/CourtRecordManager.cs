using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CourtRecordManager : MonoBehaviour
{
    public List<Evidence> evidenceList = new List<Evidence>();
    public GameObject evidencePanel;
    public GameObject evidenceButtonPrefab;
    public Transform evidenceButtonContainer;
    public TextMeshProUGUI evidenceNameText;
    public Image evidenceImage;
    public TextMeshProUGUI evidenceDescriptionText;

    private void Start()
    {
        DisplayEvidenceList();
    }

    private void DisplayEvidenceList()
    {
        foreach(Transform child in evidenceButtonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach(Evidence evidence in evidenceList)
        {
            GameObject button = Instantiate(evidenceButtonPrefab, evidenceButtonContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = evidence.name;
            button.GetComponent<Button>().onClick.AddListener(() => DisplayEvidenceDetails(evidence));
        }
    }

    private void DisplayEvidenceDetails(Evidence evidence)
    {
        evidenceNameText.text = evidence.name;
        evidenceImage.sprite = evidence.image;
        evidenceDescriptionText.text = evidence.description;
    }
}
