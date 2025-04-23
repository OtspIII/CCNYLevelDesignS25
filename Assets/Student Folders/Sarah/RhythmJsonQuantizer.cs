using System;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

public class RhythmQuantizer : EditorWindow
{
    private const double BPM = 127.0;
    private static readonly double BeatInterval = 60.0 / BPM;
    private static readonly string inputPath = "Assets/Student Folders/Sarah/Sarah.json";
    private static readonly string outputPath = "Assets/Student Folders/Sarah/Sarah_quantized.json";

    [MenuItem("Tools/Quantize Rhythm JSON")]
    public static void QuantizeRhythmJSON()
    {
        if (!File.Exists(inputPath))
        {
            Debug.LogError("Input JSON not found at: " + inputPath);
            return;
        }

        string jsonText = File.ReadAllText(inputPath);
        JObject root = JObject.Parse(jsonText);

        JArray events = (JArray)root["Events"];
        foreach (JObject evt in events)
        {
            if (evt.ContainsKey("Time"))
            {
                double originalTime = evt["Time"].Value<double>();
                double quantizedTime = Mathf.Round((float)(originalTime / BeatInterval)) * BeatInterval;
                evt["Time"] = Math.Round(quantizedTime, 3);
            }
        }

        File.WriteAllText(outputPath, root.ToString());
        Debug.Log("Rhythm quantized! Saved to: " + outputPath);
        AssetDatabase.Refresh();
    }
}