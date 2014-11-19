using System;
using System.Windows.Media;

namespace KinectColorApp {
    class SoundController {
        // the SoundPlayer that will play sounds
        public MediaPlayer soundPlayer;

        public SoundController (MediaPlayer p) {
            soundPlayer = p;
            soundPlayer.Open(new uri(@":c/Windows/Media/WheelsOnBus.mp3")); // default to Wheels on the Bus
        }

        public SetSoundLocation (string s) {
            soundPlayer.Open(new uri(s));
        }

        public PlaySound() {
            soundPlayer.Play();
        }

        public StopSound() {
            soundPlayer.Stop();
        }
    }
}

