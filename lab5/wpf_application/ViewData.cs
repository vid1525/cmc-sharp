using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace wpf_application
{
    public class InputChecker : IDataErrorInfo
    {
        private int NonUniformPointsCount_;
        private int UniformPointsCount_;
        private double LeftBound_;
        private double RightBound_;
        private double FirstLeftSecondDerValue_;
        private double FirstRightSecondDerValue_;
        private double SecondLeftSecondDerValue_;
        private double SecondRightSecondDerValue_;
        public Splines.SPf Function { get; set; }
        public bool ErrorMd { get; private set; } = false;
        public bool ErrorSp { get; private set; } = false;

        public int UniformPointsCount
        {
            get => UniformPointsCount_;
            set
            {
                UniformPointsCount_ = value;
                ErrorSp = false;
            }
        }

        public int NonUniformPointsCount
        {
            get => NonUniformPointsCount_;
            set
            {
                NonUniformPointsCount_ = value;
                ErrorMd = false;
            }
        }

        public double LeftBound
        {
            get => LeftBound_;
            set
            {
                LeftBound_ = value;
                ErrorMd = false;
            }
        }

        public double RightBound
        {
            get => RightBound_;
            set
            {
                RightBound_ = value;
                ErrorMd = false;
            }
        }

        public double FirstLeftSecondDerValue
        {
            get => FirstLeftSecondDerValue_;
            set
            {
                FirstLeftSecondDerValue_ = value;
                ErrorSp = false;
            }
        }

        public double FirstRightSecondDerValue
        {
            get => FirstRightSecondDerValue_;
            set
            {
                FirstRightSecondDerValue_ = value;
                ErrorSp = false;
            }
        }

        public double SecondLeftSecondDerValue
        {
            get => SecondLeftSecondDerValue_;
            set
            {
                SecondLeftSecondDerValue_ = value;
                ErrorSp = false;
            }
        }

        public double SecondRightSecondDerValue
        {
            get => SecondRightSecondDerValue_;
            set
            {
                SecondRightSecondDerValue_ = value;
                ErrorSp = false;
            }
        }

        public InputChecker(
            Splines.SPf function,
            int nonUniformLength, int uniformLength,
            double leftBound, double rightBound,
            double firstLeftDer, double firstRightDer,
            double secondLeftDer, double secondRightDer
        )
        {
            Function = function;
            UniformPointsCount = uniformLength;
            NonUniformPointsCount = nonUniformLength;
            LeftBound = leftBound;
            RightBound = rightBound;
            FirstLeftSecondDerValue = firstLeftDer;
            FirstRightSecondDerValue = firstRightDer;
            SecondLeftSecondDerValue = secondLeftDer;
            SecondRightSecondDerValue = secondRightDer;
        }

        public string this[string property]
        {
            get
            {
                string msg = null;
                switch (property)
                {
                    case "UniformPointsCount":
                        if (UniformPointsCount <= 2)
                        {
                            msg = "Invalid uniform points count";
                            ErrorSp = true;
                        }
                        break;
                    case "NonUniformPointsCount":
                        if (NonUniformPointsCount <= 2)
                        {
                            msg = "Invalid non uniform points count";
                            ErrorMd = true;
                        }
                        break;
                    case "LeftBound":
                    case "RightBound":
                        if (LeftBound >= RightBound)
                        {
                            msg = "Invalid boundaries";
                            ErrorMd = true;
                            ErrorSp = true;
                        }
                        break;
                    default:
                        break;
                }
                return msg;
            }
        }

        public string Error
        {
            get
            {
                return "Some error occured.";
            }
        }
    }

    public class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{(double)value}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return double.Parse((string)value, culture);
            }
            catch(Exception)
            {
                return 0;
            }
        }
    }

    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{(int)value}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return Int32.Parse((string)value, culture);
            }
            catch
            {
                return 0;
            }
        }
    }

    public class StringToFunctionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (Splines.SPf)value;

            if (val == Splines.SPf.CUBIC_FUNC)
            {
                return "Cubic";
            }
            else if (val == Splines.SPf.SQRT)
            {
                return "Sqrt";
            }
            else if (val == Splines.SPf.RANDOM_FUNC)
            {
                return "Random";
            }

            throw new InvalidEnumArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = ((System.Windows.Controls.TextBlock)value).Text;

            if (str == "Cubic")
            {
                return Splines.SPf.CUBIC_FUNC;
            }
            else if (str == "Sqrt")
            {
                return Splines.SPf.SQRT;
            }
            else if (str == "Random")
            {
                return Splines.SPf.RANDOM_FUNC;
            }

            throw new InvalidEnumArgumentException();
        }
    }

    public class ViewData
    {
        public InputChecker Input { get; set; }
        public Splines.SplinesData SpData { get; set; }
        public ChartData ChData { get; set; } = new();
        public ObservableCollection<string> MeasuredDataValues { get; set; } = new();
        public ObservableCollection<string> SplinesDataValues { get; set; } = new();

        public void UpdateMeasuredData()
        {
            if (SpData is null)
            {
                SpData = new Splines.SplinesData(null, null);
            }
            SpData.Md = new Splines.MeasuredData(Input.Function, Input.NonUniformPointsCount, Input.LeftBound, Input.RightBound);

            MeasuredDataValues.Clear();
            for (int i = 0; i < SpData.Md.PointsCount; ++i)
            {
                MeasuredDataValues.Add($"x: {SpData.Md.GridArguments[i]}\nY: {SpData.Md.CalculatedValues[i]}");
            }

            ChData.Series.Clear();
            ChData.DrawMeasuredData(SpData.Md.GridArguments, SpData.Md.CalculatedValues);
        }

        public void UpdateSplineData()
        {
            if (SpData is null)
            {
                throw new ArgumentException("Measured data is not defined.");
            }
            SpData.Sp = new Splines.SplineParameters(
                Input.UniformPointsCount, Input.LeftBound, Input.RightBound, Input.FirstLeftSecondDerValue,
                Input.FirstRightSecondDerValue, Input.SecondLeftSecondDerValue, Input.SecondRightSecondDerValue);

            SpData.BuildSplines();

            SplinesDataValues.Clear();

            SplinesDataValues.Add("FIRST SECOND DER INPUT VALUES:");
            SplinesDataValues.Add($"Second der in input:\na: {Input.FirstLeftSecondDerValue}\nb: {Input.FirstRightSecondDerValue}");
            SplinesDataValues.Add("MKL first result:");
            SplinesDataValues.Add(
                $"Spline values:\na: {SpData.FirstSplineValues[0]}\na+h: {SpData.FirstSplineValues[1]}\nb-h: " +
                $"{SpData.FirstSplineValues[SpData.Sp.PointsCount - 2]}\nb: {SpData.FirstSplineValues[SpData.Sp.PointsCount - 1]}");

            SplinesDataValues.Add("SECOND SECOND DER INPUT VALUES:");
            SplinesDataValues.Add($"Second der in input:\na: {Input.SecondLeftSecondDerValue}\nb: {Input.SecondRightSecondDerValue}");
            SplinesDataValues.Add("MKL second result:");
            SplinesDataValues.Add(
                $"Spline values:\na: {SpData.SecondSplineValues[0]}\na+h: {SpData.SecondSplineValues[1]}\nb-h: " +
                $"{SpData.SecondSplineValues[SpData.Sp.PointsCount - 2]}\nb: {SpData.SecondSplineValues[SpData.Sp.PointsCount - 1]}");

            if (ChData.Series.Count > 1)
            {
                ChData.Series.RemoveAt(ChData.Series.Count - 1);
                ChData.Series.RemoveAt(ChData.Series.Count - 1);
            }
            ChData.DrawSplinesData(SpData.Sp.Range, SpData.FirstSplineValues, "Spline #1");
            ChData.DrawSplinesData(SpData.Sp.Range, SpData.SecondSplineValues, "Spline #2", 1);
        }
    }
}
