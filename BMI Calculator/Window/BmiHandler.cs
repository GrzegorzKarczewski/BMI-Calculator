using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows;
using Newtonsoft.Json;

namespace BMI_Calculator.Window; 

public class BmiHandler {
    private static MainWindow _mainWindow;
    
    public BmiHandler(MainWindow mainWindow) {
        _mainWindow = mainWindow;
    }
    
    public static double CalculateBmi(double weight, double height)
    {
        double bmi = 0;
        bmi = (weight / height) / height * 10000;  // bmi formula 
        
        return Math.Round(bmi, 1);
    }

    /// <summary>
    /// Provides a random tip based on the user's weight type (underweight, healthy, or overweight).
    /// The tips are read from a JSON file named "tips.json", which should contain three categories
    /// of tips: underweight_tips, healthy_weight_tips, and overweight_tips.
    /// If an error occurs while reading the file or the file is not found, a default tip is displayed.
    /// </summary>
    /// <param name="weightType">A WeightType enum value representing the user's weight type.</param>
    public static string GiveTipsForBmi(WeightType weightType)
    {
        string json = String.Empty;
        try
        {
            json = File.ReadAllText("tips.json");
        }
        catch (IOException e)
        {
            //MessageBox.Show(e.ToString());
        }
        finally {
            json ??= "Try to eat Healthy!";
        }

        BMITips tips = JsonConvert.DeserializeObject<BMITips>(json);
        
        string bmiCategory = weightType switch
        {
            WeightType.Low => "underweight_tips",
            WeightType.High => "overweight_tips",
            WeightType.Normal => "healthy_weight_tips",
            _ => String.Empty
        };
        List<string> selectedTipsList = (List<string>)typeof(BMITips).GetProperty(bmiCategory).GetValue(tips);

        Random random = new Random();
        int randomIndex = random.Next(0, selectedTipsList.Count);
        string randomTip = selectedTipsList[randomIndex];
        return randomTip;
    }
    
    public void ChangeLabelBMIScoreStyle(WeightType weightType)
    {

        _mainWindow.lbl_result.FontSize = 34;
        _mainWindow.lbl_result.FontWeight = FontWeights.Bold;
        switch (weightType)
        {
            case WeightType.Low:
                _mainWindow.lbl_result.Foreground = Brushes.LightBlue;
                return;
            case WeightType.Normal:
                _mainWindow.lbl_result.Foreground = Brushes.Green;
                return;
            case WeightType.High:
                _mainWindow.lbl_result.Foreground = Brushes.OrangeRed;
                return;

        }
    }
}