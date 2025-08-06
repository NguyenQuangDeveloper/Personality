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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.Themes;
using LiveChartsCore.VisualElements;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView;

/// <summary>
/// Defines the light theme extensions.
/// </summary>
public static class ThemesExtensions
{
    private static readonly object s_lightThemeKey = new();
    private static readonly object s_darkThemeKey = new();

    /// <summary>
    /// Adds the light theme.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <param name="additionalStyles">the additional styles.</param>
    /// <returns>The current LiveCharts settings.</returns>
    public static LiveChartsSettings AddLightTheme(
        this LiveChartsSettings settings, Action<Theme>? additionalStyles = null)
    {
        LiveCharts.HasTheme = true;
        settings.CurrentThemeId = s_lightThemeKey;

        return settings
            .HasTheme((Theme theme) =>
            {
                _ = LiveCharts.DefaultSettings
                    .WithAnimationsSpeed(TimeSpan.FromMilliseconds(800))
                    .WithEasingFunction(EasingFunctions.ExponentialOut);

                theme.Colors = ColorPalletes.MaterialDesign500;

                _ = theme
                    .HasDefaultTooltip(() => new SKDefaultTooltip())
                    .HasDefaultLegend(() => new SKDefaultLegend())
                    .HasRuleForAxes(axis =>
                    {
                        axis.TextSize = 16;
                        axis.ShowSeparatorLines = true;
                        axis.NamePaint = new SolidColorPaint(new SKColor(35, 35, 35));
                        axis.LabelsPaint = new SolidColorPaint(new SKColor(70, 70, 70));
                        if (axis is ICartesianAxis cartesian)
                        {
                            axis.SeparatorsPaint = cartesian.Orientation == AxisOrientation.X
                                ? null
                                : new SolidColorPaint(new SKColor(235, 235, 235));
                            cartesian.Padding = new Padding(12);
                        }
                        else if (axis is IPolarAxis polar)
                        {
                            polar.LabelsBackground = new LvcColor(255, 255, 255);
                            axis.SeparatorsPaint = new SolidColorPaint(new SKColor(235, 235, 235));
                        }
                        else
                        {
                            axis.SeparatorsPaint = new SolidColorPaint(new SKColor(235, 235, 235));
                        }
                    })
                    .HasRuleForAnySeries(series =>
                    {
                        series.Name = LiveCharts.IgnoreSeriesName;
                    })
                    .HasRuleForLineSeries(lineSeries =>
                    {
                        var color = theme.GetSeriesColor(lineSeries).AsSKColor();

                        lineSeries.GeometrySize = 12;
                        lineSeries.GeometryStroke = new SolidColorPaint(color, 4);
                        lineSeries.GeometryFill = new SolidColorPaint(new SKColor(250, 250, 250));
                        lineSeries.Stroke = new SolidColorPaint(color, 4);
                        lineSeries.Fill = new SolidColorPaint(color.WithAlpha(50));
                    })
                    .HasRuleForStepLineSeries(steplineSeries =>
                    {
                        var color = theme.GetSeriesColor(steplineSeries).AsSKColor();

                        steplineSeries.GeometrySize = 12;
                        steplineSeries.GeometryStroke = new SolidColorPaint(color, 4);
                        steplineSeries.GeometryFill = new SolidColorPaint(new SKColor(250, 250, 250));
                        steplineSeries.Stroke = new SolidColorPaint(color, 4);
                        steplineSeries.Fill = new SolidColorPaint(color.WithAlpha(50));
                    })
                    .HasRuleForStackedLineSeries(stackedLine =>
                    {
                        var color = theme.GetSeriesColor(stackedLine).AsSKColor();

                        stackedLine.GeometrySize = 0;
                        stackedLine.GeometryStroke = null;
                        stackedLine.GeometryFill = null;
                        stackedLine.Stroke = null;
                        stackedLine.Fill = new SolidColorPaint(color);
                    })
                    .HasRuleForBarSeries(barSeries =>
                    {
                        var color = theme.GetSeriesColor(barSeries).AsSKColor();

                        barSeries.Stroke = null;
                        barSeries.Fill = new SolidColorPaint(color);
                        barSeries.Rx = 3;
                        barSeries.Ry = 3;
                    })
                    .HasRuleForStackedBarSeries(stackedBarSeries =>
                    {
                        var color = theme.GetSeriesColor(stackedBarSeries).AsSKColor();

                        stackedBarSeries.Stroke = null;
                        stackedBarSeries.Fill = new SolidColorPaint(color);
                        stackedBarSeries.Rx = 0;
                        stackedBarSeries.Ry = 0;
                    })
                    .HasRuleForStackedStepLineSeries(stackedStep =>
                    {
                        var color = theme.GetSeriesColor(stackedStep).AsSKColor();

                        stackedStep.GeometrySize = 0;
                        stackedStep.GeometryStroke = null;
                        stackedStep.GeometryFill = null;
                        stackedStep.Stroke = null;
                        stackedStep.Fill = new SolidColorPaint(color);
                    })
                    .HasRuleForBoxSeries(boxSeries =>
                    {
                        var color = theme.GetSeriesColor(boxSeries).AsSKColor();

                        boxSeries.MaxBarWidth = 60;
                        boxSeries.Stroke = new SolidColorPaint(new SKColor(30, 30, 30), 2);
                        boxSeries.Fill = new SolidColorPaint(color);
                    })
                    .HasRuleForHeatSeries(heatSeries =>
                    {
                        // ... rules here
                    })
                    .HasRuleForFinancialSeries(financialSeries =>
                    {
                        financialSeries.UpFill = new SolidColorPaint(new SKColor(139, 195, 74, 255));
                        financialSeries.UpStroke = new SolidColorPaint(new SKColor(139, 195, 74, 255), 3);
                        financialSeries.DownFill = new SolidColorPaint(new SKColor(239, 83, 80, 255));
                        financialSeries.DownStroke = new SolidColorPaint(new SKColor(239, 83, 80, 255), 3);
                    })
                    .HasRuleForScatterSeries(scatterSeries =>
                    {
                        var color = theme.GetSeriesColor(scatterSeries).AsSKColor();

                        scatterSeries.Stroke = null;
                        scatterSeries.Fill = new SolidColorPaint(color.WithAlpha(200));
                    })
                    .HasRuleForPieSeries(pieSeries =>
                    {
                        var color = theme.GetSeriesColor(pieSeries).AsSKColor();

                        pieSeries.Stroke = null;
                        pieSeries.Fill = new SolidColorPaint(color);
                    })
                    .HasRuleForPolarLineSeries(polarLine =>
                    {
                        var color = theme.GetSeriesColor(polarLine).AsSKColor();

                        polarLine.GeometrySize = 12;
                        polarLine.GeometryStroke = new SolidColorPaint(color, 4);
                        polarLine.GeometryFill = new SolidColorPaint(new SKColor(250, 250, 250));
                        polarLine.Stroke = new SolidColorPaint(color, 4);
                        polarLine.Fill = new SolidColorPaint(color.WithAlpha(50));
                    })
                    .HasRuleForGaugeSeries(gaugeSeries =>
                    {
                        var color = theme.GetSeriesColor(gaugeSeries).AsSKColor();

                        gaugeSeries.Stroke = null;
                        gaugeSeries.Fill = new SolidColorPaint(color);
                        gaugeSeries.DataLabelsPosition = PolarLabelsPosition.ChartCenter;
                        gaugeSeries.DataLabelsPaint = new SolidColorPaint(new SKColor(70, 70, 70));
                        gaugeSeries.CornerRadius = 8;
                    })
                    .HasRuleForGaugeFillSeries(gaugeFill =>
                    {
                        gaugeFill.Fill = new SolidColorPaint(new SKColor(30, 30, 30, 10));
                    })
                    .HasRuleFor<BaseLabelVisual>(label =>
                    {
                        label.Paint = new SolidColorPaint(new SKColor(30, 30, 30));
                    })
                    .HasRuleFor<BaseNeedleVisual>(needle =>
                    {
                        needle.Fill = new SolidColorPaint(new SKColor(30, 30, 30));
                    })
                    .HasRuleFor<BaseAngularTicksVisual>(ticks =>
                    {
                        ticks.Stroke = new SolidColorPaint(new SKColor(30, 30, 30));
                        ticks.LabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30));
                    });

                additionalStyles?.Invoke(theme);
            });
    }

    /// <summary>
    /// Adds the dark theme.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <param name="additionalStyles">The additional styles.</param>
    /// <returns></returns>
    public static LiveChartsSettings AddDarkTheme(
        this LiveChartsSettings settings, Action<Theme>? additionalStyles = null)
    {
        LiveCharts.HasTheme = true;
        settings.CurrentThemeId = s_darkThemeKey;

        return settings
            .HasTheme((Theme theme) =>
            {
                _ = LiveCharts.DefaultSettings
                    .WithAnimationsSpeed(TimeSpan.FromMilliseconds(800))
                    .WithEasingFunction(EasingFunctions.ExponentialOut)
                    .WithTooltipBackgroundPaint(new SolidColorPaint(new SKColor(45, 45, 45)))
                    .WithTooltipTextPaint(new SolidColorPaint(new SKColor(245, 245, 245)))
                    .WithLegendTextPaint(new SolidColorPaint(new SKColor(245, 245, 245)));

                theme.Colors = ColorPalletes.MaterialDesign200;

                _ = theme
                    .HasDefaultTooltip(() => new SKDefaultTooltip())
                    .HasDefaultLegend(() => new SKDefaultLegend())
                    .HasRuleForAxes(axis =>
                    {
                        axis.TextSize = 16;
                        axis.ShowSeparatorLines = true;
                        axis.NamePaint = new SolidColorPaint(new SKColor(235, 235, 235));
                        axis.LabelsPaint = new SolidColorPaint(new SKColor(200, 200, 200));

                        if (axis is ICartesianAxis cartesian)
                        {
                            axis.SeparatorsPaint = cartesian.Orientation == AxisOrientation.X
                                ? null
                                : new SolidColorPaint(new SKColor(90, 90, 90));
                            cartesian.Padding = new Padding(12);
                        }
                        else if (axis is IPolarAxis polar)
                        {
                            polar.LabelsBackground = new LvcColor(0, 0, 0);
                            axis.SeparatorsPaint = new SolidColorPaint(new SKColor(90, 90, 90));
                        }
                        else
                        {
                            axis.SeparatorsPaint = new SolidColorPaint(new SKColor(90, 90, 90));
                        }
                    })
                    .HasRuleForAnySeries(series =>
                    {
                        series.Name = LiveCharts.IgnoreSeriesName;
                    })
                    .HasRuleForLineSeries(lineSeries =>
                    {
                        var color = theme.GetSeriesColor(lineSeries).AsSKColor();

                        lineSeries.GeometrySize = 12;
                        lineSeries.GeometryStroke = new SolidColorPaint(color, 4);
                        lineSeries.GeometryFill = new SolidColorPaint(new SKColor(30, 30, 30));
                        lineSeries.Stroke = new SolidColorPaint(color, 4);
                        lineSeries.Fill = new SolidColorPaint(color.WithAlpha(50));
                    })
                    .HasRuleForStepLineSeries(steplineSeries =>
                    {
                        var color = theme.GetSeriesColor(steplineSeries).AsSKColor();

                        steplineSeries.GeometrySize = 12;
                        steplineSeries.GeometryStroke = new SolidColorPaint(color, 4);
                        steplineSeries.GeometryFill = new SolidColorPaint(new SKColor(30, 30, 30));
                        steplineSeries.Stroke = new SolidColorPaint(color, 4);
                        steplineSeries.Fill = new SolidColorPaint(color.WithAlpha(50));
                    })
                    .HasRuleForStackedLineSeries(stackedLine =>
                    {
                        var color = theme.GetSeriesColor(stackedLine).AsSKColor();

                        stackedLine.GeometrySize = 0;
                        stackedLine.GeometryStroke = null;
                        stackedLine.GeometryFill = null;
                        stackedLine.Stroke = null;
                        stackedLine.Fill = new SolidColorPaint(color);
                    })
                    .HasRuleForBarSeries(barSeries =>
                    {
                        var color = theme.GetSeriesColor(barSeries).AsSKColor();

                        barSeries.Stroke = null;
                        barSeries.Fill = new SolidColorPaint(color);
                        barSeries.Rx = 3;
                        barSeries.Ry = 3;
                    })
                    .HasRuleForStackedBarSeries(stackedBarSeries =>
                    {
                        var color = theme.GetSeriesColor(stackedBarSeries).AsSKColor();

                        stackedBarSeries.Stroke = null;
                        stackedBarSeries.Fill = new SolidColorPaint(color);
                        stackedBarSeries.Rx = 0;
                        stackedBarSeries.Ry = 0;
                    })
                    .HasRuleForScatterSeries(scatterSeries =>
                    {
                        var color = theme.GetSeriesColor(scatterSeries).AsSKColor();

                        scatterSeries.Stroke = null;
                        scatterSeries.Fill = new SolidColorPaint(color.WithAlpha(200));
                    })
                    .HasRuleForPieSeries(pieSeries =>
                    {
                        var color = theme.GetSeriesColor(pieSeries).AsSKColor();

                        pieSeries.Stroke = null;
                        pieSeries.Fill = new SolidColorPaint(color);
                    })
                    .HasRuleForStackedStepLineSeries(stackedStep =>
                    {
                        var color = theme.GetSeriesColor(stackedStep).AsSKColor();

                        stackedStep.GeometrySize = 0;
                        stackedStep.GeometryStroke = null;
                        stackedStep.GeometryFill = null;
                        stackedStep.Stroke = null;
                        stackedStep.Fill = new SolidColorPaint(color);
                    })
                    .HasRuleForBoxSeries(boxSeries =>
                    {
                        var color = theme.GetSeriesColor(boxSeries).AsSKColor();

                        boxSeries.MaxBarWidth = 60;
                        boxSeries.Stroke = new SolidColorPaint(new SKColor(220, 220, 220), 2);
                        boxSeries.Fill = new SolidColorPaint(color);
                    })
                    .HasRuleForHeatSeries(heatSeries =>
                    {
                        // ... rules here
                    })
                    .HasRuleForFinancialSeries(financialSeries =>
                    {
                        financialSeries.UpFill = new SolidColorPaint(new SKColor(139, 195, 74, 255));
                        financialSeries.UpStroke = new SolidColorPaint(new SKColor(139, 195, 74, 255), 3);
                        financialSeries.DownFill = new SolidColorPaint(new SKColor(239, 83, 80, 255));
                        financialSeries.DownStroke = new SolidColorPaint(new SKColor(239, 83, 80, 255), 3);
                    })
                    .HasRuleForPolarLineSeries(polarLine =>
                    {
                        var color = theme.GetSeriesColor(polarLine).AsSKColor();

                        polarLine.GeometrySize = 12;
                        polarLine.GeometryStroke = new SolidColorPaint(color, 4);
                        polarLine.GeometryFill = new SolidColorPaint(new SKColor(30, 30, 30));
                        polarLine.Stroke = new SolidColorPaint(color, 4);
                        polarLine.Fill = new SolidColorPaint(color.WithAlpha(50));
                    })
                    .HasRuleForGaugeSeries(gaugeSeries =>
                    {
                        var color = theme.GetSeriesColor(gaugeSeries).AsSKColor();

                        gaugeSeries.Stroke = null;
                        gaugeSeries.Fill = new SolidColorPaint(color);
                        gaugeSeries.DataLabelsPosition = PolarLabelsPosition.ChartCenter;
                        gaugeSeries.DataLabelsPaint = new SolidColorPaint(new SKColor(200, 200, 200));
                        gaugeSeries.CornerRadius = 8;
                    })
                    .HasRuleForGaugeFillSeries(gaugeFill =>
                    {
                        gaugeFill.Fill = new SolidColorPaint(new SKColor(255, 255, 255, 30));
                    })
                    .HasRuleFor<BaseLabelVisual>(label =>
                    {
                        label.Paint = new SolidColorPaint(new SKColor(200, 200, 200));
                    })
                    .HasRuleFor<BaseNeedleVisual>(needle =>
                    {
                        needle.Fill = new SolidColorPaint(new SKColor(200, 200, 200));
                    })
                    .HasRuleFor<BaseAngularTicksVisual>(ticks =>
                    {
                        ticks.Stroke = new SolidColorPaint(new SKColor(200, 200, 200));
                        ticks.LabelsPaint = new SolidColorPaint(new SKColor(200, 200, 200));
                    });

                additionalStyles?.Invoke(theme);
            });
    }
}
