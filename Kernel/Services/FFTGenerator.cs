using Kernel.Services.Interfaces;
using Spectrogram;

namespace Domain.Services;

public class FFTGenerator
{
    private readonly IWavAudioProvider provider;
    
    public FFTGenerator(IWavAudioProvider provider)
    {
        this.provider = provider;
    }
    
    public List<double[]> GetFFT()
    {
        var (audio, sampleRate) = provider.ReadWav();
        var sg = new SpectrogramGenerator(sampleRate, fftSize: 4096, stepSize: 2000, maxFreq: 3000);
        sg.Add(audio);
        var fft = sg.GetFFTs();

        return fft;
    }
}