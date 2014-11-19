using System;
using System.Windows.Media;

namespace KinectColorApp {
    class SoundController {
        // the SoundPlayer that will play sounds
        private MediaPlayer soundPlayer;

        public SoundController () {
            soundPlayer = new MediaPlayer();
            soundPlayer.Open(new Uri(@"C:\Users\Public\Music\Sample Music\Kalimba.mp3")); // default to Wheels on the Bus
        }

        public void SetSoundLocation (string s) {
            soundPlayer.Open(new Uri(s));
        }

        public void PlaySound() {
            soundPlayer.Play();
        }

        public void StopSound() {
            soundPlayer.Stop();
        }
    }
}

