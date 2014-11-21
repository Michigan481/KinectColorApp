using System;
using System.Windows.Media;
using System.Uri;

namespace KinectColorApp {
    class SoundController {
        // the SoundPlayer that will play sounds
<<<<<<< HEAD
        private MediaPlayer wheelsOnBusPlayer;
        private MediaPlayer mcDonaldPlayer;
        
        private MediaPlayer blueEffectPlayer;
        private MediaPlayer redEffectPlayer;
        private MediaPlayer greenEffectPlayer;

        public SoundController () {
            wheelsOnBusPlayer = new MediaPlayer();
            wheelsOnBusPlayer.Open(new uri(@":c\Users\Public\Music\Sample Music\Kalimba.mp3"));

            mcDonaldPlayer = new MediaPlayer();
            mcDonaldPlayer.Open(new uri(@"path to old mcdonald mp3"));

            blueEffectPlayer = new MediaPlayer();
            blueEffectPlayer.Open(new uri(@"path to blue effect mp3"));

            redEffectPlayer = new MediaPlayer();
            redEffectPlayer.Open(new uri(@"path to red effect mp3"));

            greenEffectPlayer = new MediaPlayer();
            greenEffectPlayer.Open(new uri(@"path to green effect mp3"));
        }

        public void SetSoundLocation (string s) {
            soundPlayer.Open(new Uri(s));
        }

        public PlayWheelsOnBus() {
            wheelsOnBusPlayer.Play();
        }

        public PlayMcDonald() {
            mcDonaldPlayer.Play();
        }

        public PlayBlueEffect() {
            blueEffectPlayer.Play();
        }

        public PlayRedEffect() {
            redEffectPlayer.Play();
        }

        public PlayGreenEffect() {
            greenEffectPlayer.Play();
        }

        public void StopSound() {
            soundPlayer.Stop();
        }
    }
}

