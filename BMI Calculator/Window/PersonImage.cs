using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace BMI_Calculator.Window;

public class PersonImage
{
    private static MainWindow _mainWindow;

    public PersonImage(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
    }

    /// <summary>
    /// Updates the displayed image based on the user's weight type and gender.
    /// The function takes the WeightType enum value and an integer representing the gender (0 for male, 1 for female),
    /// and changes the image source accordingly using the images stored in the "Images" folder.
    /// </summary>
    /// <param name="weightType">A WeightType enum value representing the user's weight type.</param>
    /// <param name="gender">An integer representing the user's gender (0 for male, 1 for female).</param>
    public void SetPersonImage(WeightType weightType, int gender)
    {
        switch (weightType)
        {
            case WeightType.Low:
                if (gender == 0)
                {
                    BitmapImage bitmap = new();
                    bitmap.BeginInit();

                    string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "male_skinny.png");
                    bitmap.UriSource = new Uri(imagePath);

                    bitmap.EndInit();
                    _mainWindow.genderImage.Source = bitmap;
                    return;
                }
                else
                {
                    BitmapImage bitmap = new();
                    bitmap.BeginInit();

                    string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "female_skinny.png");
                    bitmap.UriSource = new(imagePath);

                    bitmap.EndInit();
                    _mainWindow.genderImage.Source = bitmap;
                }

                return;
            case WeightType.Normal:
                if (gender == 0)
                {
                    BitmapImage bitmap = new();
                    bitmap.BeginInit();

                    string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "male_regular.png");
                    bitmap.UriSource = new Uri(imagePath);

                    bitmap.EndInit();
                    _mainWindow.genderImage.Source = bitmap;
                    return;
                }
                else
                {
                    BitmapImage bitmap = new();
                    bitmap.BeginInit();

                    string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "female_regular.png");
                    bitmap.UriSource = new(imagePath);

                    bitmap.EndInit();
                    _mainWindow.genderImage.Source = bitmap;
                }
                return;
            case WeightType.High:
                if (gender == 0)
                {
                    BitmapImage bitmap = new();
                    bitmap.BeginInit();

                    string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "male_fat.png");
                    bitmap.UriSource = new(imagePath);

                    bitmap.EndInit();
                    _mainWindow.genderImage.Source = bitmap;
                    return;
                }
                else
                {
                    BitmapImage bitmap = new();
                    bitmap.BeginInit();

                    string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "female_fat.png");
                    bitmap.UriSource = new(imagePath);

                    bitmap.EndInit();
                    _mainWindow.genderImage.Source = bitmap;
                }
                return;
        }
    }
    public void RemovePersonImage()
    {
        _mainWindow.genderImage.Source = null;
    }
}