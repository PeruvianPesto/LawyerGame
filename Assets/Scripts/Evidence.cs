using UnityEngine;

public class Evidence
{
    // Variables to store evidence properties
    public string name; // Name of the evidence
    public Sprite image; // Image of the evidence
    [TextArea] // Allows multiline text input in the inspector
    public string description; // Description of the evidence

    // Constructor to initialize an Evidence object
    public Evidence(string name, Sprite image, string description)
    {
        this.name = name; // Set the name
        this.image = image; // Set the image
        this.description = description; // Set the description
    }
}