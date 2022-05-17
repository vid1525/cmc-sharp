using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ViewModel
{
    public interface IErrorReporter
    {
        void ReportError(string msg);
    }

    public class ViewData
    {
        public InputChecker Input { get; set; }
        public Splines.SplinesData SpData { get; set; }
        public ChartData ChData { get; set; } = new();
        public ObservableCollection<string> MeasuredDataValues { get; set; } = new();
        public ObservableCollection<string> SplinesDataValues { get; set; } = new();
        public ICommand MeasuredDataCmd { get; private set; }
        private readonly IErrorReporter MdErrorReporter;
        public ICommand SplinesDataCmd { get; private set; }
        private readonly IErrorReporter SpErrorReporter;
        public bool IsMeasured { get; set; } = false;

        private bool MdCanExecute(object x)
        {
            return !Input.ErrorMd;
        }

        private bool SpCanExecute(object x)
        {
            return !Input.ErrorSp && IsMeasured;
        }

        public ViewData(IErrorReporter mdErrorReporter, IErrorReporter spErrorReporter)
        {
            this.MdErrorReporter = mdErrorReporter;
            this.SpErrorReporter = spErrorReporter;

            MeasuredDataCmd = new RelayCommand(_ =>
                {
                    try
                    {
                        UpdateMeasuredData();
                        DrawMeasuredData();
                        IsMeasured = true;
                    }
                    catch (Exception ex)
                    {
                        MdErrorReporter.ReportError(ex.Message);
                    }
                }, MdCanExecute);
            
            SplinesDataCmd = new RelayCommand(_ =>
                {
                    try
                    {
                        UpdateSplineData();
                        DrawSplinesData();
                    }
                    catch (Exception ex)
                    {
                        SpErrorReporter.ReportError(ex.Message);
                    }
                }, SpCanExecute);
            Input = new InputChecker(Splines.SPf.CUBIC_FUNC, 3, 3, 0, 1, 0, 0, 0, 0);
        }

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
        }

        public void DrawMeasuredData()
        {
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
        }

        public void DrawSplinesData()
        {
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
