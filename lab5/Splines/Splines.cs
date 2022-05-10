using System;
using System.Runtime.InteropServices;


namespace Splines
{
    public enum SPf
    {
        CUBIC_FUNC = 1, // 0.1*x^3+2*x^2+9*x+1
        SQRT,   // Math.Sqrt
        RANDOM_FUNC
    }

    public class MeasuredData
    {
        public SPf Function { get; set; }
        public int PointsCount { get; set; }
        public double LeftBound { get; set; }
        public double RightBound { get; set; }
        public double[] GridArguments { get; private set; }
        public double[] CalculatedValues { get; set; }

        public MeasuredData(SPf function, int pointsCount, double leftBound, double rightBound)
        {
            Function = function;
            PointsCount = pointsCount;
            LeftBound = leftBound;
            RightBound = rightBound;
            GenerateArguments();
            CalculateValues();
        }

        public void GenerateArguments()
        {
            GridArguments = new double[PointsCount];
            GridArguments[0] = LeftBound;
            GridArguments[1] = RightBound;

            var range = RightBound - LeftBound;
            var rand = new Random();
            for (int i = 2; i < PointsCount; ++i)
            {
                GridArguments[i] = rand.NextDouble() * range + LeftBound;
            }
            Array.Sort(GridArguments);
        }

        delegate double CalculatingFunction(double x);

        public void CalculateValues()
        {
            CalculatingFunction func = (double x) => x;
            switch (Function)
            {
                case SPf.CUBIC_FUNC:
                    func = (double x) => 0.1 * x * x * x + 2 * x * x + 9 * x + 1;
                    break;
                case SPf.SQRT:
                    if (LeftBound < 0)
                    {
                        throw new ArgumentException("sqrt function doesn't support values lower than 0");
                    }
                    func = Math.Sqrt;
                    break;
                case SPf.RANDOM_FUNC:
                    func = (double x) => (new Random()).NextDouble();
                    break;
            }
            CalculatedValues = new double[PointsCount];
            for (int i = 0; i < PointsCount; ++i)
            {
                CalculatedValues[i] = func(GridArguments[i]);
            }
        }
    }

    public class SplineParameters
    {
        public int PointsCount { get; }
        public double[] Range { get; }
        public double FirstSplineLeftBound { get; set; }
        public double FirstSplineRightBound { get; set; }
        public double SecondSplineLeftBound { get; set; }
        public double SecondSplineRightBound { get; set; }

        public SplineParameters(int pointsCount, double leftBound, double rightBound, double fslb, double fsrb, double sslb, double ssrb)
        {
            PointsCount = pointsCount;
            Range = new double[pointsCount];

            double step = (rightBound - leftBound) / (pointsCount - 1);
            for (int i = 0; i < pointsCount; ++i)
            {
                Range[i] = leftBound + i * step;
            }
            FirstSplineLeftBound = fslb;
            FirstSplineRightBound = fsrb;
            SecondSplineLeftBound = sslb;
            SecondSplineRightBound = ssrb;
        }
    }

    public class SplinesData
    {
        public MeasuredData Md { get; set; }
        public SplineParameters Sp { get; set; }
        public double[] FirstSplineValues { get; private set; }
        public double[] SecondSplineValues { get; private set; }

        public SplinesData(MeasuredData md, SplineParameters sp)
        {
            Md = md;
            Sp = sp;
        }

        public void BuildSplines()
        {
            FirstSplineValues = new double[Sp.PointsCount];
            var status = CalculateSplines(
                Md.PointsCount, Md.GridArguments, Md.CalculatedValues,
                Sp.FirstSplineLeftBound, Sp.FirstSplineRightBound, Sp.PointsCount,
                FirstSplineValues);

            if (status != 0)
            {
                throw new Exception($"First spline interpolation failed with status code: {status}");
            }

            SecondSplineValues = new double[Sp.PointsCount];
            status = CalculateSplines(
                Md.PointsCount, Md.GridArguments, Md.CalculatedValues,
                Sp.SecondSplineLeftBound, Sp.SecondSplineRightBound, Sp.PointsCount,
                SecondSplineValues);

            if (status != 0)
            {
                throw new Exception($"Second spline interpolation failed with status code: {status}");
            }
        }

        [DllImport("..\\..\\..\\..\\x64\\Debug\\pinvoke_dll_lib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int CalculateSplines(
            int non_uniform_points_count, double[] x, double[] y, double second_der_left,
            double second_der_right, int uniform_points_count, double[] res);
    }
}
