using System;
using System.IO;
using System.Windows.Media;

namespace KinectColorApp {
    class SoundController {

        // File paths for sounds
        const String kalimbaPath = @"../../Resources\Kalimba.mp3";
        const String busPath = @"../../Resources\Kalimba.mp3";
        const String farmPath = @"C:\Users\Public\Music\Sample Music\Kalimba.mp3";
        const String redEffectPath = @"";
        const String greenEffectPath = @"";
        const String blueEffectPath = @"";

        private MediaPlayer musicPlayer;
        private MediaPlayer effectPlayer;

        public SoundController () {
            musicPlayer = new MediaPlayer();
            musicPlayer.Open(new Uri(kalimbaPath, UriKind.RelativeOrAbsolute));

            effectPlayer = new MediaPlayer();
            //effectPlayer.Open(new Uri(redEffectPath));
        }

        public void PlayEffect(Colors color)
        {
            switch (color)
            {
                case Colors.Red:
                    effectPlayer.Open(new Uri(redEffectPath, UriKind.RelativeOrAbsolute));
                    break;
                case Colors.Green:
                    effectPlayer.Open(new Uri(greenEffectPath, UriKind.RelativeOrAbsolute));
                    break;
                case Colors.Blue:
                    effectPlayer.Open(new Uri(blueEffectPath, UriKind.RelativeOrAbsolute));
                    break;
                default:
                    break;
            }

            effectPlayer.Play();
        }

        public void PlayMusic(Backgrounds background) {
            switch (background)
            {
                case Backgrounds.Bus:
                    musicPlayer.Open(new Uri(busPath, UriKind.RelativeOrAbsolute));
                    break;
                case Backgrounds.Farm:
                    musicPlayer.Open(new Uri(farmPath, UriKind.RelativeOrAbsolute));
                    break;
                default:
                    break;
            }

            musicPlayer.Play();
        }

        public void StopMusic() {
            musicPlayer.Stop();
        }
    }
}

