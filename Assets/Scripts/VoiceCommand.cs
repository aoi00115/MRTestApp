using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows.Speech;
using TMPro;
using MixedReality.Toolkit.Examples.Demos;

public class VoiceCommand : MonoBehaviour
{
    public TextMeshProUGUI recognizedSentence;
    public string[] triggerWords;
    public string[] triggerPhrases;

    public string parsedPhraseTransform;
    public string parsedPhraseTargetObject;
    public string parsedPhrasePosition;
    public string parsedPhraseRelativeObject;

    public Transform objectA, objectB, objectC;
    public Transform[] gridPositions;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Do the following only and only if the recognition is finished

        // Checking words
        // Convert the sentence to lowercase
        string procesedSentence = recognizedSentence.text.ToLower();

        // Split the sentence into words
        string[] words = procesedSentence.Split(' ');

        // Loop through each word
        foreach (string word in words)
        {
            // Check if the word is in the trigger words
            foreach (string trigger in triggerWords)
            {
                if (word == trigger)
                {
                    TriggerAction(word);  // Call the function to do something
                }
            }
        }

        // Checking phrase
        // Convert the sentence to lowercase
        string lowerSentence = recognizedSentence.text.ToLower();

        // Check if the sentence contains any of the trigger phrases
        foreach (string phrase in triggerPhrases)
        {
            if (procesedSentence.Contains(phrase.ToLower())) // Case insensitive comparison
            {
                TriggerAction(phrase);  // Call the function to do something
            }
        }

        // Parsed word that is responsible for object's transform : Put, Remove, Rotate
        switch (parsedPhraseTransform)
        {
            case "put":
                // Parsed word that is responsible for object's name to be manipulated : ObjectA, ObjectB, ObjectC
                switch(parsedPhraseTargetObject)
                {
                    case "object a":
                        // Parsed word that is responsible for object's position : in front of, behind, to the right of, to the left of
                        switch(parsedPhrasePosition)
                        {
                            case "in front of objectA":
                                Debug.Log("ObjectA already exists in the grid");
                                break;

                            case "in front of objectB":
                                foreach(Transform gridPosition in gridPositions)
                                {
                                    if(gridPosition.name == CalculateRowInFront(objectB))
                                    {
                                        objectA.SetParent(gridPosition);
                                        objectA.localPosition = Vector3.zero;
                                    }
                                }
                                break;

                            case "in front of objectC":
                                break;
                        }
                        break;
                }
                break;
        }


        // switch(parsedPhrasePosition)
        // {
        //     case "in front of objectA":
        //         foreach(Transform gridPosition in gridPositions)
        //         {
                    
        //         }
        //         break;
        //     case "in front of objectB":
        //         break;
        //     case "in front of objectC":
        //         break;
        // }
    }

    string CalculateRowInFront(Transform ambiguousObject)
    {
        string objectPosition = ambiguousObject.parent.name;      // ex) objectPosition : A2
        string row = objectPosition.Substring(0, 1);              // ex) row : A
        string column = objectPosition.Substring(1, 1);           // ex) column : 2                  Substring extract the character
        string rowInFront = "";

        switch(row)
        {
            case "A":
                rowInFront = "B" + column;
                break;
            case "B":
                rowInFront = "C" + column;
                break;
            case "C":
                Debug.Log("Row limit exceeded");
                break;
        }

        return rowInFront;
    }

    string CalculateRowBehind(Transform ambiguousObject)
    {
        string objectPosition = ambiguousObject.parent.name;      // ex) objectPosition : A2
        string row = objectPosition.Substring(0, 1);              // ex) row : A
        string column = objectPosition.Substring(1, 1);           // ex) column : 2                  Substring extract the character
        string rowBehind = "";

        switch(row)
        {
            case "A":
                Debug.Log("Row limit exceeded");
                break;
            case "B":
                rowBehind = "A" + column;
                break;
            case "C":
                rowBehind = "B" + column;
                break;
        }

        return rowBehind;
    }

    string CalculateColumnToRight(Transform ambiguousObject)
    {
        string objectPosition = ambiguousObject.parent.name;      // ex) objectPosition : A2
        string row = objectPosition.Substring(0, 1);              // ex) row : A
        string column = objectPosition.Substring(1, 1);           // ex) column : 2                  Substring extract the character
        string columnToRight = "";

        switch(column)
        {
            case "1":
                columnToRight = row + "2";
                break;
            case "2":
                columnToRight = row + "3";
                break;
            case "3":
                Debug.Log("Column limit exceeded");
                break;
        }

        return columnToRight;
    }

    string CalculateColumnToLeft(Transform ambiguousObject)
    {
        string objectPosition = ambiguousObject.parent.name;      // ex) objectPosition : A2
        string row = objectPosition.Substring(0, 1);              // ex) row : A
        string column = objectPosition.Substring(1, 1);           // ex) column : 2                  Substring extract the character
        string columnToLeft = "";

        switch(column)
        {
            case "1":
                Debug.Log("Column limit exceeded");
                break;
            case "2":
                columnToLeft = row + "1";
                break;
            case "3":
                columnToLeft = row + "2";
                break;
        }

        return columnToLeft;
    }

    void CalculateNextRow(string nextRow)
    {
        
    }

    void TriggerAction(string word)
    {
        // Implement what happens when a word is recognized
        Debug.Log("Word recognized: " + word);

        // Example: Trigger different actions based on the word

        // Transform
        if (word == "put" || word == "remove" || word == "move" || word == "rotate")
        {
            parsedPhraseTransform = word;
        }

        // Target Object
        if (word == "object a" || word == "object b" || word == "object c")
        {
            if(parsedPhraseTransform != null)
            {
                parsedPhraseTargetObject = word;
            }
        }

        // Position
        if (word == "in front of" || word == "to the right" || word == "to the left" || word == "behind" || word == "a1" || word == "a2" || word == "a3" || word == "b1" || word == "b2" || word == "b3" || word == "c1" || word == "c2" || word == "c3")
        {
            if (parsedPhraseTargetObject != null)
            {
                parsedPhrasePosition = word;
            }
        }

        // Relative Object
        if (word == "object a" || word == "object b" || word == "object c")
        {
            if (parsedPhrasePosition != null)
            {
                parsedPhraseRelativeObject = word;
            }
        }
    }
}
