using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace FiaMedKnuff.Classes
{
    public class ClickEvents
    {
        public void ToggleSoundClicked(List<Player> playerList, Sound sound, bool soundMuted, Image musicIcon)
        {
            foreach (Player player in playerList)
            {
                player.ToggleMoveSound();
            }

            if (!sound.ToggleSoundClicked())
            {
                soundMuted = false;
                musicIcon.Opacity = 1;
            }
            else
            {
                soundMuted = true;
                musicIcon.Opacity = 0.5;
            }
        }
    }
}