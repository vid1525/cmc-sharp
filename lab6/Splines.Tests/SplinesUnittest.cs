using System;
using Xunit;

namespace Splines.Tests
{
    public class MeasuredDataTests
    {
        [Fact]
        public void TestConstructorGeneratedArguments()
        {
            double left = -10;
            double right = 20;
            int cnt = 11;

            var md = new MeasuredData(SPf.CUBIC_FUNC, cnt, left, right);

            Assert.Equal(left, md.LeftBound);
            Assert.Equal(right, md.RightBound);
            Assert.Equal(cnt, md.PointsCount);
            Assert.Equal(cnt, md.GridArguments.Length);
            Assert.Equal(cnt, md.CalculatedValues.Length);

            for (int i = 1; i < cnt; ++i)
            {
                Assert.True(md.GridArguments[i - 1] < md.GridArguments[i]);
            }
            Assert.Equal(left, md.GridArguments[0]);
            Assert.Equal(right, md.GridArguments[cnt - 1]);
        }

        [Fact]
        public void TestUpdatedGeneratedArguments()
        {
            double left = -10;
            double right = 20;
            int cnt = 11;

            var md = new MeasuredData(SPf.CUBIC_FUNC, cnt, left, right);

            left += 6;
            right += 10;
            cnt += 10;
            md.PointsCount = cnt;
            md.LeftBound = left;
            md.RightBound = right;
            md.GenerateArguments();

            Assert.Equal(left, md.LeftBound);
            Assert.Equal(right, md.RightBound);
            Assert.Equal(cnt, md.PointsCount);
            Assert.Equal(cnt, md.GridArguments.Length);
            Assert.Equal(cnt - 10, md.CalculatedValues.Length);
            md.CalculateValues();
            Assert.Equal(cnt, md.CalculatedValues.Length);


            for (int i = 1; i < cnt; ++i)
            {
                Assert.True(md.GridArguments[i - 1] < md.GridArguments[i]);
            }
            Assert.Equal(left, md.GridArguments[0]);
            Assert.Equal(right, md.GridArguments[cnt - 1]);
        }

        [Fact]
        public void TestInvalidRangeForSqrt()
        {
            double left = -10;
            double right = 20;
            int cnt = 11;

            bool is_caught = false;
            try
            {
                var md = new MeasuredData(SPf.SQRT, cnt, left, right);
            }
            catch (Exception)
            {
                is_caught = true;
            }
            Assert.True(is_caught);
        }
    }

    public class SplineParametersTests
    {
        private readonly double EPS = 1e-9;

        [Fact]
        public void TestjConstructorUniformGrid()
        {
            double left = -10;
            double right = 20;
            int cnt = 11;

            var sp = new SplineParameters(cnt, left, right, 0, 0, 0, 0);

            Assert.Equal(cnt, sp.Range.Length);
            
            double step = (right - left) / (cnt - 1);
            for (int i = 1; i < cnt; ++i)
            {
                var diff = sp.Range[i] - sp.Range[i - 1];
                Assert.True(Math.Abs(diff - step) < EPS);
            }
            Assert.Equal(left, sp.Range[0]);
            Assert.Equal(right, sp.Range[cnt - 1]);
        }
    }

    public class SplinesDataTests
    {
        private readonly double EPS = 1e-3;

        [Fact]
        public void TestBuildSplinesMdOrSpIsNull()
        {
            var sd = new SplinesData(null, null);

            bool is_caught = false;
            try
            {
                sd.BuildSplines();
            } 
            catch (Exception)
            {
                is_caught = true;
            }
            Assert.True(is_caught);

            sd.Md = new MeasuredData(SPf.CUBIC_FUNC, 10, 0, 10);
            is_caught = false;
            try
            {
                sd.BuildSplines();
            }
            catch (Exception)
            {
                is_caught = true;
            }
            Assert.True(is_caught);

            sd.Md = null;
            sd.Sp = new SplineParameters(10, 0, 10, 0, 0, 0, 0);
            is_caught = false;
            try
            {
                sd.BuildSplines();
            }
            catch (Exception)
            {
                is_caught = true;
            }
            Assert.True(is_caught);
        }

        private static double CubicFunction(double x)
        {
            return 0.1 * x * x * x + 2 * x * x + 9 * x + 1;
        }

        [Fact]
        public void TestBuildSplines()
        {
            double left = -13;
            double right = 1;

            var sd = new SplinesData(
                new MeasuredData(SPf.CUBIC_FUNC, 5000, left, right),
                new SplineParameters(2000, left, right, -7.8, 4.6, 0, 0));

            sd.BuildSplines();

            for (int i = 0; i < sd.Sp.PointsCount; ++i)
            {
                Assert.True(Math.Abs(sd.FirstSplineValues[i] - CubicFunction(sd.Sp.Range[i])) < EPS);
            }
        }
    }
}
