#include "pch.h"
#include "pinvoke_dll_lib.h"
#include <mkl.h>

#include <iostream>
#include <cmath>
#include <ctime>

#ifdef __cplusplus
extern "C" {
#endif

    // enum VMf
    // vmsExp == 1
    // vmdExp == 2
    // vmsErf == 3
    // vmdErf == 4

    namespace {

        class FloatMemoryHolder {
        public:
            FloatMemoryHolder(const int len) {
                mem = new float[len];
            }

            ~FloatMemoryHolder() {
                delete[] mem;
            }

            float* get() {
                return mem;
            }
        private:
            float* mem;
        };

        class DoubleMemoryHolder {
        public:
            DoubleMemoryHolder(const int len) {
                mem = new double[len];
            }

            ~DoubleMemoryHolder() {
                delete[] mem;
            }

            double* get() {
                return mem;
            }
        private:
            double* mem;
        };

        int handleError(const int status) {
            switch (status) {
            case VML_STATUS_OK:
                // status ok
                return 0;
            case VML_STATUS_BADSIZE:
                // The function does not support the present accuracy mode.
                // The Low Accuracy mode is used instead
                return 1;
            case VML_STATUS_BADMEM:
                // NULL pointer is passed
                return 2;
            case VML_STATUS_ERRDOM:
                // At least one of array values is out of a range of definition
                return 3;
            case VML_STATUS_SING:
                // At least one of array values causes a divide-by-zero exception
                // or produces an invalid (QNan) result
                return 4;
            case VML_STATUS_OVERFLOW:
                // An overflow has happened during the calculation process
                return 5;
            case VML_STATUS_UNDERFLOW:
                // An underflow has happened during the calculation process
                return 6;
            case VML_STATUS_ACCURACYWARNING:
                // The execution was completed successfully in a different accuracy mode
                return 7;
            default:
                return -1;
            }
        }

    }

    __declspec(dllexport) void CalculateFunction(
        const int function_code, const int points_count, const float* range,
        float& noMKLtime, float& haTime, float& epTime, float& maxAbsDiff,
        float& maxAbsDiffArg, float& maxAbsDiffHaValue, float& maxAbsDiffEpValue,
        int& err
    ) {
        vmlSetErrStatus(VML_STATUS_OK);
        if (function_code % 2) {
            auto floatMemHolder1 = FloatMemoryHolder(points_count);
            auto floatMemHolder2 = FloatMemoryHolder(points_count);
            auto floatMemHolder3 = FloatMemoryHolder(points_count);
            auto args = floatMemHolder1.get();
            auto result1 = floatMemHolder2.get();
            auto result2 = floatMemHolder3.get();

            float step = (range[1] - range[0]) / (points_count - 1);
            for (int i = 0; i < points_count; ++i) {
                args[i] = range[0] + step * i;
            }
            
            float (*no_mkl_function)(float);
            no_mkl_function = function_code == 1 ? (float(*)(float)) exp : (float(*)(float)) erf;

            double start = clock();
            for (int i = 0; i < points_count; ++i) {
                result1[i] = no_mkl_function(args[i]);
            }
            double finish = clock();
            noMKLtime = (float)(finish - start);

            void (*exec_function)(MKL_INT, const float*, float*, MKL_INT64);
            exec_function = function_code == 1 ? vmsExp : vmsErf;

            start = clock();
            exec_function(points_count, args, result2, VML_EP);
            finish = clock();
            epTime = (float)(finish - start);
            auto ret = handleError(vmlGetErrStatus());
            if (ret) {
                err = ret;
                return;
            }

            start = clock();
            exec_function(points_count, args, result1, VML_HA);
            finish = clock();
            haTime = (float)(finish - start);
            ret = handleError(vmlGetErrStatus());
            if (ret) {
                err = ret;
                return;
            }

            maxAbsDiff = fabs(result1[0] - result2[0]);
            maxAbsDiffArg = args[0];
            maxAbsDiffHaValue = result1[0];
            maxAbsDiffEpValue = result2[0];
            for (int i = 1; i < points_count; ++i) {
                float cur_diff = fabs(result1[i] - result2[i]);
                if (maxAbsDiff < cur_diff) {
                    maxAbsDiff = cur_diff;
                    maxAbsDiffArg = args[i];
                    maxAbsDiffHaValue = result1[i];
                    maxAbsDiffEpValue = result2[i];
                }
            }
        } else {
            auto doubleMemHolder1 = DoubleMemoryHolder(points_count);
            auto doubleMemHolder2 = DoubleMemoryHolder(points_count);
            auto doubleMemHolder3 = DoubleMemoryHolder(points_count);
            auto args = doubleMemHolder1.get();
            auto result1 = doubleMemHolder2.get();
            auto result2 = doubleMemHolder3.get();

            double step = (range[1] - range[0]) / (points_count - 1);
            for (int i = 0; i < points_count; ++i) {
                args[i] = range[0] + step * i;
            }

            double (*no_mkl_function)(double);
            no_mkl_function = function_code == 1 ? (double(*)(double)) exp : (double(*)(double)) erf;

            double start = clock();
            for (int i = 0; i < points_count; ++i) {
                result1[i] = no_mkl_function(args[i]);
            }
            double finish = clock();
            noMKLtime = (float)(finish - start);

            void (*exec_function)(MKL_INT, const double*, double*, MKL_INT64);
            exec_function = function_code == 2 ? vmdExp : vmdErf;

            start = clock();
            exec_function(points_count, args, result2, VML_EP);
            finish = clock();
            epTime = (float)(finish - start);
            auto ret = handleError(vmlGetErrStatus());
            if (ret) {
                err = ret;
                return;
            }

            start = clock();
            exec_function(points_count, args, result1, VML_HA);
            finish = clock();
            haTime = (float)(finish - start);
            ret = handleError(vmlGetErrStatus());
            if (ret) {
                err = ret;
                return;
            }

            maxAbsDiff = fabs(result1[0] - result2[0]);
            maxAbsDiffArg = args[0];
            maxAbsDiffHaValue = result1[0];
            maxAbsDiffEpValue = result2[0];
            for (int i = 1; i < points_count; ++i) {
                double cur_diff = fabs(result1[i] - result2[i]);
                if (maxAbsDiff < cur_diff) {
                    maxAbsDiff = cur_diff;
                    maxAbsDiffArg = args[i];
                    maxAbsDiffHaValue = result1[i];
                    maxAbsDiffEpValue = result2[i];
                }
            }
        }
    }

#ifdef __cplusplus
}
#endif