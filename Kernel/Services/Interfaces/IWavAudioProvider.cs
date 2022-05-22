namespace Kernel.Services.Interfaces;

public interface IWavAudioProvider
{
    public (double[] audio, int sampleRate) ReadWav(string filename);
}