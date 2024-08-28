using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public object element; // Holds the choice element associated with this button

    public void Decide()
    {
        // Call the DialogueManager to set the player's decision
        DialogueManager.SetDecision(element);
    }
}
