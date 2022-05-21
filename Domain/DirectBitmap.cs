using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Domain.Render;
using ILGPU;

namespace Domain;

public class DirectBitmap : IDisposable
{
    public Bitmap Bitmap { get; private set; }

    private byte[] bits;
    
    public byte[] Bits {
        get
        {
            return bits;
        } 
        set
        {
            bits = value;
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(Width, Height, Width, PixelFormat.Format8bppIndexed, BitsHandle.AddrOfPinnedObject());
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
        Bits = new byte[width * height];
        //BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
        //Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
    }

    public void SetPixel(int x, int y, Color colour)
    {
        int index = x + (y * Width);
        int col = colour.ToArgb();

        Bits[index] = (byte)col;
    }

    public Color GetPixel(int x, int y)
    {
        int index = x + (y * Width);
        int col = Bits[index];
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


