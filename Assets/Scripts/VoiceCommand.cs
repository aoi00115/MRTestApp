using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UIElements;

public class VoiceCommand : MonoBehaviour
{
    public string parsedPhraseTransform;
    public string parsedPhraseObject;
    public string parsedPhrasePosition;
    public Transform objectA, objectB, objectC;
    public Transform[] gridPositions;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Parsed word that is responsible for object's transform : Put, Remove, Rotate
        switch(parsedPhraseTransform)
        {
            case "put":
                // Parsed word that is responsible for object's name to be manipulated : ObjectA, ObjectB, ObjectC
                switch(parsedPhraseObject)
                {
                    case "objectA":
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
}
