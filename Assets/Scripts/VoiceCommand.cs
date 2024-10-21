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
using MixedReality.Toolkit;

public class VoiceCommand : MonoBehaviour
{
    // Setting the enableDebugMode allows it to simulate the DictationSubsystem_Recognizing and DictationSubsystem_Recognized function in DictationHandlerScript
    public bool enableDebugMode = false;
    public bool isRecognized = false;
    bool isTimeLogging = false;
    float timer;
    public float executionTime;

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
    string transformWhenRecognizing = "";
    string targetObjectWhenRecognizing = "";
    string positionWhenRecognizing = "";
    string relativeObjectWhenRecognizing = "";


    public Transform objectA, objectB, objectC;
    public Transform[] gridPositions;
    public Transform restingPositionA, restingPositionB, restingPositionC;
    public float verticalMargin = 0.5f;

    public bool error = false;
    public Transform errorMessage;
    public TextMeshProUGUI errorMessageText;
    string errorMessageString;
    public AudioClip errorSound;
    Vector3 hologramPosition;

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
        if(Input.GetKeyDown(KeyCode.X) && enableDebugMode)
        {
            if(isRecognized)
            {
                isRecognized = false;
            
                // Resetting all parameters for the next transcription
                ResetRecognitionResult();
            }
            else
            {
                isRecognized = true;

                // Parsing words and showing the recognition result
                ResetRecognitionResult();
                ShowRecognitionResult("recognizing");
                if(error)
                {
                    ShowErrorMessage();
                }
                isTimeLogging = true;
            }
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            if(isTimeLogging)
            {
                isTimeLogging = false;
            
                // Resetting all parameters for the next transcription
                executionTime = timer;
                timer = 0;
            }
        }

        if(isTimeLogging)
        {
            timer += Time.deltaTime;
        }

