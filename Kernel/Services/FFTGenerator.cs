using Kernel.Services.Interfaces;
using Spectrogram;

namespace Kernel.Services;

public class FFTGenerator
{
    private readonly IWavAudioProvider provider;
    
    public FFTGenerator(IWavAudioProvider provider)
    {
        this.provider = provider;
    }
    
    public List<double[]> GetFFT(string filename)
    {
        var (audio, sampleRate) = provider.ReadWav(filename);
        var sg = new SpectrogramGenerator(sampleRate, fftSize: 4096, stepSize: 2000, maxFreq: 3000);
        sg.Add(audio);
        var fft = sg.GetFFTs();

        return fft;
    }
}