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

using LiveChartsCore.Painting;

namespace LiveChartsCore.Drawing;

/// <summary>
/// Defines a context that is able to draw 2D shapes in the user interface.
/// </summary>
public abstract class DrawingContext
{
    /// <summary>
    /// Gets the active opacity.
    /// </summary>
    public float ActiveOpacity { get; internal set; }

    /// <summary>
    /// Gets the active paint task.
    /// </summary>
    /// <value>
    /// The paint task.
    /// </value>
    public Paint? ActiveLvcPaint { get; internal set; }

    /// <summary>
    /// Called when the frame starts.
    /// </summary>
    public virtual void OnBeginDraw()
    { }

    /// <summary>
    /// Draws the given string over the canvas.
    /// </summary>
    /// <param name="log">the log content.</param>
    public abstract void LogOnCanvas(string log);

    /// <summary>
    /// Called when the frame ends.
    /// </summary>
    public virtual void OnEndDraw()
    { }

    /// <summary>
    /// Draws the given element.
    /// </summary>
    /// <param name="drawable">The drawable element.</param>
    public abstract void Draw(IDrawnElement drawable);

    /// <summary>
    /// Initializes the task.
    /// </summary>
    /// <param name="paint"></param>
    public abstract void InitializePaintTask(Paint paint);

    /// <summary>
    /// Disposes the task.
    /// </summary>
    /// <param name="paint"></param>
    public abstract void DisposePaintTask(Paint paint);
}
