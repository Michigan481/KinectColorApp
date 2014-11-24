using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;

namespace KinectColorApp {
    class SoundController {

        // File paths for sounds
        const String kalimbaPath = @"../../Resources\Kalimba.mp3";
        const String busPath = @"../../Resources\Kalimba.mp3";
        const String farmPath = @"C:\Users\Public\Music\Sample Music\Kalimba.mp3";
        const String redEffectPath = @"";
        const String greenEffectPath = @"";
        const String blueEffectPath = @"";

        private MediaElement musicPlayer;

        public SoundController () {
            musicPlayer = new MediaElement();
            musicPlayer.LoadedBehavior = MediaState.Manual;
            musicPlayer.UnloadedBehavior = MediaState.Manual;
            musicPlayer.Source =  new Uri(kalimbaPath, UriKind.RelativeOrAbsolute);
            musicPlayer.Volume = 0;
        }

        public void PlayMusic(Backgrounds background) {
            switch (background)
            {
                case Backgrounds.Bus:
                    musicPlayer.Source = new Uri(busPath, UriKind.RelativeOrAbsolute);
                    break;
                case Backgrounds.Farm:
                    musicPlayer.Source = new Uri(farmPath, UriKind.RelativeOrAbsolute);
                    break;
                default:
                    break;
            }
        }

        public void StartMusic()
        {
            musicPlayer.Play();

            DoubleAnimation newAnimation = new DoubleAnimation();
            newAnimation.From = musicPlayer.Volume;
            newAnimation.To = 0.5;
            newAnimation.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(5));
            newAnimation.AutoReverse = false;

            musicPlayer.BeginAnimation(MediaElement.VolumeProperty, newAnimation, HandoffBehavior.SnapshotAndReplace);
        }

        public void StopMusic() {
            Console.WriteLine("here");
            DoubleAnimation newAnimation = new DoubleAnimation();
            newAnimation.From = musicPlayer.Volume;
            newAnimation.To = 0.0;
            newAnimation.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(5));
            newAnimation.AutoReverse = false;
            newAnimation.Completed += (s, e) =>
            {
                musicPlayer.Pause();
            };

            musicPlayer.BeginAnimation(MediaElement.VolumeProperty, newAnimation, HandoffBehavior.SnapshotAndReplace);
        }
    }
}

