using System;
using System.Linq;
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
    // Setting the enableDebugMode allows it to simulate the DictationSubsystem_Recognizing and DictationSubsystem_Recognized function in DictationHandlerScript
    public bool enableDebugMode;
    public bool isRecognized = false;

    public TextMeshProUGUI[] debugTexts;
    public TextMeshProUGUI recognizedSentence;
    public string processedSentence;

    public string[] triggerPhrases;
    public string[] triggerObjectPhrases;
    public string[] phrasesToReplace;

    public string parsedPhraseTransform;
    public string parsedPhraseTargetObject;
    public string parsedPhrasePosition;
    public string parsedPhraseRelativeObject;
    public bool isTransformRecognized = false;
    public bool isTargetObjectRecognized = false;
    public bool isPositionRecognized = false;
    public bool isRelativeObjectRecognized = false;

    public Transform objectA, objectB, objectC;
    public Transform[] gridPositions;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(enableDebugMode)
        {
            // Do the following only and only if the recognition is finished
            if(isRecognized)
            {
                // Parsing words and showing the recognition result
                ShowRecognitionResult();
            }
            else
            {
                // Resetting all parameters for the next transcription
                ResetRecognitionResult();
            }
        }

        // // Parsed word that is responsible for object's transform : Put, Remove, Rotate
        // switch (parsedPhraseTransform)
        // {
        //     case "put":
        //         // Parsed word that is responsible for object's name to be manipulated : ObjectA, ObjectB, ObjectC
        //         switch(parsedPhraseTargetObject)
        //         {
        //             case "object a":
        //                 // Parsed word that is responsible for object's position : in front of, behind, to the right of, to the left of
        //                 switch(parsedPhrasePosition)
        //                 {
        //                     case "in front of objectA":
        //                         Debug.Log("ObjectA already exists in the grid");
        //                         break;

        //                     case "in front of objectB":
        //                         foreach(Transform gridPosition in gridPositions)
        //                         {
        //                             if(gridPosition.name == CalculateRowInFront(objectB))
        //                             {
        //                                 objectA.SetParent(gridPosition);
        //                                 objectA.localPosition = Vector3.zero;
        //                             }
        //                         }
        //                         break;

        //                     case "in front of objectC":
        //                         break;
        //                 }
        //                 break;
        //         }
        //         break;
        // }


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

    public void ShowRecognitionResult()
    {
        // Checking words
        // Convert the sentence to lowercase
        processedSentence = recognizedSentence.text.ToLower();

        // Replace txt number into numerical number 
        foreach(string phraseToReplace in phrasesToReplace)
        {
            if(processedSentence.Contains(phraseToReplace.ToLower()))
            {
                ReplaceTextToNumerical(processedSentence, phraseToReplace);
            }
        }




        // Parsing object phrases : 
        // Parse Transform/Position and Objects sepaprately due to the order requirement of objects
        // Dictionary to store the phrase and its position in the sentence
        Dictionary<string, int> objectPhrasePositions = new Dictionary<string, int>();

        // Find the position of each phrase in the sentence
        foreach (string triggerObjectPhrase in triggerObjectPhrases)
        {
            int index = processedSentence.IndexOf(triggerObjectPhrase.ToLower());
            if (index != -1)
            {
                objectPhrasePositions[triggerObjectPhrase] = index;  // Store the phrase and its index
            }
        }

        // Sort the phrases by their position in the sentence (by index)
        foreach (var objectPhrase in objectPhrasePositions.OrderBy(p => p.Value))
        {
            Debug.Log("Phrase detected in order: " + objectPhrase.Key);
            TriggerActionForTargetAndRelativeObjects(objectPhrase.Key);
        }



        // Parsing phrases : put, place, remove, in front of, to the right of etc...
        // Check if the sentence contains any of the trigger phrases
        foreach (string phrase in triggerPhrases)
        {
            if (processedSentence.Contains(phrase.ToLower())) // Case insensitive comparison
            {
                TriggerActionForTransformAndPosition(phrase);  // Call the function to do something
            }
        }

        // Update debug text
        debugTexts[0].text = "Transform : " + parsedPhraseTransform;
        debugTexts[1].text = "Target Object : " + parsedPhraseTargetObject;
        debugTexts[2].text = "Position : " + parsedPhrasePosition;
        debugTexts[3].text = "Relative Object : " + parsedPhraseRelativeObject;
    }

    public void ResetRecognitionResult()
    {
        debugTexts[0].text = "Transform : none";
        debugTexts[1].text = "Target Object : none";
        debugTexts[2].text = "Position : none";
        debugTexts[3].text = "Relative Object : none";

        parsedPhraseTransform = null;
        parsedPhraseTargetObject = null;
        parsedPhrasePosition = null;
        parsedPhraseRelativeObject = null;
        isTransformRecognized = false;
        isTargetObjectRecognized = false;
        isPositionRecognized = false;
        isRelativeObjectRecognized = false;
    }

    void ReplaceTextToNumerical(string sentence, string phrase)
    {
        Debug.Log("Phrase to replace : " + phrase.ToLower());

        string[] words = phrase.Split(' ');
        string textNumber = words[1];
        string numericalNumber = null;
        if(textNumber == "one" || textNumber == "1") numericalNumber = "1";
        if(textNumber == "two" || textNumber == "2") numericalNumber = "2";
        if(textNumber == "three" || textNumber == "3") numericalNumber = "3";
        processedSentence = sentence.Replace(phrase.ToLower(), words[0] + numericalNumber);
        Debug.Log(words[0]);
    }

    void TriggerActionForTargetAndRelativeObjects(string word)
    {
        // Target Object
        if(!isTargetObjectRecognized)
        {
            if (word == "object a" || word == "object b" || word == "object c")
            {
                parsedPhraseTargetObject = word;
                isTargetObjectRecognized = true;
                Debug.Log("Target Object : " + parsedPhraseTargetObject);
            }
        }

        // Relative Object
        if (!isRelativeObjectRecognized && isTargetObjectRecognized && parsedPhraseTargetObject != word)
        {
            if (word == "object a" || word == "object b" || word == "object c")
            {
                // Set parsedPhraseRelativeObject only if the sentence contains parsedPhrasePosition and the parsedPhraseRelativeObject is not parsedPhraseTargetObject itself
                parsedPhraseRelativeObject = word;
                isRelativeObjectRecognized = true;
                Debug.Log("Relative Object : " + parsedPhraseRelativeObject);
            }
        }
    }

    void TriggerActionForTransformAndPosition(string phrase)
    {
        // Implement what happens when a word is recognized
        // Transform
        if(!isTransformRecognized)
        {
            if (phrase == "put" || phrase == "place" || phrase == "remove" || phrase == "move" || phrase == "rotate")
            {
                parsedPhraseTransform = phrase;
                isTransformRecognized = true;
                Debug.Log("Transform : " + parsedPhraseTransform);
            }
        }

        // Position
        if (!isPositionRecognized)
        {
            if (phrase == "in front of" || phrase == "to the right of" || phrase == "to the left of" || phrase == "behind" || phrase == "a1" || phrase == "a2" || phrase == "a3" || phrase == "b1" || phrase == "b2" || phrase == "b3" || phrase == "c1" || phrase == "c2" || phrase == "c3")
            {
                parsedPhrasePosition = phrase;
                isPositionRecognized = true;
                Debug.Log("Position : " + parsedPhrasePosition);
            }
        }
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
}
