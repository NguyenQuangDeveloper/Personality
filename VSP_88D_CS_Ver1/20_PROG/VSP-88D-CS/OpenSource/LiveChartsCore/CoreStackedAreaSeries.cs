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

using System.Collections.Generic;
using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Segments;

namespace LiveChartsCore;

/// <summary>
/// Defines the stacked area series class.
/// </summary>
/// <typeparam name="TModel">The type of the model to plot.</typeparam>
/// <typeparam name="TVisual">The type of the visual point.</typeparam>
/// <typeparam name="TLabel">The type of the data label.</typeparam>
/// <typeparam name="TPathGeometry">The type of the path geometry.</typeparam>
/// <typeparam name="TErrorGeometry">The type of the error geometry.</typeparam>
/// <seealso cref="CoreLineSeries{TModel, TVisual, TLabel, TPathGeometry, TErrorGeometry}" />
public abstract class CoreStackedAreaSeries<TModel, TVisual, TLabel, TPathGeometry, TErrorGeometry>
    : CoreLineSeries<TModel, TVisual, TLabel, TPathGeometry, TErrorGeometry>
        where TPathGeometry : BaseVectorGeometry<CubicBezierSegment>, new()
        where TVisual : BoundedDrawnGeometry, new()
        where TLabel : BaseLabelGeometry, new()
        where TErrorGeometry : BaseLineGeometry, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CoreStackedAreaSeries{TModel, TVisual, TLabel, TPathGeometry, TBezierVisual}"/> class.
    /// </summary>
    /// <param name="values">The values.</param>
    public CoreStackedAreaSeries(IReadOnlyCollection<TModel>? values)
        : base(values, true)
    {
        GeometryFill = null;
        GeometryStroke = null;
        GeometrySize = 0;
    }
}
