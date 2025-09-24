using NAudio.Wave;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ICE.Sounds
{
    public static class SoundPlayer
    {
        public static async Task PlaySoundAsync()
        {
            try
            {
                string sound = "ICE.Sounds.Task_Completed.mp3";
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(sound);

                if (stream == null)
                {
                    Console.WriteLine($"Could not find embedded resource: {sound}");
                    return;
                }

                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using var reader = new Mp3FileReader(memoryStream);
                using var waveOut = new WaveOutEvent();

                var tcs = new TaskCompletionSource<bool>();
                waveOut.PlaybackStopped += (sender, args) => tcs.SetResult(true);

                waveOut.Init(reader);
                waveOut.Volume = C.SoundVolume;
                waveOut.Play();

                await tcs.Task; // Wait for playback to complete
            }
            catch (Exception ex)
            {
                ex.Log();
            }
        }
    }
}