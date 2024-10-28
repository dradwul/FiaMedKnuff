using System;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace FiaMedKnuff.Classes
{
    public class Sound
    {
        private MediaPlayer MediaPlayer { get; set; }
        private MediaPlayer DiceSoundPlayer { get; set; }

        public async Task InitializeSound()
        {
            //Initialize MediaPlayer
            MediaPlayer = new MediaPlayer
            {
                Volume = 0.2,
                AudioCategory = MediaPlayerAudioCategory.Media
            };

            await PlayMenuMusic();

            // Initialize Dice Sound Player
            DiceSoundPlayer = new MediaPlayer
            {
                Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/FIA - Dice.mp3")),
                Volume = 0.04
            };
        }

        public async Task PlayMenuMusic()
        {
            MediaPlayer.IsLoopingEnabled = true;
            MediaPlayer.Pause(); // Stop current playback
            MediaPlayer.PlaybackSession.Position = TimeSpan.Zero; // Reset playback position
            MediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/FIA - Menu.mp3"));
            await Task.Delay(100); // Short delay to ensure MediaPlayer has time to load the new source
            MediaPlayer.Play();
        }

        public async Task PlayGameplayMusic()
        {
            MediaPlayer.IsLoopingEnabled = true;
            MediaPlayer.Pause(); // Stop current playback
            MediaPlayer.PlaybackSession.Position = TimeSpan.Zero; // Reset playback position
            MediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/FIA - Play.mp3"));
            await Task.Delay(100); // Short delay to ensure MediaPlayer has time to load the new source
            MediaPlayer.Play();
        }

        public async Task PlayWinMusic()
        {
            MediaPlayer.Pause(); // Stop current playback
            MediaPlayer.PlaybackSession.Position = TimeSpan.Zero; // Reset playback position
            MediaPlayer.IsLoopingEnabled = false; // Disable looping for win music
            MediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/FIA - Win.mp3"));
            await Task.Delay(100); // Short delay to ensure MediaPlayer has time to load the new source
            MediaPlayer.Play();
        }

        public void PlayDiceSound()
        {
            if (DiceSoundPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                DiceSoundPlayer.Pause(); // Stop any ongoing dice sound playback
                DiceSoundPlayer.PlaybackSession.Position = TimeSpan.Zero; // Reset position
            }
            DiceSoundPlayer.Play(); // Play the dice sound
        }

        public void Pause()
        {
            MediaPlayer.Pause();
        }

        public bool ToggleSoundClicked()
        {
            if (MediaPlayer.Volume == 0)
            {
                MediaPlayer.Volume = 0.2;
                DiceSoundPlayer.Volume = 0.04;
                return false;
            }
            else
            {
                MediaPlayer.Volume = 0;
                DiceSoundPlayer.Volume = 0;
                return true;
            }
        }
    }
}
