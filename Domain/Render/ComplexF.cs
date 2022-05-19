using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Render;

public readonly struct ComplexF : IEquatable<ComplexF>, IFormattable
{
    public static readonly ComplexF Zero = new ComplexF(0.0f, 0.0f);
    public static readonly ComplexF One = new ComplexF(1.0f, 0.0f);
    public static readonly ComplexF ImaginaryOne = new ComplexF(0.0f, 1.0f);
    public static readonly ComplexF NaN = new ComplexF(float.NaN, float.NaN);
    public static readonly ComplexF Infinity = new ComplexF(float.PositiveInfinity, float.PositiveInfinity);


    // This is the largest x for which (Hypot(x,x) + x) will not overflow. It is used for branching inside Sqrt.

    // This is the largest x for which 2 x^2 will not overflow. It is used for branching inside Asin and Acos.

    // This value is used inside Asin and Acos.

    // Do not rename, these fields are needed for binary serialization
    private readonly float m_real; // Do not rename (binary serialization)
    private readonly float m_imaginary; // Do not rename (binary serialization)

    public ComplexF(float real, float imaginary)
    {
        m_real = real;
        m_imaginary = imaginary;
    }

    public float Real => m_real;

    public float Imaginary => m_imaginary;

    public float Magnitude => Abs(this);

    public float Phase => MathF.Atan2(m_imaginary, m_real);


    public static ComplexF operator -(ComplexF value) /* Unary negation of a complex number */
    {
        return new ComplexF(-value.m_real, -value.m_imaginary);
    }

    public static ComplexF operator +(ComplexF left, ComplexF right)
    {
        return new ComplexF(left.m_real + right.m_real, left.m_imaginary + right.m_imaginary);
    }

    public static ComplexF operator +(ComplexF left, float right)
    {
        return new ComplexF(left.m_real + right, left.m_imaginary);
    }

    public static ComplexF operator +(float left, ComplexF right)
    {
        return new ComplexF(left + right.m_real, right.m_imaginary);
    }

    public static ComplexF operator -(ComplexF left, ComplexF right)
    {
        return new ComplexF(left.m_real - right.m_real, left.m_imaginary - right.m_imaginary);
    }

    public static ComplexF operator -(ComplexF left, float right)
    {
        return new ComplexF(left.m_real - right, left.m_imaginary);
    }

    public static ComplexF operator -(float left, ComplexF right)
    {
        return new ComplexF(left - right.m_real, -right.m_imaginary);
    }

    public static ComplexF operator *(ComplexF left, ComplexF right)
    {
        // Multiplication:  (a + bi)(c + di) = (ac -bd) + (bc + ad)i
        float result_realpart = (left.m_real * right.m_real) - (left.m_imaginary * right.m_imaginary);
        float result_imaginarypart = (left.m_imaginary * right.m_real) + (left.m_real * right.m_imaginary);
        return new ComplexF(result_realpart, result_imaginarypart);
    }

    public static ComplexF operator *(ComplexF left, float right)
    {
        if (!float.IsFinite(left.m_real))
        {
            if (!float.IsFinite(left.m_imaginary))
            {
                return new ComplexF(float.NaN, float.NaN);
            }

            return new ComplexF(left.m_real * right, float.NaN);
        }

        if (!float.IsFinite(left.m_imaginary))
        {
            return new ComplexF(float.NaN, left.m_imaginary * right);
        }

        return new ComplexF(left.m_real * right, left.m_imaginary * right);
    }

    public static ComplexF operator *(float left, ComplexF right)
    {
        if (!float.IsFinite(right.m_real))
        {
            if (!float.IsFinite(right.m_imaginary))
            {
                return new ComplexF(float.NaN, float.NaN);
            }

            return new ComplexF(left * right.m_real, float.NaN);
        }

        if (!float.IsFinite(right.m_imaginary))
        {
            return new ComplexF(float.NaN, left * right.m_imaginary);
        }

        return new ComplexF(left * right.m_real, left * right.m_imaginary);
    }

    public static ComplexF operator /(ComplexF left, ComplexF right)
    {
        // Division : Smith's formula.
        float a = left.m_real;
        float b = left.m_imaginary;
        float c = right.m_real;
        float d = right.m_imaginary;

        // Computing c * c + d * d will overflow even in cases where the actual result of the division does not overflow.
        if (Math.Abs(d) < Math.Abs(c))
        {
            float doc = d / c;
            return new ComplexF((a + b * doc) / (c + d * doc), (b - a * doc) / (c + d * doc));
        }
        else
        {
            float cod = c / d;
            return new ComplexF((b + a * cod) / (d + c * cod), (-a + b * cod) / (d + c * cod));
        }
    }

    public static ComplexF operator /(ComplexF left, float right)
    {
        // IEEE prohibit optimizations which are value changing
        // so we make sure that behaviour for the simplified version exactly match
        // full version.
        if (right == 0)
        {
            return new ComplexF(float.NaN, float.NaN);
        }

        if (!float.IsFinite(left.m_real))
        {
            if (!float.IsFinite(left.m_imaginary))
            {
                return new ComplexF(float.NaN, float.NaN);
            }

            return new ComplexF(left.m_real / right, float.NaN);
        }

        if (!float.IsFinite(left.m_imaginary))
        {
            return new ComplexF(float.NaN, left.m_imaginary / right);
        }

        // Here the actual optimized version of code.
        return new ComplexF(left.m_real / right, left.m_imaginary / right);
    }

    public static ComplexF operator /(float left, ComplexF right)
    {
        // Division : Smith's formula.
        float a = left;
        float c = right.m_real;
        float d = right.m_imaginary;

        // Computing c * c + d * d will overflow even in cases where the actual result of the division does not overflow.
        if (Math.Abs(d) < Math.Abs(c))
        {
            float doc = d / c;
            return new ComplexF(a / (c + d * doc), (-a * doc) / (c + d * doc));
        }
        else
        {
            float cod = c / d;
            return new ComplexF(a * cod / (d + c * cod), -a / (d + c * cod));
        }
    }

    public static float Abs(ComplexF value)
    {
        return Hypot(value.m_real, value.m_imaginary);
    }

    private static float Hypot(float a, float b)
    {
        // Using
        //   sqrt(a^2 + b^2) = |a| * sqrt(1 + (b/a)^2)
        // we can factor out the larger component to dodge overflow even when a * a would overflow.

        a = Math.Abs(a);
        b = Math.Abs(b);

        float small, large;
        if (a < b)
        {
            small = a;
            large = b;
        }
        else
        {
            small = b;
            large = a;
        }

        if (small == 0.0)
        {
            return (large);
        }
        else if (float.IsPositiveInfinity(large) && !float.IsNaN(small))
        {
            // The NaN test is necessary so we don't return +inf when small=NaN and large=+inf.
            // NaN in any other place returns NaN without any special handling.
            return (float.PositiveInfinity);
        }
        else
        {
            float ratio = small / large;
            return (large * MathF.Sqrt(1.0f + ratio * ratio));
        }
    }


    private static float Log1P(float x)
    {
        // Compute log(1 + x) without loss of accuracy when x is small.

        // Our only use case so far is for positive values, so this isn't coded to handle negative values.
        Debug.Assert((x >= 0.0) || float.IsNaN(x));

        float xp1 = 1.0f + x;
        if (xp1 == 1.0f)
        {
            return x;
        }
        else if (x < 0.75)
        {
            // This is accurate to within 5 ulp with any floating-point system that uses a guard digit,
            // as proven in Theorem 4 of "What Every Computer Scientist Should Know About Floating-Point
            // Arithmetic" (https://docs.oracle.com/cd/E19957-01/806-3568/ncg_goldberg.html)
            return x * MathF.Log(xp1) / (xp1 - 1.0f);
        }
        else
        {
            return MathF.Log(xp1);
        }
    }

    public static ComplexF Conjugate(ComplexF value)
    {
        // Conjugate of a Complex number: the conjugate of x+i*y is x-i*y
        return new ComplexF(value.m_real, -value.m_imaginary);
    }

    public static ComplexF Reciprocal(ComplexF value)
    {
        // Reciprocal of a Complex number : the reciprocal of x+i*y is 1/(x+i*y)
        if (value.m_real == 0 && value.m_imaginary == 0)
        {
            return Zero;
        }

        return One / value;
    }

    public static bool operator ==(ComplexF left, ComplexF right)
    {
        return left.m_real == right.m_real && left.m_imaginary == right.m_imaginary;
    }

    public static bool operator !=(ComplexF left, ComplexF right)
    {
        return left.m_real != right.m_real || left.m_imaginary != right.m_imaginary;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (!(obj is ComplexF)) return false;
        return Equals((ComplexF) obj);
    }

    public bool Equals(ComplexF value)
    {
        return m_real.Equals(value.m_real) && m_imaginary.Equals(value.m_imaginary);
    }

    public override int GetHashCode()
    {
        int n1 = 99999997;
        int realHash = m_real.GetHashCode() % n1;
        int imaginaryHash = m_imaginary.GetHashCode();
        int finalHash = realHash ^ imaginaryHash;
        return finalHash;
    }

    public override string ToString() => $"({m_real}, {m_imaginary})";

    public string ToString(string? format) => ToString(format, null);

    public string ToString(IFormatProvider? provider) => ToString(null, provider);

    public string ToString(string? format, IFormatProvider? provider)
    {
        return string.Format(provider, "({0}, {1})", m_real.ToString(format, provider),
            m_imaginary.ToString(format, provider));
    }


    public static bool IsFinite(ComplexF value) => float.IsFinite(value.m_real) && float.IsFinite(value.m_imaginary);

    public static bool IsInfinity(ComplexF value) =>
        float.IsInfinity(value.m_real) || float.IsInfinity(value.m_imaginary);

    public static bool IsNaN(ComplexF value) => !IsInfinity(value) && !IsFinite(value);

    public static ComplexF Pow(ComplexF value, ComplexF power)
    {
        if (power == Zero)
        {
            return One;
        }

        if (value == Zero)
        {
            return Zero;
        }

        float valueReal = value.m_real;
        float valueImaginary = value.m_imaginary;
        float powerReal = power.m_real;
        float powerImaginary = power.m_imaginary;

        float rho = Abs(value);
        float theta = MathF.Atan2(valueImaginary, valueReal);
        float newRho = powerReal * theta + powerImaginary * MathF.Log(rho);

        float t = MathF.Pow(rho, powerReal) * MathF.Pow(MathF.E, -powerImaginary * theta);

        return new ComplexF(t * MathF.Cos(newRho), t * MathF.Sin(newRho));
    }

    public static ComplexF Pow(ComplexF value, float power)
    {
        return Pow(value, new ComplexF(power, 0));
    }

    private static ComplexF Scale(ComplexF value, float factor)
    {
        float realResult = factor * value.m_real;
        float imaginaryResuilt = factor * value.m_imaginary;
        return new ComplexF(realResult, imaginaryResuilt);
    }
}