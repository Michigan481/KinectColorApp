using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;

namespace KinectColorApp {
    class SoundController {

        // File paths for sounds
        const String kalimbaPath = @"../../Resources\Kalimba.mp3";
        const String redEffectPath = @"../../Resources\red_SFX.mp3";
        const String greenEffectPath = @"../../Resources\green_SFX.mp3";
        const String blueEffectPath = @"../../Resources\blue_SFX.mp3";
        const String eraserEffectPath = @"../../Resources\eraser_SFX.mp3";
        const String backgroundEffectPath = @"../../Resources\change_background_SFX.mp3";

        const String loadedEffectPath = kalimbaPath;

        private MediaElement musicPlayer;
        private SoundPlayer effectPlayer;

        public SoundController () {
            musicPlayer = new MediaElement();
            musicPlayer.LoadedBehavior = MediaState.Manual;
            musicPlayer.UnloadedBehavior = MediaState.Manual;
            effectPlayer = new SoundPlayer();
            musicPlayer.Source =  new Uri(kalimbaPath, UriKind.RelativeOrAbsolute);
            musicPlayer.Volume = 0;
        }

        public void StartMusic()
        {
            if (musicPlayer.CurrentState == MediaElementState.Stopped) {
                // loop only when it is music and the playback has reached its end
                musicPlayer.position = TimeSpan.zero;
            }

            musicPlayer.Play();

            DoubleAnimation newAnimation = new DoubleAnimation();
            newAnimation.From = musicPlayer.Volume;
            newAnimation.To = 0.5;
            newAnimation.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2));
            newAnimation.AutoReverse = false;

            musicPlayer.BeginAnimation(MediaElement.VolumeProperty, newAnimation, HandoffBehavior.SnapshotAndReplace);
        }

        public void StopMusic() {
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

        public void TriggerBackgroundEffect() {
            effectPlayer.stop();
            effectPlayer.SoundLocation = backgroundEffectPath;
            effectPlayer.play();
        }

        public void TriggerColorEffect(int c) {
            effectPlayer.stop();
            switch (c) {
                case 0:
                    effectPlayer.SoundLocation = redEffectPath;
                    effectPlayer.play();
                    break;
                case 1:
                    effectPlayer.SoundLocation = greenEffectPath;
                    effectPlayer.play();
                    break;
                case 2:
                    effectPlayer.SoundLocation = blueEffectPath;
                    effectPlayer.play();
                    break;
                case 3:
                    effectPlayer.SoundLocation = eraserEffectPath;
                    effectPlayer.play();
                    break;
                default:
                    effectPlayer.stop();
            }
    }
}

