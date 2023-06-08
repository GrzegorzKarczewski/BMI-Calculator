using System;
using System.Windows.Media.Imaging;

using System.Windows.Controls;

namespace BMI_Calculator; 

public class PersonImage {
    // Move SetPersonImage() method from MainWindow.xaml.cs to this class
    public static void SetPersonImage(WeightType weightType, int gender) {
        Image image = new Image();
        
        if (gender == 0) {
            string imagePath = weightType switch
            {
                WeightType.Low => "Images/male_skinny.png",
                WeightType.High => "Images/male_fat.png",
                WeightType.Normal => "Images/male_regular.png",
                _ => String.Empty
            };
            image.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));   
        }
        else {
            string imagePath = weightType switch
            {
                WeightType.Low => "Images/male_skinny.png",
                WeightType.High => "Images/male_fat.png",
                WeightType.Normal => "Images/male_regular.png",
                _ => String.Empty
            };
            image.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));   
        }
    }
}