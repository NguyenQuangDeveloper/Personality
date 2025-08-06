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
using System.Collections.Generic;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;

namespace LiveChartsCore.Kernel.Sketches;

/// <summary>
/// Defines a Cartesian chart view, this view is able to host one or many series in a Cartesian coordinate system.
/// </summary>
/// <seealso cref="IChartView" />
public interface ICartesianChartView : IChartView
{
    /// <summary>
    /// Gets the core.
    /// </summary>
    /// <value>
    /// The core.
    /// </value>
    CartesianChartEngine Core { get; }

    /// <summary>
    /// Gets or sets the x axes.
    /// </summary>
    /// <value>
    /// The x axes.
    /// </value>
    IEnumerable<ICartesianAxis> XAxes { get; set; }

    /// <summary>
    /// Gets or sets the y axes.
    /// </summary>
    /// <value>
    /// The y axes.
    /// </value>
    IEnumerable<ICartesianAxis> YAxes { get; set; }

    /// <summary>
    /// Gets or sets the sections.
    /// </summary>
    /// <value>
    /// The sections.
    /// </value>
    IEnumerable<CoreSection> Sections { get; set; }

    /// <summary>
    /// Gets or sets the series to plot in the user interface.
    /// </summary>
    /// <value>
    /// The series.
    /// </value>
    IEnumerable<ISeries> Series { get; set; }

    /// <summary>
    /// Gets or sets the draw margin frame.
    /// </summary>
    /// <value>
    /// The draw margin frame.
    /// </value>
    CoreDrawMarginFrame? DrawMarginFrame { get; set; }

    /// <summary>
    /// Gets or sets the zoom mode.
    /// </summary>
    /// <value>
    /// The zoom mode.
    /// </value>
    ZoomAndPanMode ZoomMode { get; set; }

    /// <summary>
    /// Gets or sets the finding strategy.
    /// </summary>
    /// <value>
    /// The finding strategy.
    /// </value>
    FindingStrategy FindingStrategy { get; set; }

    /// <summary>
    /// Gets or sets the zooming speed from 0 to 1, where 0 is the slowest and 1 the fastest.
    /// </summary>
    /// <value>
    /// The zooming speed.
    /// </value>
    double ZoomingSpeed { get; set; }

    /// <summary>
    /// Matches the axes screen data ratio, this method forces the x and y axes to use the same scale,
    /// this means that both axes will have the same number of pixels per data unit.
    /// </summary>
    bool MatchAxesScreenDataRatio { get; set; }

    /// <summary>
    /// Scales a point in pixels to the chart data scale.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="xAxisIndex">Index of the x axis.</param>
    /// <param name="yAxisIndex">Index of the y axis.</param>
    /// <returns></returns>
    LvcPointD ScalePixelsToData(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0);

    /// <summary>
    /// Scales a point in the chart data scale to pixels.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="xAxisIndex">Index of the x axis.</param>
    /// <param name="yAxisIndex">Index of the y axis.</param>
    /// <returns></returns>
    LvcPointD ScaleDataToPixels(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0);
}

/// <summary>
/// Obsolete.
/// </summary>
/// <typeparam name="T"></typeparam>
[Obsolete("Replaced by the non generic ICartesianChartView interface.")]
public interface ICartesianChartView<T> : ICartesianChartView
    where T : DrawingContext
{ }
