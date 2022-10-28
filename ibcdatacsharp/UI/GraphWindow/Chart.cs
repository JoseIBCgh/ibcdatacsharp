using Arction.Wpf.Charting;
using Arction.Wpf.Charting.Axes;
using Arction.Wpf.Charting.SeriesXY;
using Arction.Wpf.Charting.Views.ViewXY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;

namespace ibcdatacsharp.UI.GraphWindow
{
    public class Chart
    {
        private const int _seriesCount = 3;
        private const double XInterval = 1;
        private double YMin;
        private double YMax;
        private string YUnits;
        private int _pointsAppended;
        public Chart(string name, double YMin, double YMax, string YUnits)
        {
            this.YMin = YMin;
            this.YMax = YMax;
            this.YUnits = YUnits;
            CreateChart(name);
        }
        public LightningChart chart { get; private set; }

        private void CreateChart(string name)
        {
            chart = new LightningChart();

            chart.BeginUpdate();
            chart.ChartName = name;
            chart.ChartRenderOptions.DeviceType = RendererDeviceType.AutoPreferD11;
            chart.ChartRenderOptions.LineAAType2D = LineAntiAliasingType.QLAA;

            chart.Title.Font.Size = 16;
            chart.Title.Text = "";//"Set options and press Start.\nPC with 16GB RAM + fast graphics card is strongly recommended";
            chart.Title.Color = Color.FromArgb(255, 255, 204, 0);
            chart.Title.Align = ChartTitleAlignment.TopCenter;

            ViewXY view = chart.ViewXY;

            //Set real-time monitoring scroll mode 
            view.XAxes[0].ScrollMode = XAxisScrollMode.Scrolling;
            view.XAxes[0].SweepingGap = 0;
            view.XAxes[0].ValueType = AxisValueType.Number;
            view.XAxes[0].AutoFormatLabels = false;
            view.XAxes[0].LabelsNumberFormat = "N0";
            view.XAxes[0].Title.Text = "Frames";
            view.XAxes[0].SetRange(0, 100000);
            view.XAxes[0].MajorGrid.Pattern = LinePattern.Solid;

            //Set real-time monitoring automatic old data destruction
            view.DropOldSeriesData = true;

            //Set Axis layout to Segmented
            view.AxisLayout.YAxesLayout = YAxesLayout.Stacked;
            view.AxisLayout.SegmentsGap = 2;
            view.AxisLayout.YAxisAutoPlacement = YAxisAutoPlacement.LeftThenRight;
            view.AxisLayout.YAxisTitleAutoPlacement = true;

            // fix margins to prevent Graph resize, which may take long time for Billion points
            view.AxisLayout.AutoAdjustMargins = false;
            view.Margins = new Thickness(70, 17, 70, 34);


            //Create a dark sweeping gradient band for old page
            Band sweepBandDark = new Band(view, view.XAxes[0], view.YAxes[0])
            {
                BorderWidth = 0
            };
            sweepBandDark.Fill.Color = Color.FromArgb(255, 0, 0, 0);
            sweepBandDark.Fill.GradientColor = Color.FromArgb(0, 0, 0, 0);
            sweepBandDark.Fill.GradientFill = GradientFill.Linear;
            sweepBandDark.Fill.GradientDirection = 0;
            sweepBandDark.Binding = AxisBinding.XAxis;
            sweepBandDark.AllowUserInteraction = false;
            view.Bands.Add(sweepBandDark);

            //Create a bright sweeping gradient band, for new page
            Band sweepBandBright = new Band(view, view.XAxes[0], view.YAxes[0])
            {
                BorderWidth = 0
            };
            sweepBandBright.Fill.Color = Color.FromArgb(0, 0, 0, 0);
            sweepBandBright.Fill.GradientColor = Color.FromArgb(150, 255, 255, 255);
            sweepBandBright.Fill.GradientFill = GradientFill.Linear;
            sweepBandBright.Fill.GradientDirection = 0;
            sweepBandBright.Binding = AxisBinding.XAxis;
            sweepBandBright.AllowUserInteraction = false;
            view.Bands.Add(sweepBandBright);

            //Don't show legend box
            view.LegendBoxes[0].Visible = false;

            chart.EndUpdate();

            Start();
        }
        public void Start()
        {
            _pointsAppended = 0;

            ViewXY v = chart.ViewXY;


            chart.BeginUpdate();

            chart.ViewXY.AxisLayout.AutoShrinkSegmentsGap = true;

            //Clear Data series
            DisposeAllAndClear(v.SampleDataBlockSeries);

            //Clear Y axes
            DisposeAllAndClear(v.YAxes);

            AxisY axisY = new AxisY(v);
            axisY.SetRange(YMin, YMax);

            axisY.Title.Angle = 0;
            axisY.Title.Color = ChartTools.CalcGradient(DefaultColors.SeriesForBlackBackgroundWpf[0], System.Windows.Media.Colors.White, 50);
            axisY.Units.Visible = false;
            axisY.AllowScaling = false;
            axisY.MajorGrid.Visible = false;
            axisY.MinorGrid.Visible = false;
            axisY.MajorGrid.Pattern = LinePattern.Solid;
            axisY.AutoDivSeparationPercent = 0;
            axisY.Units.Text = YUnits;
            axisY.Visible = true;
            axisY.MajorDivTickStyle.Alignment = Alignment.Near;
            axisY.Title.HorizontalAlign = YAxisTitleAlignmentHorizontal.Left;

            //Create a mini-scale for last axis, it's used when Y axes are hidden.
            axisY.MiniScale.ShowX = true;
            axisY.MiniScale.ShowY = true;
            axisY.MiniScale.Color = Color.FromArgb(255, 255, 204, 0);
            axisY.MiniScale.HorizontalAlign = AlignmentHorizontal.Right;
            axisY.MiniScale.VerticalAlign = AlignmentVertical.Bottom;
            axisY.MiniScale.Offset = new PointIntXY(-30, -30);
            axisY.MiniScale.LabelX.Color = Colors.White;
            axisY.MiniScale.LabelY.Color = Colors.White;
            axisY.MiniScale.PreferredSize = new Arction.Wpf.Charting.SizeDoubleXY(50, 50);
            v.YAxes.Add(axisY);
            //Series count of Y axes and SampleDataBlockSeries  
            for (int seriesIndex = 0; seriesIndex < _seriesCount; seriesIndex++)
            {
                Color lineBaseColor = DefaultColors.SeriesForBlackBackgroundWpf[seriesIndex % DefaultColors.SeriesForBlackBackgroundWpf.Length];

                SampleDataBlockSeries series = new SampleDataBlockSeries(v, v.XAxes[0], axisY);
                v.SampleDataBlockSeries.Add(series);
                series.Color = ChartTools.CalcGradient(lineBaseColor, System.Windows.Media.Colors.White, 50);

                series.SamplingFrequency = 1.0 / XInterval; //Set 1 / X interval here 
                series.FirstSampleTimeStamp = 1.0 / series.SamplingFrequency;//Set first X here 
                series.ScrollModePointsKeepLevel = 1;
                series.AllowUserInteraction = false;
            }

            v.XAxes[0].SetRange(0, 100);


            chart.EndUpdate();
        }
        public void update(float[] data)
        {
            if (chart != null)
            {
                chart.BeginUpdate();

                //Append data to series
                System.Threading.Tasks.Parallel.For(0, _seriesCount, (seriesIndex) =>
                {
                    float[] dataToAppendNow = new float[1] { data[seriesIndex] };
                    chart.ViewXY.SampleDataBlockSeries[seriesIndex].AddSamples(dataToAppendNow, false);
                });

                _pointsAppended += 1;

                //Set X axis real-time scrolling position 
                double lastX = _pointsAppended * XInterval;
                chart.ViewXY.XAxes[0].ScrollPosition = lastX;

                //Update sweep bands
                if (chart.ViewXY.XAxes[0].ScrollMode == XAxisScrollMode.Sweeping)
                {

                    //Dark band of old page fading away 
                    double pageLen = chart.ViewXY.XAxes[0].Maximum - chart.ViewXY.XAxes[0].Minimum;
                    double sweepGapWidth = pageLen / 20.0;
                    chart.ViewXY.Bands[0].SetValues(lastX - pageLen, lastX - pageLen + sweepGapWidth);
                    if (chart.ViewXY.Bands[0].Visible == false)
                    {
                        chart.ViewXY.Bands[0].Visible = true;
                    }


                    //Bright new page band
                    chart.ViewXY.Bands[1].SetValues(lastX - sweepGapWidth / 6, lastX);
                    if (chart.ViewXY.Bands[1].Visible == false)
                    {
                        chart.ViewXY.Bands[1].Visible = true;
                    }
                }
                else
                {
                    //Hide sweeping bands if not in sweeping mode
                    if (chart.ViewXY.Bands[0].Visible == true)
                    {
                        chart.ViewXY.Bands[0].Visible = false;
                    }

                    if (chart.ViewXY.Bands[1].Visible == true)
                    {
                        chart.ViewXY.Bands[1].Visible = false;
                    }
                }
                chart.EndUpdate();
            }
        }
        public static void DisposeAllAndClear<T>(List<T> list) where T : IDisposable
        {
            if (list == null)
            {
                return;
            }

            while (list.Count > 0)
            {
                int lastInd = list.Count - 1;
                T item = list[lastInd]; // take item ref from list. 
                list.RemoveAt(lastInd); // remove item first
                if (item != null)
                {
                    (item as IDisposable).Dispose();     // then dispose it. 
                }
            }
        }
    }
}
