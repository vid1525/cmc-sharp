using System.ComponentModel;

namespace ViewModel
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
}
