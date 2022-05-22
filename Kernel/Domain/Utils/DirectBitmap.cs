using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

#pragma warning disable CS8618

namespace Kernel.Domain.Utils;

public class DirectBitmap : IDisposable
{
    public Bitmap Bitmap { get; private set; }

    private Int32[] data;

    public Int32[] Data
    {
        get => data;
        set
        {
            data = value;
            BitsHandle = GCHandle.Alloc(Data, GCHandleType.Pinned);
            Bitmap = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppRgb, BitsHandle.AddrOfPinnedObject());
        }
    }

    public bool Disposed { get; private set; }
    public int Height { get; private set; }
    public int Width { get; private set; }

    public Size ImageSize => new(Width, Height);

    protected GCHandle BitsHandle { get; private set; }

    public DirectBitmap(int width, int height)
    {
        Width = width;
        Height = height;
        Data = new Int32[width * height];
    }

    public DirectBitmap(int[] array, int width, int height)
    {
        Width = width;
        Height = height;
        Data = array;
    }

    public void SetPixel(int x, int y, Color colour)
    {
        int index = x + (y * Width);
        int col = colour.ToArgb();

        Data[index] = (byte) col;
    }

    public Color GetPixel(int x, int y)
    {
        int index = x + (y * Width);
        int col = Data[index];
        Color result = Color.FromArgb(col);

        return result;
    }

    public void Dispose()
    {
        if (Disposed) return;
        Disposed = true;
        Bitmap.Dispose();
        BitsHandle.Free();
    }
}