using System;
using Xunit;
using ViewModel;

namespace ViewModel.Tests
{
    public class ConvertersTests
    {
        [Fact]
        public void TestStringToDoubleConverter()
        {
            var conv = new StringToDoubleConverter();
            var res = conv.Convert(0.123, null, null, null);
            Assert.Equal("0,123", res);

            var number_res = conv.ConvertBack("0,123", null, null, null);
            Assert.Equal(0.123, number_res);
        }

        [Fact]
        public void TestStringToIntConverter()
        {
            var conv = new StringToIntConverter();
            var res = conv.Convert(123, null, null, null);
            Assert.Equal("123", res);

            var number_res = conv.ConvertBack("123", null, null, null);
            Assert.Equal(123, number_res);
        }
    }

    public class ViewDataTests
    {
        [Fact]
        public void TestUpdateMdAndMdIsNull()
        {
            var viewData = new ViewData(null, null);

            viewData.Input = new InputChecker(Splines.SPf.CUBIC_FUNC, 5, 10, 0, 10, 0, 0, 0, 0);

            viewData.UpdateMeasuredData();

            Assert.Equal(5, viewData.MeasuredDataValues.Count);
            Assert.Empty(viewData.SplinesDataValues);
            Assert.Empty(viewData.ChData.Series);
        }

        [Fact]
        public void TestUpdateSpAndMdIsNull()
        {
            var viewData = new ViewData(null, null);

            viewData.Input = new InputChecker(Splines.SPf.CUBIC_FUNC, 5, 10, 0, 10, 0, 0, 0, 0);

            bool is_caught = false;
            try
            {
                viewData.UpdateSplineData();
            }
            catch (Exception)
            {
                is_caught = true;
            }
            Assert.True(is_caught);
        }

        [Fact]
        public void TestUpdateSpOk()
        {
            var viewData = new ViewData(null, null);

            viewData.Input = new InputChecker(Splines.SPf.CUBIC_FUNC, 5, 10, 0, 10, 0, 0, 0, 0);

            viewData.UpdateMeasuredData();

            Assert.Equal(5, viewData.MeasuredDataValues.Count);
            Assert.Empty(viewData.SplinesDataValues);
            Assert.Empty(viewData.ChData.Series);

            viewData.UpdateSplineData();
            Assert.Equal(8, viewData.SplinesDataValues.Count);
            Assert.Empty(viewData.ChData.Series);
        }

        [Fact]
        public void TestDoubleMdUpdate()
        {
            var viewData = new ViewData(null, null);

            viewData.Input = new InputChecker(Splines.SPf.CUBIC_FUNC, 5, 10, 0, 10, 0, 0, 0, 0);

            viewData.UpdateMeasuredData();

            Assert.Equal(5, viewData.MeasuredDataValues.Count);
            Assert.Empty(viewData.SplinesDataValues);
            Assert.Empty(viewData.ChData.Series);

            viewData.Input = new InputChecker(Splines.SPf.CUBIC_FUNC, 15, 12, 0, 10, 0, 0, 0, 0);

            viewData.UpdateMeasuredData();

            Assert.Equal(15, viewData.MeasuredDataValues.Count);
            Assert.Empty(viewData.SplinesDataValues);
            Assert.Empty(viewData.ChData.Series);
        }

        [Fact]
        public void TestDoubleMdAndSpUpdate()
        {
            // first
            var viewData = new ViewData(null, null);

            viewData.Input = new InputChecker(Splines.SPf.CUBIC_FUNC, 5, 10, 0, 10, 0, 0, 0, 0);

            viewData.UpdateMeasuredData();

            Assert.Equal(5, viewData.MeasuredDataValues.Count);
            Assert.Empty(viewData.SplinesDataValues);
            Assert.Empty(viewData.ChData.Series);

            viewData.UpdateSplineData();

            Assert.Equal(8, viewData.SplinesDataValues.Count);
            Assert.Empty(viewData.ChData.Series);
            
            // second
            viewData.Input = new InputChecker(Splines.SPf.CUBIC_FUNC, 15, 12, 0, 10, 0, 0, 0, 0);

            viewData.UpdateMeasuredData();

            Assert.Equal(15, viewData.MeasuredDataValues.Count);
            Assert.Equal(8, viewData.SplinesDataValues.Count);
            Assert.Empty(viewData.ChData.Series);

            viewData.UpdateSplineData();
            Assert.Equal(8, viewData.SplinesDataValues.Count);
            Assert.Empty(viewData.ChData.Series);
        }
    }
}
