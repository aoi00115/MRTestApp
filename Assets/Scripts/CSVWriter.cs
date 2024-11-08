using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVWriter : MonoBehaviour
{
    private string filePath;

    void Start()
    {
        
    }

    // Function to write data row, now taking parameters
    public void WriteData(string subjectName, string experimentMode, string transcribedText, float executionTime)
    {
        // Define the path of the CSV file
        // filePath = Path.Combine(Application.dataPath, subjectName + "_ExperimentData.csv");
        filePath = Path.Combine(Application.persistentDataPath, subjectName + "_ExperimentData.csv");

        // If the file does not exist, create it and write the header row
        if (!File.Exists(filePath))
        {
            string[] header = { "SubjectName", "ExperimentMode", "TranscribedText", "ExecutionTime" };
            File.AppendAllText(filePath, string.Join(",", header) + "\n");
        }

        string[] data = { subjectName, experimentMode, transcribedText, executionTime.ToString() };
        File.AppendAllText(filePath, string.Join(",", data) + "\n");
    }
}
