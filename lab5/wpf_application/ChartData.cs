using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;

namespace wpf_application
{
    public class ChartData
    {
        public SeriesCollection Series { get; set; }
        public Func<double, string> ResultFormatter { get; set; }

        public ChartData(string formatterString="f2")
        {
            Series = new();
            ResultFormatter = x => x.ToString(formatterString);
        }
        public void DrawMeasuredData(double[] x, double[] y)
        {
            Series.Add(
                new ScatterSeries
                {
                    Title = "Measured Data",
                    Fill = Brushes.Blue,
                    MinPointShapeDiameter = 4,
                    MaxPointShapeDiameter = 4,
                    Values = UpdateInputPoints(x, y)
                }
            );
        }

        public void DrawSplinesData(double[] x, double[] y, string title, int color = 0)
        {
            Series.Add(
                new LineSeries
                {
                    Title = title,
                    Values = UpdateInputPoints(x, y),
                    Fill = Brushes.Transparent,
                    Stroke = (color == 0 ? Brushes.Green : Brushes.Red),
                    PointGeometry = null,
                    LineSmoothness = 0
                }
            );
        }

        private ChartValues<ObservablePoint> UpdateInputPoints(double[] x, double[] y)
        {
            var res = new ChartValues<ObservablePoint>();
            for (int i = 0; i < y.Length; i++)
            {
                res.Add(new(x[i], y[i]));
            }
            return res;
        }
    }
}
