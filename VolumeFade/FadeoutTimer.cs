using System;
using System.Threading;
using CoreAudio;

namespace Org.AvaLanCS.VolumeFadeout
{
    public class FadeoutTimer
    {
        AudioSessionControl2 program;
        SimpleAudioVolume volumeControl;

        bool shouldRun = true;

        float originalVolume;
        float decrease_rate;
        int maxMillis;
        int remaining;
        const int periodMillis = 50;

        public FadeoutTimer(ref AudioSessionControl2 program)
        {
            this.program = program;
            this.program.OnSessionDisconnected += programStopped;
        }

        private void programStopped(object sender, AudioSessionDisconnectReason disconnectReason)
        {
            Console.WriteLine("Program has stopped running, exiting...");
            shouldRun = false;
        }

        public void start(int maxMillis, float decrease_rate)
        {
            if(shouldRun == false)
            {
                throw new Exception("You can only run the FadeoutTimer once! You need to instantiate a new one...");
            }

            volumeControl = this.program.SimpleAudioVolume;
            this.maxMillis = maxMillis;
            this.decrease_rate = decrease_rate;
            remaining = maxMillis;
            originalVolume = getVolume();

            // System.Threading.Timer starts with a noticable delay, so I used Thread.Sleep() instead
            while (shouldRun)
            {
                tick();
                Thread.Sleep(periodMillis);
            }
            Console.WriteLine("Finished.");
        }

        public void tick()
        {
            try
            {
                if (!shouldRun)
                {
                    return;
                }

                if (remaining >= 0)
                {
                    float factor = (float)remaining / maxMillis;
                    if (decrease_rate != 1.0f)
                    {
                        factor = (float)Math.Pow(factor, decrease_rate);
                    }
                    setVolume(originalVolume * factor);

                    remaining -= periodMillis;
                }
                else
                {
                    restoreVolumeAndMute();
                    shouldRun = false;
                }
            } catch (Exception e)
            {
                shouldRun = false;
                throw e;
            }
        }

        protected float getVolume()
        {
            return volumeControl.MasterVolume;
        }

        protected void setVolume(float val)
        {
            volumeControl.MasterVolume = val;
        }

        protected void restoreVolumeAndMute()
        {
            volumeControl.Mute = true;
            volumeControl.MasterVolume = originalVolume;
        }
    }
}
