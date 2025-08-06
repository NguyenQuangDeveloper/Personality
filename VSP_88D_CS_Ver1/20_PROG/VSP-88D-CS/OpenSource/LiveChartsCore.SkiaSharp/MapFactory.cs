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
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Segments;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace LiveChartsCore.SkiaSharpView;

/// <summary>
/// Defines a map builder.
/// </summary>
public class MapFactory : IMapFactory
{
    private readonly HashSet<LandAreaGeometry> _usedPathShapes = [];
    private readonly HashSet<Paint> _usedPaints = [];
    private readonly HashSet<string> _usedLayers = [];
    private IGeoMapView? _mapView;

    /// <inheritdoc cref="IMapFactory.GenerateLands(MapContext)"/>
    public void GenerateLands(MapContext context)
    {
        var projector = context.Projector;

        var toRemoveLayers = new HashSet<string>(_usedLayers);
        var toRemovePathShapes = new HashSet<LandAreaGeometry>(_usedPathShapes);
        var toRemovePaints = new HashSet<Paint>(_usedPaints);

        var layersQuery = context.View.ActiveMap.Layers.Values
            .Where(x => x.IsVisible)
            .OrderByDescending(x => x.ProcessIndex);

        _mapView = context.View;

        foreach (var layer in layersQuery)
        {
            var stroke = context.View.Stroke ?? layer.Stroke;
            var fill = context.View.Fill ?? layer.Fill;

            if (fill is not null)
            {
                context.View.Canvas.AddDrawableTask(fill);
                _ = _usedPaints.Add(fill);
                _ = toRemovePaints.Remove(fill);
            }
            if (stroke is not null)
            {
                context.View.Canvas.AddDrawableTask(stroke);
                _ = _usedPaints.Add(stroke);
                _ = toRemovePaints.Remove(stroke);
            }

            _ = _usedLayers.Add(layer.Name);
            _ = toRemoveLayers.Remove(layer.Name);

            foreach (var landDefinition in layer.Lands.Values)
            {
                foreach (var landData in landDefinition.Data)
                {
                    LandAreaGeometry shape;

                    if (landData.Shape is null)
                    {
                        landData.Shape = shape = new LandAreaGeometry();
                        shape.Animate(EasingFunctions.ExponentialOut, TimeSpan.FromMilliseconds(800));
                    }
                    else
                    {
                        shape = (LandAreaGeometry)landData.Shape;
                    }

                    _ = _usedPathShapes.Add(shape);
                    _ = toRemovePathShapes.Remove(shape);

                    stroke?.AddGeometryToPaintTask(context.View.Canvas, shape);
                    fill?.AddGeometryToPaintTask(context.View.Canvas, shape);

                    shape.Commands.Clear();

                    var isFirst = true;
                    float xp = 0, yp = 0;

                    foreach (var point in landData.Coordinates)
                    {
                        var p = projector.ToMap([point.X, point.Y]);

                        var x = p[0];
                        var y = p[1];

                        if (isFirst)
                        {
                            xp = x;
                            yp = y;
                        }

                        _ = shape.Commands.AddLast(new Segment
                        {
                            Xi = xp,
                            Yi = yp,
                            Xj = x,
                            Yj = y,
                        });
                    }
                }
            }

            foreach (var shape in toRemovePathShapes)
            {
                stroke?.RemoveGeometryFromPaintTask(context.View.Canvas, shape);
                fill?.RemoveGeometryFromPaintTask(context.View.Canvas, shape);

                shape.Commands.Clear();

                _ = _usedPathShapes.Remove(shape);
            }
        }

        foreach (var paint in toRemovePaints)
        {
            _ = _usedPaints.Remove(paint);
            context.View.Canvas.RemovePaintTask(paint);
        }

        foreach (var layerName in toRemoveLayers)
        {
            _ = context.MapFile.Layers.Remove(layerName);
            _ = _usedLayers.Remove(layerName);
        }
    }

    /// <inheritdoc cref="IMapFactory.ViewTo(GeoMapChart, object)"/>
    public void ViewTo(GeoMapChart sender, object? command) { }

    /// <inheritdoc cref="IMapFactory.Pan(GeoMapChart, LvcPoint)"/>
    public void Pan(GeoMapChart sender, LvcPoint delta) { }

    /// <summary>
    /// Disposes the map factory.
    /// </summary>
    public void Dispose()
    {
        if (_mapView is not null)
        {
            var layersQuery = _mapView.ActiveMap.Layers.Values
               .Where(x => x.IsVisible)
               .OrderByDescending(x => x.ProcessIndex);

            foreach (var layer in layersQuery)
            {
                var stroke = _mapView.Stroke ?? layer.Stroke;
                var fill = _mapView.Fill ?? layer.Fill;

                foreach (var landDefinition in layer.Lands.Values)
                {
                    foreach (var landData in landDefinition.Data)
                    {
                        var shape = landData.Shape;
                        if (shape is null) continue;

                        stroke?.RemoveGeometryFromPaintTask(_mapView.Canvas, shape);
                        fill?.AddGeometryToPaintTask(_mapView.Canvas, shape);

                        landData.Shape = null;
                    }
                }
                foreach (var paint in _usedPaints)
                {
                    _mapView.Canvas.RemovePaintTask(paint);
                    paint.ClearGeometriesFromPaintTask(_mapView.Canvas);
                }

                if (stroke is not null) _mapView.Canvas.RemovePaintTask(stroke);
                if (fill is not null) _mapView.Canvas.RemovePaintTask(fill);
            }
        }

        _usedPathShapes.Clear();
        _usedLayers.Clear();
        _usedPaints.Clear();
    }
}
