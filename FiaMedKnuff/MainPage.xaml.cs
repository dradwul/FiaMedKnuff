using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace FiaMedKnuff
{
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Creates a random number generator.
        /// Creates an array of dice images.
        /// </summary>
        private static readonly Random Random = new Random();
        private static readonly string[] DiceImages =
        {
            "ms-appx:///Assets/dice1.png",
            "ms-appx:///Assets/dice2.png",
            "ms-appx:///Assets/dice3.png",
            "ms-appx:///Assets/dice4.png",
            "ms-appx:///Assets/dice5.png",
            "ms-appx:///Assets/dice6.png"
        };

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the Tapped event of the DiceImage control.
        /// Generates a random dice value, animates the correct dice image and creates a random movement animation.
        /// </summary>
        private void DiceImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            int diceValue = Random.Next(1, 7);
            string diceImage = DiceImages[diceValue - 1];

            // Create a random movement animation for X-axis and Y-axis.
            var storyboard = new Storyboard();
            var translateXAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            var translateYAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };

            // Define random movements with X and Y axis.
            for (int i = 0; i <= 10; i++)
            {
                double x = Random.Next(-20, 20); // Random X movement, change value to get bigger movements
                double y = Random.Next(0, 40); // Random Y movement, change value to get bigger movements
                TimeSpan keyTime = TimeSpan.FromMilliseconds(i * 30); //Increase value to get slower movement

                translateXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = keyTime,
                    Value = x
                });
                translateYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = keyTime,
                    Value = y
                });
            }
            
            // Sets the target for the animations
            Storyboard.SetTarget(translateXAnimation, DiceImage);
            Storyboard.SetTargetProperty(translateXAnimation, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
            Storyboard.SetTarget(translateYAnimation, DiceImage);
            Storyboard.SetTargetProperty(translateYAnimation, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");

            storyboard.Children.Add(translateXAnimation);
            storyboard.Children.Add(translateYAnimation);
            // Update the dice image source once the animation is completed
            storyboard.Completed += (s, a) =>
            {
                DiceImage.Source = new BitmapImage(new Uri(diceImage));
            };

            storyboard.Begin();
            Debug.WriteLine(diceValue);
        }

    }
}