using Kernel.Services.Interfaces;
using NAudio.Wave;

namespace Domain.Services;

public class WavAudioMonoProvider : IWavAudioProvider
{
    private readonly string filename;
    private readonly double multiplier;

    public WavAudioMonoProvider(string filename, double multiplier)
    {
        this.filename = filename;
        this.multiplier = multiplier;
    }
    
    public (double[] audio, int sampleRate) ReadWav()
    {
        using var afr = new AudioFileReader(filename);
        var sampleRate = afr.WaveFormat.SampleRate;
        var bytesPerSample = afr.WaveFormat.BitsPerSample / 8;
        var sampleCount = (int)(afr.Length / bytesPerSample);
        var channelCount = afr.WaveFormat.Channels;
        var audio = new List<double>(sampleCount);
        var buffer = new float[sampleRate * channelCount];
        var samplesRead = 0;
        while ((samplesRead = afr.Read(buffer, 0, buffer.Length)) > 0)
            audio.AddRange(buffer.Take(samplesRead).Select(x => x * multiplier));
        return (audio.ToArray(), sampleRate);
    }
}