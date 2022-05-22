namespace Domain.Services;

public interface IWavAudioProvider
{
    public (double[] audio, int sampleRate) ReadWav();
}