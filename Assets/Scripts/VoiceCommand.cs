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

    public string[] triggerWords;
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
    public Transform restingPositionA, restingPositionB, restingPositionC;

    public Transform errorMessage;
    public AudioClip errorSound;

    // Start is called before the first frame update
    void Start()
    {
        // Moving objects to their respective resting positions
        objectA.SetParent(restingPositionA);
        objectA.localPosition = Vector3.zero;
        objectB.SetParent(restingPositionB);
        objectB.localPosition = Vector3.zero;
        objectC.SetParent(restingPositionC);
        objectC.localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            isRecognized = !isRecognized;
        }

        if(enableDebugMode)
        {
            // Do the following only and only if the recognition is finished
            if(isRecognized)
            {
                // Parsing words and showing the recognition result
                ShowRecognitionResult();
                ManipulateHologram(parsedPhraseTransform, parsedPhraseTargetObject, parsedPhrasePosition, parsedPhraseRelativeObject);
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
        //     case "place":
        //         // Parsed word that is responsible for object's name to be manipulated : ObjectA, ObjectB, ObjectC
        //         switch(parsedPhraseTargetObject)
        //         {
        //             case "object a":
        //                 // Parsed word that is responsible for object's position : in front of, behind, to the right of, to the left of
        //                 switch(parsedPhrasePosition)
        //                 {
                            
        //                     case "in front of":
        //                         // Parsed word that is responsible for the name of relative objects : ObjectA, ObjectB, ObjectC
        //                         switch(parsedPhraseRelativeObject)
        //                         {
        //                             case "object a":
        //                                 Debug.Log("ObjectA is in use");
        //                                 break;

        //                             case "object b":
        //                                 foreach(Transform gridPosition in gridPositions)
        //                                 {
        //                                     if(gridPosition.name == CalculateRowInFront(parsedPhraseRelativeObject))
        //                                     {
        //                                         objectA.SetParent(gridPosition);
        //                                         objectA.localPosition = Vector3.zero;
        //                                         Debug.Log(parsedPhraseTargetObject + " was put " + parsedPhrasePosition + " " + parsedPhraseRelativeObject + ", which is in " + CalculateRowInFront(parsedPhraseRelativeObject));
        //                                     }
        //                                     else if(CalculateRowInFront(parsedPhraseRelativeObject) == "Row limit exceeded")
        //                                     {
        //                                         Debug.Log("Error putting " + parsedPhraseTargetObject + " " + parsedPhrasePosition + " " + parsedPhraseRelativeObject + " due to : " + CalculateRowInFront(parsedPhraseRelativeObject));
        //                                     }
        //                                 }
        //                                 break;

        //                             case "object c":
        //                                 foreach(Transform gridPosition in gridPositions)
        //                                 {
        //                                     if(gridPosition.name == CalculateRowInFront(parsedPhraseRelativeObject))
        //                                     {
        //                                         objectA.SetParent(gridPosition);
        //                                         objectA.localPosition = Vector3.zero;
        //                                         Debug.Log(parsedPhraseTargetObject + " was put " + parsedPhrasePosition + " " + parsedPhraseRelativeObject + ", which is in " + CalculateRowInFront(parsedPhraseRelativeObject));
        //                                     }
        //                                     else if(CalculateRowInFront(parsedPhraseRelativeObject) == "Row limit exceeded")
        //                                     {
        //                                         Debug.Log("Error putting " + parsedPhraseTargetObject + " " + parsedPhrasePosition + " " + parsedPhraseRelativeObject + " due to : " + CalculateRowInFront(parsedPhraseRelativeObject));
        //                                     }
        //                                 }
        //                                 break;
        //                         }
        //                         break;

        //                     case "behind":
        //                         // Parsed word that is responsible for the name of relative objects : ObjectA, ObjectB, ObjectC
        //                         switch(parsedPhraseRelativeObject)
        //                         {
        //                             case "object a":
        //                                 Debug.Log("ObjectA is in use");
        //                                 break;

        //                             case "object b":
        //                                 foreach(Transform gridPosition in gridPositions)
        //                                 {
        //                                     if(gridPosition.name == CalculateRowBehind(parsedPhraseRelativeObject))
        //                                     {
        //                                         objectA.SetParent(gridPosition);
        //                                         objectA.localPosition = Vector3.zero;
        //                                         Debug.Log(parsedPhraseTargetObject + " was put " + parsedPhrasePosition + " " + parsedPhraseRelativeObject + ", which is in " + CalculateRowBehind(parsedPhraseRelativeObject));
        //                                     }
        //                                     else if(CalculateRowBehind(parsedPhraseRelativeObject) == "Row limit exceeded")
        //                                     {
        //                                         Debug.Log("Error putting " + parsedPhraseTargetObject + " " + parsedPhrasePosition + " " + parsedPhraseRelativeObject + " due to : " + CalculateRowBehind(parsedPhraseRelativeObject));
        //                                     }
        //                                 }
        //                                 break;

        //                             case "object c":
        //                                 foreach(Transform gridPosition in gridPositions)
        //                                 {
        //                                     if(gridPosition.name == CalculateRowBehind(parsedPhraseRelativeObject))
        //                                     {
        //                                         objectA.SetParent(gridPosition);
        //                                         objectA.localPosition = Vector3.zero;
        //                                         Debug.Log(parsedPhraseTargetObject + " was put " + parsedPhrasePosition + " " + parsedPhraseRelativeObject + ", which is in " + CalculateRowBehind(parsedPhraseRelativeObject));
        //                                     }
        //                                     else if(CalculateRowBehind(parsedPhraseRelativeObject) == "Row limit exceeded")
        //                                     {
        //                                         Debug.Log("Error putting " + parsedPhraseTargetObject + " " + parsedPhrasePosition + " " + parsedPhraseRelativeObject + " due to : " + CalculateRowBehind(parsedPhraseRelativeObject));
        //                                     }
        //                                 }
        //                                 break;
        //                         }
        //                         break;

        //                     case "to the right of":
        //                         // Parsed word that is responsible for the name of relative objects : ObjectA, ObjectB, ObjectC
        //                         switch(parsedPhraseRelativeObject)
        //                         {
        //                             case "object a":
        //                                 Debug.Log("ObjectA is in use");
        //                                 break;

        //                             case "object b":
        //                                 foreach(Transform gridPosition in gridPositions)
        //                                 {
        //                                     if(gridPosition.name == CalculateColumnToRight(parsedPhraseRelativeObject))
        //                                     {
        //                                         objectA.SetParent(gridPosition);
        //                                         objectA.localPosition = Vector3.zero;
        //                                         Debug.Log(parsedPhraseTargetObject + " was put " + parsedPhrasePosition + " " + parsedPhraseRelativeObject + ", which is in " + CalculateColumnToRight(parsedPhraseRelativeObject));
        //                                     }
        //                                     else if(CalculateColumnToRight(parsedPhraseRelativeObject) == "Column limit exceeded")
        //                                     {
        //                                         Debug.Log("Error putting " + parsedPhraseTargetObject + " " + parsedPhrasePosition + " " + parsedPhraseRelativeObject + " due to : " + CalculateColumnToRight(parsedPhraseRelativeObject));
        //                                     }
        //                                 }
        //                                 break;

        //                             case "object c":
        //                                 foreach(Transform gridPosition in gridPositions)
        //                                 {
        //                                     if(gridPosition.name == CalculateColumnToRight(parsedPhraseRelativeObject))
        //                                     {
        //                                         objectA.SetParent(gridPosition);
        //                                         objectA.localPosition = Vector3.zero;
        //                                         Debug.Log(parsedPhraseTargetObject + " was put " + parsedPhrasePosition + " " + parsedPhraseRelativeObject + ", which is in " + CalculateColumnToRight(parsedPhraseRelativeObject));
        //                                     }
        //                                     else if(CalculateColumnToRight(parsedPhraseRelativeObject) == "Column limit exceeded")
        //                                     {
        //                                         Debug.Log("Error putting " + parsedPhraseTargetObject + " " + parsedPhrasePosition + " " + parsedPhraseRelativeObject + " due to : " + CalculateColumnToRight(parsedPhraseRelativeObject));
        //                                     }
        //                                 }
        //                                 break;
        //                         }
        //                         break;

        //                     case "to the left of":
        //                         // Parsed word that is responsible for the name of relative objects : ObjectA, ObjectB, ObjectC
        //                         switch(parsedPhraseRelativeObject)
        //                         {
        //                             case "object a":
        //                                 Debug.Log("ObjectA is in use");
        //                                 break;

        //                             case "object b":
        //                                 foreach(Transform gridPosition in gridPositions)
        //                                 {
        //                                     if(gridPosition.name == CalculateColumnToLeft(parsedPhraseRelativeObject))
        //                                     {
        //                                         objectA.SetParent(gridPosition);
        //                                         objectA.localPosition = Vector3.zero;
        //                                         Debug.Log(parsedPhraseTargetObject + " was put " + parsedPhrasePosition + " " + parsedPhraseRelativeObject + ", which is in " + CalculateColumnToLeft(parsedPhraseRelativeObject));
        //                                     }
        //                                     else if(CalculateColumnToLeft(parsedPhraseRelativeObject) == "Column limit exceeded")
        //                                     {
        //                                         Debug.Log("Error putting " + parsedPhraseTargetObject + " " + parsedPhrasePosition + " " + parsedPhraseRelativeObject + " due to : " + CalculateColumnToLeft(parsedPhraseRelativeObject));
        //                                     }
        //                                 }
        //                                 break;

        //                             case "object c":
        //                                 foreach(Transform gridPosition in gridPositions)
        //                                 {
        //                                     if(gridPosition.name == CalculateColumnToLeft(parsedPhraseRelativeObject))
        //                                     {
        //                                         objectA.SetParent(gridPosition);
        //                                         objectA.localPosition = Vector3.zero;
        //                                         Debug.Log(parsedPhraseTargetObject + " was put " + parsedPhrasePosition + " " + parsedPhraseRelativeObject + ", which is in " + CalculateColumnToLeft(parsedPhraseRelativeObject));
        //                                     }
        //                                     else if(CalculateColumnToLeft(parsedPhraseRelativeObject) == "Column limit exceeded")
        //                                     {
        //                                         Debug.Log("Error putting " + parsedPhraseTargetObject + " " + parsedPhrasePosition + " " + parsedPhraseRelativeObject + " due to : " + CalculateColumnToLeft(parsedPhraseRelativeObject));
        //                                     }
        //                                 }
        //                                 break;
        //                         }
        //                         break;
        //                 }
        //                 break;

        //             case "object B":
        //                 break;

        //             case "object C":
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



        // Separate phrases and words because parsing only by phrase mis parsing replace as place and what not
        // Parsing words : put, place, remove, etc...
        // Split the sentence into words
        string[] words = processedSentence.Split(' ');

        // Loop through each word
        foreach (string word in words)
        {
            // Check if the word is in the trigger words
            foreach (string trigger in triggerWords)
            {
                if (word == trigger)
                {
                    TriggerActionForTransformAndPosition(word);  // Call the function to do something
                }
            }
        }



        // Parsing phrases : in front of, to the right of, etc...
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

        ManipulateHologram(parsedPhraseTransform, parsedPhraseTargetObject, parsedPhrasePosition, parsedPhraseRelativeObject);
    }

    public void ResetRecognitionResult()
    {
        debugTexts[0].text = "Transform : ";
        debugTexts[1].text = "Target Object : ";
        debugTexts[2].text = "Position : ";
        debugTexts[3].text = "Relative Object : ";

        parsedPhraseTransform = "";
        parsedPhraseTargetObject = "";
        parsedPhrasePosition = "";
        parsedPhraseRelativeObject = "";
        isTransformRecognized = false;
        isTargetObjectRecognized = false;
        isPositionRecognized = false;
        isRelativeObjectRecognized = false;

        errorMessage.gameObject.SetActive(false);
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
            if (phrase == "put" || phrase == "place" || phrase == "remove" || phrase == "move" || phrase == "replace" || phrase == "swap" || phrase == "rotate")
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

    Transform CalculateRowInFront(string relativeObject)
    {
        Transform tempAmbiguousObject = null;
        if(relativeObject == "object a") tempAmbiguousObject = objectA;
        if(relativeObject == "object b") tempAmbiguousObject = objectB;
        if(relativeObject == "object c") tempAmbiguousObject = objectC;
        string objectPosition = tempAmbiguousObject.parent.name;      // ex) objectPosition : A2
        string row = objectPosition.Substring(0, 1);              // ex) row : A
        string column = objectPosition.Substring(1, 1);           // ex) column : 2                  Substring extract the character
        string rowInFront = "";
        Transform rowObjectInFront = null;

        switch(row)
        {
            case "A":
                rowInFront = "B" + column;
                break;
            case "B":
                rowInFront = "C" + column;
                break;
            case "C":
                rowInFront = "Row limit exceeded";
                break;
        }

        foreach(Transform gridPosition in gridPositions)
        {
            if(gridPosition.name == rowInFront)
            {
                rowObjectInFront = gridPosition;
            }
        }

        return rowObjectInFront;
    }

    Transform CalculateRowBehind(string relativeObject)
    {
        Transform tempAmbiguousObject = null;
        if(relativeObject == "object a") tempAmbiguousObject = objectA;
        if(relativeObject == "object b") tempAmbiguousObject = objectB;
        if(relativeObject == "object c") tempAmbiguousObject = objectC;
        string objectPosition = tempAmbiguousObject.parent.name;      // ex) objectPosition : A2
        string row = objectPosition.Substring(0, 1);              // ex) row : A
        string column = objectPosition.Substring(1, 1);           // ex) column : 2                  Substring extract the character
        string rowBehind = "";
        Transform rowObjectBehind = null;

        switch(row)
        {
            case "A":
                rowBehind = "Row limit exceeded";
                break;
            case "B":
                rowBehind = "A" + column;
                break;
            case "C":
                rowBehind = "B" + column;
                break;
        }

        foreach(Transform gridPosition in gridPositions)
        {
            if(gridPosition.name == rowBehind)
            {
                rowObjectBehind = gridPosition;
            }
        }

        return rowObjectBehind;
    }

    Transform CalculateColumnToRight(string relativeObject)
    {
        Transform tempAmbiguousObject = null;
        if(relativeObject == "object a") tempAmbiguousObject = objectA;
        if(relativeObject == "object b") tempAmbiguousObject = objectB;
        if(relativeObject == "object c") tempAmbiguousObject = objectC;
        string objectPosition = tempAmbiguousObject.parent.name;      // ex) objectPosition : A2
        string row = objectPosition.Substring(0, 1);              // ex) row : A
        string column = objectPosition.Substring(1, 1);           // ex) column : 2                  Substring extract the character
        string columnToRight = "";
        Transform columnObjectToRight = null;

        switch(column)
        {
            case "1":
                columnToRight = row + "2";
                break;
            case "2":
                columnToRight = row + "3";
                break;
            case "3":
                columnToRight =  "Column limit exceeded";
                break;
        }

        foreach(Transform gridPosition in gridPositions)
        {
            if(gridPosition.name == columnToRight)
            {
                columnObjectToRight = gridPosition;
            }
        }

        return columnObjectToRight;
    }

    Transform CalculateColumnToLeft(string relativeObject)
    {
        Transform tempAmbiguousObject = null;
        if(relativeObject == "object a") tempAmbiguousObject = objectA;
        if(relativeObject == "object b") tempAmbiguousObject = objectB;
        if(relativeObject == "object c") tempAmbiguousObject = objectC;
        string objectPosition = tempAmbiguousObject.parent.name;      // ex) objectPosition : A2
        string row = objectPosition.Substring(0, 1);              // ex) row : A
        string column = objectPosition.Substring(1, 1);           // ex) column : 2                  Substring extract the character
        string columnToLeft = "";
        Transform columnObjectToLeft = null;

        switch(column)
        {
            case "1":
                columnToLeft = "Column limit exceeded";
                break;
            case "2":
                columnToLeft = row + "1";
                break;
            case "3":
                columnToLeft = row + "2";
                break;
        }

        foreach(Transform gridPosition in gridPositions)
        {
            if(gridPosition.name == columnToLeft)
            {
                columnObjectToLeft = gridPosition;
            }
        }

        return columnObjectToLeft;
    }

    Transform CalculatePosition(string position, string relativeObject)
    {
        Transform tempPosition = null;

        if(relativeObject == "")          // If there's no relative object
        {
            if(position == "a1") tempPosition = gridPositions[0];
            else if(position == "a2") tempPosition = gridPositions[1];
            else if(position == "a3") tempPosition = gridPositions[2];
            else if(position == "b1") tempPosition = gridPositions[3];
            else if(position == "b2") tempPosition = gridPositions[4];
            else if(position == "b3") tempPosition = gridPositions[5];
            else if(position == "c1") tempPosition = gridPositions[6];
            else if(position == "c2") tempPosition = gridPositions[7];
            else if(position == "c3") tempPosition = gridPositions[8];

        }
        else            // If there's relative object
        {
            if(position == "in front of") 
            {
                if(CalculateRowInFront(relativeObject) != null)
                {
                    tempPosition = CalculateRowInFront(relativeObject);
                }
            }
            else if(position == "behind") 
            {
                if(CalculateRowBehind(relativeObject) != null)
                {
                    tempPosition = CalculateRowBehind(relativeObject);
                }
            }
            else if(position == "to the right of")
            {
                if(CalculateColumnToRight(relativeObject) != null)
                {
                    tempPosition = CalculateColumnToRight(relativeObject);
                }
            }
            else if(position == "to the left of")
            {
                if(CalculateColumnToLeft(relativeObject) != null)
                {
                    tempPosition = CalculateColumnToLeft(relativeObject);
                }
            }
            else if(position == "")         // In case of swapping/replacing where there's no position with relative object
            {
                Transform tempAmbiguousObject = null;
                if(relativeObject == "object a") tempAmbiguousObject = objectA;
                if(relativeObject == "object b") tempAmbiguousObject = objectB;
                if(relativeObject == "object c") tempAmbiguousObject = objectC;
                tempPosition = tempAmbiguousObject.parent;
            }
        }
        
        return tempPosition;
    }

    Transform CalculateResetPosition(string ambiguousTargetObject)
    {
        Transform tempPosition = null;
        if(ambiguousTargetObject == "object a") tempPosition = restingPositionA;
        if(ambiguousTargetObject == "object b") tempPosition = restingPositionB;
        if(ambiguousTargetObject == "object c") tempPosition = restingPositionC;

        return tempPosition;
    }

    void ManipulateHologram(string transform, string targetObject, string position, string relativeObject)
    {
        Transform tempTargetObject = null;
        if(targetObject == "object a") tempTargetObject = objectA;
        else if(targetObject == "object b") tempTargetObject = objectB;
        else if(targetObject == "object c") tempTargetObject = objectC;
        Transform tempRelativeObject = null;
        if(relativeObject == "object a") tempRelativeObject = objectA;
        else if(relativeObject == "object b") tempRelativeObject = objectB;
        else if(relativeObject == "object c") tempRelativeObject = objectC;
        Transform tempPosition = null;
        if(position == "a1") tempPosition = gridPositions[0];
        else if(position == "a2") tempPosition = gridPositions[1];
        else if(position == "a3") tempPosition = gridPositions[2];
        else if(position == "b1") tempPosition = gridPositions[3];
        else if(position == "b2") tempPosition = gridPositions[4];
        else if(position == "b3") tempPosition = gridPositions[5];
        else if(position == "c1") tempPosition = gridPositions[6];
        else if(position == "c2") tempPosition = gridPositions[7];
        else if(position == "c3") tempPosition = gridPositions[8];

        Debug.Log(CalculatePosition(position, relativeObject));
        Debug.Log(relativeObject);

        if(targetObject != "")
        {
            if(targetObject != relativeObject)
            {
                if(CalculatePosition(position, relativeObject) != null)
                {
                    if(transform == "put" || transform == "place")
                    {
                        if(CalculatePosition(position, relativeObject).childCount == 0)
                        {
                            // Put target object in the position only when the position is not taken by another object
                            tempTargetObject.SetParent(CalculatePosition(position, relativeObject));
                            tempTargetObject.localPosition = Vector3.zero;
                            Debug.Log(targetObject + " was successfully put in " + position);
                        }
                        else if(CalculatePosition(position, relativeObject).GetChild(0) == tempTargetObject) 
                        {
                            Debug.Log(tempTargetObject.name + " already exists in " + position);
                            ShowErrorMessage(tempTargetObject.name + " already exists in " + position);
                        }
                        else 
                        {
                            Debug.Log(CalculatePosition(position, relativeObject).GetChild(0).name + " already exists in " + position);
                            ShowErrorMessage(CalculatePosition(position, relativeObject).GetChild(0).name + " already exists in " + position);
                        }
                    }
                    if(transform == "remove")
                    {
                        if(CalculatePosition(position, relativeObject).childCount == 0) 
                        {
                            Debug.Log(position + " does not contain " + targetObject + " to be removed");
                            ShowErrorMessage(position + " does not contain " + targetObject + " to be removed");

                        }
                        else
                        {
                            // Put target object in the position only when the position is not taken by another object
                            tempTargetObject.SetParent(CalculateResetPosition(targetObject));
                            tempTargetObject.localPosition = Vector3.zero;
                            Debug.Log(targetObject + " was successfully removed");
                        }
                    }
                    if(transform == "move")
                    {
                        // if target object exists in grid
                        if(gridPositions.Contains(tempTargetObject.parent))
                        {
                            if(CalculatePosition(position, relativeObject).childCount == 0)
                            {
                                // Put target object in the position only when the position is not taken by another object
                                tempTargetObject.SetParent(CalculatePosition(position, relativeObject));
                                tempTargetObject.localPosition = Vector3.zero;
                                Debug.Log(targetObject + " was successfully moved to " + position);
                            }
                            else if(CalculatePosition(position, relativeObject).GetChild(0) == tempTargetObject) 
                            {
                                Debug.Log(tempTargetObject.name + " already exists in " + position);
                                ShowErrorMessage(tempTargetObject.name + " already exists in " + position);
                            }
                            else if(!gridPositions.Contains(tempRelativeObject.parent)) 
                            {
                                Debug.Log(relativeObject + " does not exist in the grid");
                                ShowErrorMessage(relativeObject + " does not exist in the grid");
                            }
                            else 
                            {
                                Debug.Log(CalculatePosition(position, relativeObject).GetChild(0).name + " already exists in " + position);
                                ShowErrorMessage(CalculatePosition(position, relativeObject).GetChild(0).name + " already exists in " + position);
                            }
                        }
                        else 
                        {
                            Debug.Log(targetObject + " does not exist in the grid");
                            ShowErrorMessage(targetObject + " does not exist in the grid");
                        }
                        
                    }
                    if(transform == "replace" || transform == "swap")
                    {
                        // Store the parent object of tempTargetObject before changing the parent of tempTargetObject
                        Transform tempTargetObjectsParent = tempTargetObject.parent;
                        tempTargetObject.SetParent(CalculatePosition(position, relativeObject));
                        tempTargetObject.localPosition = Vector3.zero;
                        tempRelativeObject.SetParent(tempTargetObjectsParent);
                        tempRelativeObject.localPosition = Vector3.zero;
                        Debug.Log(targetObject + " was successfully replaced/swapped with " + relativeObject);
                    }
                }
                else
                {
                    Debug.Log("Column/Row limit is exceeded or The relative object is in the resting position");
                    ShowErrorMessage("Column/Row limit is exceeded or The relative object is in the resting position");
                }
            }
            else
            {
                Debug.Log("The referenced object in use");
                ShowErrorMessage("The referenced object in use");

            }
        }
    }

    void ShowErrorMessage(string message)
    {
        AudioSource audioSource = this.GetComponent<AudioSource>();
        TextMeshProUGUI errorMessageText = errorMessage.Find("Background/ErrorMessageText").GetComponent<TextMeshProUGUI>();

        errorMessageText.text = message;
        errorMessage.gameObject.SetActive(true);
        audioSource.PlayOneShot(errorSound, 0);
        Invoke("CloseErrorMessage()", 5f);
    }

    void CloseErrorMessage()
    {
        errorMessage.gameObject.SetActive(false);
    }
}
