﻿// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;

namespace LiveChartsCore.Drawing;

/// <summary>
/// Defines a size.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LvcSize"/> struct.
/// </remarks>
/// <param name="width">The width.</param>
/// <param name="height">The height.</param>
public struct LvcSize(float width, float height)
{
    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    public float Width { get; set; } = width;

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    public float Height { get; set; } = height;

    /// <summary>
    /// Gets a new size that considers the given rotation [degrees].
    /// </summary>
    /// <param name="degrees">The rotation in degrees..</param>
    /// <returns>The rotated size.</returns>
    public readonly LvcSize GetRotatedSize(float degrees)
    {
        if (Math.Abs(degrees) < 0.001) return this;

        const double toRadians = Math.PI / 180;

        degrees %= 360;
        if (degrees < 0) degrees += 360;

        if (degrees > 180) degrees = 360 - degrees;
        if (degrees is > 90 and <= 180) degrees = 180 - degrees;

        var rRadians = degrees * toRadians;

        var w = (float)(Math.Cos(rRadians) * Width + Math.Sin(rRadians) * Height);
        var h = (float)(Math.Sin(rRadians) * Width + Math.Cos(rRadians) * Height);

        return new(w, h);
    }

    /// <summary>
    /// Determines whether the instance is equals to the given instance.
    /// </summary>
    /// <param name="obj">The instance to compare to.</param>
    /// <returns>The comparision result.</returns>
    public override readonly bool Equals(object? obj)
    {
        return obj is LvcSize size &&
            Width == size.Width &&
            Height == size.Height;
    }

    /// <summary>
    /// Gets the object hash code.
    /// </summary>
    /// <returns></returns>
    public override readonly int GetHashCode()
    {
        var hashCode = 859600377;
        hashCode = hashCode * -1521134295 + Width.GetHashCode();
        hashCode = hashCode * -1521134295 + Height.GetHashCode();
        return hashCode;
    }

    /// <summary>
    /// Compares two <see cref="LvcSize"/> instances.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(LvcSize left, LvcSize right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="LvcSize"/> instances.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(LvcSize left, LvcSize right) => !(left == right);
}
