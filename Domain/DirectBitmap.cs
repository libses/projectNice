using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Domain.Render;
using ILGPU;

namespace Domain;

public class DirectBitmap : IDisposable
{
    public Bitmap Bitmap { get; private set; }

    private Int32[] data;
    
    public Int32[] Data {
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

    protected GCHandle BitsHandle { get; private set; }

    public DirectBitmap(int width, int height)
    {
        Width = width;
        Height = height;
        Data = new Int32[width * height];
        //BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
        //Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
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

        Data[index] = (byte)col;
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

    //public static DirectBitmap FromPixelArray(int[] pixels)
    //{
    //    var w = pixels.GetLength(0);
    //    var h = pixels.GetLength(1);
    //    var result = new DirectBitmap(w,h);

    //    for (int x = 0; x < w; x++)
    //    {
    //        for (int y = 0; y < h; y++)
    //        {
    //            result.Bits
    //        }
    //    }
        
    //    Parallel.For(0, w, x =>
    //    {
    //        for (var y = 0; y < h; y++)
    //        {
    //            result.SetPixel(x,y,pixels[x,y]);
    //        }  
    //    });
        
    //    return result;
    //}
}


