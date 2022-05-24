using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ILGPU;
using ILGPU.Runtime;

#pragma warning disable CS8618

namespace Kernel.Domain.Utils;

public class DirectBitmap : Combinable<DirectBitmap>, IDisposable
{
    private Bitmap bitmap;

    public Bitmap Bitmap
    {
        get
        {
            if (buffer is not null && !buffer.IsDisposed)
            {
                buffer.CopyToCPU(Data);
                buffer.Dispose();
            }

            return bitmap;
        }
        private set => bitmap = value;
    }

    private Int32[] data;
    private MemoryBuffer1D<int, Stride1D.Dense>? buffer;

    public Int32[] Data
    {
        get => data;
        set
        {
            data = value;
            BitsHandle = GCHandle.Alloc(Data, GCHandleType.Pinned);
            Bitmap = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppArgb, BitsHandle.AddrOfPinnedObject());
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

        Data[index] = col;
    }

    public Color GetPixel(int x, int y)
    {
        int index = x + (y * Width);
        int col = Data[index];
        Color result = Color.FromArgb(col);

        return Bitmap.GetPixel(x, y);
    }

    public void Dispose()
    {
        if (Disposed) return;
        Disposed = true;
        Bitmap.Dispose();
        BitsHandle.Free();
    }

    protected override MemoryBuffer1D<int, Stride1D.Dense> GetBuffer()
    {
        if (buffer is null || buffer.IsDisposed)
        {
            buffer = Gpu.GpuSingleton.Gpu.Allocate1D(Data);
        }

        return buffer;
    }

    public override DirectBitmap GetBitmap()
    {
        return this;
    }

    public override DirectBitmap Update(DirectBitmap bitmap)
    {
        return this;
    }
}