        if(objectA.parent != restingPositionA || objectB.parent != restingPositionB || objectB.parent != restingPositionB)
        {
            Transform space = transform.Find("../Space");
            Transform resetHologramButton = transform.Find("../ResetHologramButton");
            space.gameObject.SetActive(true);
            resetHologramButton.gameObject.SetActive(true);
        }
        else if(objectA.parent == restingPositionA || objectB.parent == restingPositionB || objectB.parent == restingPositionB)
        {
            Transform space = transform.Find("../Space");
            Transform resetHologramButton = transform.Find("../ResetHologramButton");
            space.gameObject.SetActive(false);
            resetHologramButton.gameObject.SetActive(false);
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

    public void ShowRecognitionResult(string recognitionState)
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

        // Manipulate hologram again only when there's any change in parsed phrases
        if(recognitionState == "recognizing")
        {
            // Do the following when recognizing 
            ManipulateHologram(parsedPhraseTransform, parsedPhraseTargetObject, parsedPhrasePosition, parsedPhraseRelativeObject);
            transformWhenRecognizing = parsedPhraseTransform;
            targetObjectWhenRecognizing = parsedPhraseTargetObject;
            positionWhenRecognizing = parsedPhrasePosition;
            relativeObjectWhenRecognizing = parsedPhraseRelativeObject;
        }
        else if(recognitionState == "recognized" && (transformWhenRecognizing != parsedPhraseTransform || targetObjectWhenRecognizing != parsedPhraseTargetObject || positionWhenRecognizing != parsedPhrasePosition || relativeObjectWhenRecognizing != parsedPhraseRelativeObject))
        {
            // Do the following when recognized and there's any change in recognized phrases
            ManipulateHologram(parsedPhraseTransform, parsedPhraseTargetObject, parsedPhrasePosition, parsedPhraseRelativeObject);
        }
        
        if(recognitionState == "recognized")
        {
            // Reset the whenRecognizing string regardless after being recognized
            transformWhenRecognizing = "";
            targetObjectWhenRecognizing = "";
            positionWhenRecognizing = "";
            relativeObjectWhenRecognizing = "";

            isTimeLogging = true;
        }
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
        CancelInvoke();
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
            if (phrase == "on top of" || phrase == "under" || phrase == "in front of" || phrase == "to the right of" || phrase == "to the left of" || phrase == "behind" || phrase == "a1" || phrase == "a2" || phrase == "a3" || phrase == "b1" || phrase == "b2" || phrase == "b3" || phrase == "c1" || phrase == "c2" || phrase == "c3")
            {
                parsedPhrasePosition = phrase;
                isPositionRecognized = true;
                Debug.Log("Position : " + parsedPhrasePosition);
            }
        }
    }

    Transform CalculateRowInFront(string relativeObject)
    {
        Transform tempObject = null;
        if(relativeObject == "object a") tempObject = objectA;
        if(relativeObject == "object b") tempObject = objectB;
        if(relativeObject == "object c") tempObject = objectC;
        string objectPosition = tempObject.parent.name;      // ex) objectPosition : A2
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
        Transform tempObject = null;
        if(relativeObject == "object a") tempObject = objectA;
        if(relativeObject == "object b") tempObject = objectB;
        if(relativeObject == "object c") tempObject = objectC;
        string objectPosition = tempObject.parent.name;      // ex) objectPosition : A2
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
        Transform tempObject = null;
        if(relativeObject == "object a") tempObject = objectA;
        if(relativeObject == "object b") tempObject = objectB;
        if(relativeObject == "object c") tempObject = objectC;
        string objectPosition = tempObject.parent.name;      // ex) objectPosition : A2
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
        Transform tempObject = null;
        if(relativeObject == "object a") tempObject = objectA;
        if(relativeObject == "object b") tempObject = objectB;
        if(relativeObject == "object c") tempObject = objectC;
        string objectPosition = tempObject.parent.name;      // ex) objectPosition : A2
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

    // Insert this before SetParent() the targetObject
    void CheckAndMoveObjectAbove(Transform targetObject)
    {
        if(targetObject.parent.childCount == 2 || targetObject.parent.childCount == 3)
        {
            if(targetObject.localPosition.y == 0)
            {
                foreach (Transform child in targetObject.parent.GetComponentsInChildren<Transform>())
                {
                    // Check if the child is neither transformA nor transformB
                    if (child != targetObject.parent && child != targetObject)
                    {
                        child.localPosition -= new Vector3(0, verticalMargin, 0);
                    }
                }
            }
            else if(targetObject.localPosition.y == 0.5f)
            {
                foreach (Transform child in targetObject.parent.GetComponentsInChildren<Transform>())
                {
                    // Check if the child is neither transformA nor transformB
                    if (child != targetObject.parent && child != targetObject && child.localPosition.y == 1)
                    {
                        child.localPosition -= new Vector3(0, verticalMargin, 0);
                    }
                }
            }
        }
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
            else if(position == "on top of" || position == "under")
            {
                Transform tempObject = null;
                if(relativeObject == "object a") tempObject = objectA;
                if(relativeObject == "object b") tempObject = objectB;
                if(relativeObject == "object c") tempObject = objectC;
                if(gridPositions.Contains(tempObject.parent))
                {
                    tempPosition = tempObject.parent;
                }
            }
            else if(position == "")         // In case of swapping/replacing where there's no position with relative object
            {
                Transform tempObject = null;
                if(relativeObject == "object a") tempObject = objectA;
                if(relativeObject == "object b") tempObject = objectB;
                if(relativeObject == "object c") tempObject = objectC;
                tempPosition = tempObject.parent;
            }
        }
        
        return tempPosition;
    }

    Transform CalculateResetPosition(string targetObject)
    {
        Transform tempPosition = null;
        if(targetObject == "object a") tempPosition = restingPositionA;
        if(targetObject == "object b") tempPosition = restingPositionB;
        if(targetObject == "object c") tempPosition = restingPositionC;

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

        // Debug.Log(CalculatePosition(position, relativeObject));
        // Debug.Log(relativeObject);

        errorMessageString = "";
        error = false;

        if(targetObject != "")
        {
            if(targetObject != relativeObject)
            {
                if(CalculatePosition(position, relativeObject) != null)
                {
                    if(transform == "put" || transform == "place" || transform == "move")
                    {
                        if(position == "on top of" || position == "under")
                        {
                            if(position == "on top of")
                            {
                                if(CalculatePosition(position, relativeObject).childCount == 1)
                                {
                                    if(CalculatePosition(position, relativeObject).GetChild(0) == tempTargetObject)
                                    {
                                        Debug.Log("The object cannot be put on top of itself");
                                        errorMessageString = "The object cannot be put on top of itself";
                                    }
                                    else
                                    {
                                        CheckAndMoveObjectAbove(tempTargetObject);
                                        tempTargetObject.SetParent(CalculatePosition(position, relativeObject));
                                        tempTargetObject.localPosition = new Vector3(0, verticalMargin, 0);
                                        Debug.Log(targetObject + " was successfully put on top of the " + relativeObject);
                                    }
                                }
                                else if(CalculatePosition(position, relativeObject).childCount == 2)
                                {
                                    if(CalculatePosition(position, relativeObject).GetChild(0) == tempTargetObject || CalculatePosition(position, relativeObject).GetChild(1) == tempTargetObject)
                                    {
                                        if(tempRelativeObject.localPosition.y == 0)
                                        {
                                            Debug.Log(targetObject + " already on top of " + relativeObject);
                                            errorMessageString = targetObject + " already on top of " + relativeObject;
                                        }
                                        else
                                        {
                                            tempTargetObject.localPosition = new Vector3(0, verticalMargin, 0);
                                            tempRelativeObject.localPosition = Vector3.zero;
                                        }
                                    }
                                    else
                                    {
                                        if(tempRelativeObject.localPosition.y == 0)
                                        {
                                            CheckAndMoveObjectAbove(tempTargetObject);
                                            foreach (Transform child in CalculatePosition(position, relativeObject).GetComponentsInChildren<Transform>())
                                            {
                                                // Check if the child is neither transformA nor transformB
                                                if (child != CalculatePosition(position, relativeObject) && child != tempRelativeObject)
                                                {
                                                    child.localPosition = new Vector3(0, 2*verticalMargin, 0);
                                                    Debug.Log(child);
                                                }
                                            }
                                            // CalculatePosition(position, relativeObject).localPosition = new Vector3(0, 0, 0.66f);
                                            
                                            tempTargetObject.SetParent(CalculatePosition(position, relativeObject));
                                            tempTargetObject.localPosition = new Vector3(0, verticalMargin, 0);
                                        }
                                        else
                                        {
                                            CheckAndMoveObjectAbove(tempTargetObject);
                                            tempTargetObject.SetParent(CalculatePosition(position, relativeObject));
                                            tempTargetObject.localPosition = new Vector3(0, 2*verticalMargin, 0);
                                        }
                                        Debug.Log(targetObject + " was successfully put on top of the " + relativeObject);
                                    }
                                }
                                else if(CalculatePosition(position, relativeObject).childCount == 3)
                                {
                                    if(tempRelativeObject.localPosition.y == tempTargetObject.localPosition.y - 0.5f)
                                    {
                                        Debug.Log(tempTargetObject.name + " already on top of the " + relativeObject);
                                        errorMessageString = tempTargetObject.name + " already on top of the " + relativeObject;
                                    }
                                    else
                                    {
                                        if(tempRelativeObject.localPosition.y == 0)
                                        {
                                            if(tempTargetObject.localPosition.y == 1)
                                            {
                                                tempTargetObject.localPosition = new Vector3(0, verticalMargin, 0);
                                                foreach (Transform child in CalculatePosition(position, relativeObject).GetComponentsInChildren<Transform>())
                                                {
                                                    // Check if the child is neither transformA nor transformB
                                                    if (child != CalculatePosition(position, relativeObject) && child != tempRelativeObject && child != tempTargetObject)
                                                    {
                                                        child.localPosition = new Vector3(0, 2*verticalMargin, 0);
                                                    }
                                                }
                                            }
                                        }
                                        else if(tempRelativeObject.localPosition.y == 0.5f)
                                        {
                                            if(tempTargetObject.localPosition.y == 0)
                                            {
                                                tempTargetObject.localPosition = new Vector3(0, 2*verticalMargin, 0);

                                                foreach (Transform child in CalculatePosition(position, relativeObject).GetComponentsInChildren<Transform>())
                                                {
                                                    // Check if the child is neither transformA nor transformB
                                                    if (child != CalculatePosition(position, relativeObject) && child != tempRelativeObject && child != tempTargetObject)
                                                    {
                                                        child.localPosition = Vector3.zero;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if(tempTargetObject.localPosition.y == 0)
                                            {
                                                tempTargetObject.localPosition = new Vector3(0, 2*verticalMargin, 0);
                                                tempRelativeObject.localPosition = new Vector3(0, verticalMargin, 0);


                                                foreach (Transform child in CalculatePosition(position, relativeObject).GetComponentsInChildren<Transform>())
                                                {
                                                    // Check if the child is neither transformA nor transformB
                                                    if (child != CalculatePosition(position, relativeObject) && child != tempRelativeObject && child != tempTargetObject)
                                                    {
                                                        child.localPosition = Vector3.zero;
                                                    }
                                                }
                                            }
                                            if(tempTargetObject.localPosition.y == 0.5f)
                                            {
                                                tempTargetObject.localPosition = new Vector3(0, 2*verticalMargin, 0);
                                                tempRelativeObject.localPosition = new Vector3(0, verticalMargin, 0);

                                            }
                                        }
                                        Debug.Log(targetObject + " was successfully put on top of the " + relativeObject);
                                    }
                                }
                            }
                            else if(position == "under")
                            {
                                if(CalculatePosition(position, relativeObject).childCount == 1)
                                {
                                    if(CalculatePosition(position, relativeObject).GetChild(0) == tempTargetObject)
                                    {
                                        Debug.Log(tempTargetObject.name + " already exists in " + position);
                                        errorMessageString = tempTargetObject.name + " already exists in " + position;
                                    }
                                    else
                                    {
                                        CheckAndMoveObjectAbove(tempTargetObject);
                                        tempTargetObject.SetParent(CalculatePosition(position, relativeObject));
                                        tempTargetObject.localPosition = Vector3.zero;
                                        tempRelativeObject.localPosition = new Vector3(0, verticalMargin, 0);
                                        Debug.Log(targetObject + " was successfully put under the " + relativeObject);
                                    }
                                }
                                else if(CalculatePosition(position, relativeObject).childCount == 2)
                                {
                                    if(CalculatePosition(position, relativeObject).GetChild(0) == tempTargetObject || CalculatePosition(position, relativeObject).GetChild(1) == tempTargetObject)
                                    {
                                        if(tempRelativeObject.localPosition.y == 0.5f)
                                        {
                                            Debug.Log(targetObject + " already under the " + relativeObject);
                                            errorMessageString = targetObject + " already under the " + relativeObject;
                                        }
                                        else
                                        {
                                            tempTargetObject.localPosition = Vector3.zero;
                                            tempRelativeObject.localPosition = new Vector3(0, verticalMargin, 0);
                                        }
                                    }
                                    else
                                    {
                                        if(tempRelativeObject.localPosition.y == 0)
                                        {
                                            CheckAndMoveObjectAbove(tempTargetObject);
                                            CalculatePosition(position, relativeObject).GetChild(0).localPosition += new Vector3(0, verticalMargin, 0);
                                            CalculatePosition(position, relativeObject).GetChild(1).localPosition += new Vector3(0, verticalMargin, 0);
                                            tempTargetObject.SetParent(CalculatePosition(position, relativeObject));
                                            tempTargetObject.localPosition = Vector3.zero;
                                        }
                                        else
                                        {
                                            CheckAndMoveObjectAbove(tempTargetObject);
                                            tempTargetObject.SetParent(CalculatePosition(position, relativeObject));
                                            tempTargetObject.localPosition = new Vector3(0, verticalMargin, 0);
                                            tempRelativeObject.localPosition = new Vector3(0, 2*verticalMargin, 0);
                                        }
                                        Debug.Log(targetObject + " was successfully put under the " + relativeObject);
                                    }
                                }
                                else if(CalculatePosition(position, relativeObject).childCount == 3)
                                {
                                    if(tempRelativeObject.localPosition.y == tempTargetObject.localPosition.y + 0.5f)
                                    {
                                        Debug.Log(tempTargetObject.name + " already under the " + relativeObject);
                                        errorMessageString = tempTargetObject.name + " already under the " + relativeObject;
                                    }
                                    else
                                    {
                                        if(tempRelativeObject.localPosition.y == 0)
                                        {
                                            if(tempTargetObject.localPosition.y == 0.5f)
                                            {
                                                tempTargetObject.localPosition = Vector3.zero;
                                                tempRelativeObject.localPosition = new Vector3(0, verticalMargin, 0);
                                            }
                                            else if(tempTargetObject.localPosition.y == 1)
                                            {
                                                tempTargetObject.localPosition = Vector3.zero;
                                                tempRelativeObject.localPosition = new Vector3(0, verticalMargin, 0);

                                                foreach (Transform child in CalculatePosition(position, relativeObject).GetComponentsInChildren<Transform>())
                                                {
                                                    // Check if the child is neither transformA nor transformB
                                                    if (child != CalculatePosition(position, relativeObject) && child != tempRelativeObject && child != tempTargetObject)
                                                    {
                                                        child.localPosition = new Vector3(0, 2*verticalMargin, 0);
                                                    }
                                                }
                                            }
                                        }
                                        else if(tempRelativeObject.localPosition.y == 0.5f)
                                        {
                                            if(tempTargetObject.localPosition.y == 1)
                                            {
                                                tempTargetObject.localPosition = new Vector3(0, verticalMargin, 0);
                                                tempRelativeObject.localPosition = new Vector3(0, 2*verticalMargin, 0);
                                            }
                                        }
                                        else
                                        {
                                            if(tempTargetObject.localPosition.y == 0f)
                                            {
                                                tempTargetObject.localPosition = new Vector3(0, verticalMargin, 0);
                                                
                                                foreach (Transform child in CalculatePosition(position, relativeObject).GetComponentsInChildren<Transform>())
                                                {
                                                    // Check if the child is neither transformA nor transformB
                                                    if (child != CalculatePosition(position, relativeObject) && child != tempRelativeObject && child != tempTargetObject)
                                                    {
                                                        child.localPosition = Vector3.zero;
                                                    }
                                                }
                                            }
                                        }
                                        Debug.Log(targetObject + " was successfully put under the " + relativeObject);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if(CalculatePosition(position, relativeObject).childCount == 0)
                            {
                                CheckAndMoveObjectAbove(tempTargetObject);
                                // Put target object in the position only when the position is not taken by another object
                                tempTargetObject.SetParent(CalculatePosition(position, relativeObject));
                                tempTargetObject.localPosition = Vector3.zero;
                                Debug.Log(targetObject + " was successfully put in " + position);
                            }
                            else if(CalculatePosition(position, relativeObject).GetChild(0) == tempTargetObject) 
                            {
                                Debug.Log(tempTargetObject.name + " already exists in " + position);
                                errorMessageString = tempTargetObject.name + " already exists in " + position;
                            }
                            else 
                            {
                                Debug.Log(CalculatePosition(position, relativeObject).GetChild(0).name + " already exists in " + position);
                                errorMessageString = CalculatePosition(position, relativeObject).GetChild(0).name + " already exists in " + position;
                            }
                        }
                    }
                    if(transform == "remove")
                    {
                        if(!CalculatePosition(position, relativeObject).GetComponentsInChildren<Transform>().Contains(tempTargetObject)) 
                        {
                            Debug.Log(position + " does not contain " + targetObject + " to be removed");
                            errorMessageString = position + " does not contain " + targetObject + " to be removed";

                        }
                        else
                        {
                            CheckAndMoveObjectAbove(tempTargetObject);
                            // Put target object in the position only when the position is not taken by another object
                            tempTargetObject.SetParent(CalculateResetPosition(targetObject));
                            tempTargetObject.localPosition = Vector3.zero;
                            Debug.Log(targetObject + " was successfully removed");
                        }
                    }
                    // if(transform == "move")
                    // {
                    //     // if target object exists in grid
                    //     if(gridPositions.Contains(tempTargetObject.parent))
                    //     {
                    //         if(CalculatePosition(position, relativeObject).childCount == 0)
                    //         {
                    //             CheckAndMoveObjectAbove(tempTargetObject);
                    //             // Put target object in the position only when the position is not taken by another object
                    //             tempTargetObject.SetParent(CalculatePosition(position, relativeObject));
                    //             tempTargetObject.localPosition = Vector3.zero;
                    //             Debug.Log(targetObject + " was successfully moved to " + position);
                    //         }
                    //         else if(CalculatePosition(position, relativeObject).GetChild(0) == tempTargetObject) 
                    //         {
                    //             Debug.Log(targetObject + " already exists in " + position);
                    //             errorMessageString = targetObject + " already exists in " + position;
                    //         }
                    //         else if(!gridPositions.Contains(tempRelativeObject.parent)) 
                    //         {
                    //             Debug.Log(relativeObject + " does not exist in the grid");
                    //             errorMessageString = relativeObject + " does not exist in the grid";
                    //         }
                    //         else 
                    //         {
                    //             Debug.Log(CalculatePosition(position, relativeObject).GetChild(0).name + " already exists in " + position);
                    //             errorMessageString = CalculatePosition(position, relativeObject).GetChild(0).name + " already exists in " + position;
                    //         }
                    //     }
                    //     else 
                    //     {
                    //         Debug.Log(targetObject + " does not exist in the grid");
                    //         errorMessageString = targetObject + " does not exist in the grid";
                    //     }
                    // }
                    if(transform == "replace" || transform == "swap")
                    {
                        // Store the parent object of tempTargetObject before changing the parent of tempTargetObject
                        Transform tempTargetObjectsParent = tempTargetObject.parent;
                        // The following two lines are required in order to store the vertical information of swapping objects
                        float tempTargetObjectY = tempTargetObject.localPosition.y;
                        float tempRelativeObjectY = tempRelativeObject.localPosition.y;
                        tempTargetObject.SetParent(CalculatePosition(position, relativeObject));
                        tempTargetObject.localPosition = new Vector3(0, tempRelativeObjectY, 0);
                        tempRelativeObject.SetParent(tempTargetObjectsParent);
                        tempRelativeObject.localPosition = new Vector3(0, tempTargetObjectY, 0);
                        Debug.Log(targetObject + " was successfully replaced/swapped with " + relativeObject);
                    }
                }
                else
                {
                    Debug.Log("Column/Row limit is exceeded or The relative object is in the resting position");
                    errorMessageString = "Column/Row limit is exceeded or The relative object is in the resting position";
                }
            }
            else
            {
                Debug.Log("The referenced object in use");
                errorMessageString = "The referenced object in use";
            }
        }

        if(errorMessageString != "")
        {
            error = true;
        }
    }

    public void ResetHologram()
    {
        // Moving objects to their respective resting positions
        objectA.SetParent(restingPositionA);
        objectA.localPosition = Vector3.zero;
        objectB.SetParent(restingPositionB);
        objectB.localPosition = Vector3.zero;
        objectC.SetParent(restingPositionC);
        objectC.localPosition = Vector3.zero;

        ResetRecognitionResult();
    }

    public void ShowErrorMessage()
    {
        AudioSource audioSource = this.GetComponent<AudioSource>();
        errorMessage.gameObject.SetActive(true);
        errorMessageText.text = errorMessageString;
        audioSource.PlayOneShot(errorSound);
        Invoke("CloseErrorMessage", 10f);
    }

    void CloseErrorMessage()
    {
        errorMessage.gameObject.SetActive(false);
    }

    public void EnableTextMode()
    {
        GameObject grid = GameObject.Find("Grid");
        hologramPosition = grid.transform.position;

        grid.transform.position = new Vector3(1000, 1000, 1000);
    }

    public void EnableHologram()
    {
        GameObject grid = GameObject.Find("Grid");
        grid.transform.position = hologramPosition;
    }
}
