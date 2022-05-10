using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace calculations
{
    public enum VMf
    {
        VMS_EXP = 1,
        VMD_EXP,
        VMS_ERF,
        VMD_ERF
    }

    public class VMGrid
    {
        public int ArgsVectorLength { get; set; }
        public float[] GridRange { get; set; }
        public float GridStep { get; }
        public VMf VMfParameter { get; set; }

        public VMGrid(int argsVectorLength, float leftBound, float rightBound, VMf vmfFunction)
        {
            ArgsVectorLength = argsVectorLength;
            GridRange = new float[] { leftBound, rightBound };
            GridStep = 0;
            if (argsVectorLength > 1)
            {
                GridStep = (rightBound - leftBound) / (ArgsVectorLength - 1);
            }
            VMfParameter = vmfFunction;
        }

        public override string ToString()
        {
            return $"Points count: {ArgsVectorLength}\n" +
                $"Range: [{GridRange[0]}, {GridRange[1]}]\n" +
                $"Grid step: {GridStep}\n" +
                $"Function: {VMfParameter}\n";
        }
    }

    public struct VMTime
    {
        public VMGrid GridParams { get; set; }
        public float VmlHaExecTime { get; set; }
        public float VmlEpExecTime { get; set; }
        public float WithoutMklExecTime { get; set; }
        public float CoefOfExecVmlHa { get; set; }
        public float CoefOfExecVmlEp { get; set; }
        public override string ToString()
        {
            return $"GridParams:\n{GridParams}\n" +
                $"VML_HA exec time: {VmlHaExecTime}\n" +
                $"VML_EP exec time: {VmlEpExecTime}\n" +
                $"Without MKL exec time: {WithoutMklExecTime}\n" +
                $"MKL optimization coefs:\n\t- VML_HA: {CoefOfExecVmlHa}\n\t- VML_EP: {CoefOfExecVmlEp}\n";
        }
    }

    public struct VMAccuracy
    {
        public VMGrid GridParams { get; set; }
        public float MaxAbsDiffValue { get; set; }
        public float MaxAbsDiffArgument { get; set; }
        public float MaxAbsDiffValueVmlHa { get; set; }
        public float MaxAbsDiffValueVmlEp { get; set; }

        public override string ToString()
        {
            return $"GridParams:\n{GridParams}\n" +
                $"Max abs diff between VML_HA and VML_EP: {MaxAbsDiffValue}\n" +
                $"Max abs diff argument: {MaxAbsDiffArgument}\n" +
                $"Values by max abs diff argument: VML_HA - {MaxAbsDiffValueVmlHa}; VML_EP - {MaxAbsDiffValueVmlEp}\n";
        }
    }

    public class VMBenchmark
    {
        private static string HandleException(int status)
        {
            switch (status)
            {
                case 1:
                    return "The function does not support the present accuracy mode." +
                        "The Low Accuracy mode is used instead";
                case 2:
                    return "NULL pointer is passed";
                case 3:
                    return "At least one of array values is out of a range of definition";
                case 4:
                    return "At least one of array values causes a divide-by-zero exception" +
                        "or produces an invalid (QNan) result";
                case 5:
                    return "An overflow has happened during the calculation process";
                case 6:
                    return "An underflow has happened during the calculation process";
                case 7:
                    return "The execution was completed successfully in a different accuracy mode";
                default:
                    return "Unknown exception";
            }
        }

        public ObservableCollection<VMTime> TimeResults { get; set; } = new ObservableCollection<VMTime>();
        public ObservableCollection<VMAccuracy> AccuracyResults { get; set; } = new ObservableCollection<VMAccuracy>();
        public float MinCoefOfExecVmlHa { get; set; }
        public float MinCoefOfExecVmlEp { get; set; }

        public void AddVMTime(VMGrid vmGrid)
        {
            var vmTime = new VMTime();
            vmTime.GridParams = vmGrid;

            float withoutMklTime = 1;
            float haTime = 1;
            float epTime = 1;
            float absDiff = 0;
            float absDiffArg = 0;
            float absDiffHaValue = 0;
            float absDiffEpValue = 0;
            int err = 0;
            CalculateFunction(
                (int)vmGrid.VMfParameter, vmGrid.ArgsVectorLength, vmGrid.GridRange,
                ref withoutMklTime, ref haTime, ref epTime, ref absDiff, ref absDiffArg,
                ref absDiffHaValue, ref absDiffEpValue, ref err
            );
            if (err != 0)
            {
                throw new Exception(HandleException(err));
            }
            vmTime.VmlHaExecTime = haTime;
            vmTime.VmlEpExecTime = epTime;
            vmTime.WithoutMklExecTime = withoutMklTime;
            if (withoutMklTime > 0)
            {
                vmTime.CoefOfExecVmlHa = haTime / withoutMklTime;
                vmTime.CoefOfExecVmlEp = epTime / withoutMklTime;
            }
            else
            {
                vmTime.CoefOfExecVmlHa = 1;
                vmTime.CoefOfExecVmlEp = 1;
            }
            
            TimeResults.Add(vmTime);

            if (TimeResults.Count == 1)
            {
                MinCoefOfExecVmlHa = vmTime.CoefOfExecVmlHa;
                MinCoefOfExecVmlEp = vmTime.CoefOfExecVmlEp;
            }
            else
            {
                MinCoefOfExecVmlHa = Math.Min(MinCoefOfExecVmlHa, vmTime.CoefOfExecVmlHa);
                MinCoefOfExecVmlEp = Math.Min(MinCoefOfExecVmlEp, vmTime.CoefOfExecVmlEp);
            }
        }

        public void AddVMAccuracy(VMGrid vmGrid)
        {
            var vmAccuracy = new VMAccuracy();
            vmAccuracy.GridParams = vmGrid;

            float withoutMklTime = 1;
            float haTime = 1;
            float epTime = 1;
            float absDiff = 0;
            float absDiffArg = 0;
            float absDiffHaValue = 0;
            float absDiffEpValue = 0;
            int err = 0;
            CalculateFunction(
                (int)vmGrid.VMfParameter, vmGrid.ArgsVectorLength, vmGrid.GridRange,
                ref withoutMklTime, ref haTime, ref epTime, ref absDiff, ref absDiffArg,
                ref absDiffHaValue, ref absDiffEpValue, ref err
            );
            if (err != 0)
            {
                throw new Exception(HandleException(err));
            }
            vmAccuracy.MaxAbsDiffValue = absDiff;
            vmAccuracy.MaxAbsDiffArgument = absDiffArg;
            vmAccuracy.MaxAbsDiffValueVmlHa = absDiffHaValue;
            vmAccuracy.MaxAbsDiffValueVmlEp = absDiffEpValue;
            AccuracyResults.Add(vmAccuracy);
        }

        [DllImport("..\\..\\..\\..\\lab4\\x64\\Debug\\pinvoke_dll_lib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CalculateFunction(
            int function_code, int points_count, float[] range, ref float noMKLtime,
            ref float haTime, ref float epTime, ref float maxAbsDiff, ref float maxAbsDiffArg,
            ref float maxAbsDiffHaValue, ref float maxAbsDiffEpValue, ref int err
        );

    }
}
