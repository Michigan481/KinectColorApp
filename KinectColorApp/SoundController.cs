using System;
using System.Windows.Media;

namespace KinectColorApp {
    class SoundController {
        // the SoundPlayer that will play sounds
        private MediaPlayer player;

        public SoundController (MediaPlayer p) {
            player = p;
            player.Open(new uri(@":c/Windows/Media/WheelsOnBus.mp3")); // default to Wheels on the Bus
        }

        public SetSoundLocation (string s) {
            player.Open(new uri(s));
        }

        public PlaySound() {
            player.Play();
        }

        public StopSound() {
            player.Stop();
        }
    }
}

