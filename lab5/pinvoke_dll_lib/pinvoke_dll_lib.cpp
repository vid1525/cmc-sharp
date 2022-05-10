#include "pch.h"
#include "pinvoke_dll_lib.h"
#include <mkl.h>

#include <iostream>
#include <cmath>

#ifdef __cplusplus
extern "C" {
#endif

    namespace {

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

    }

    __declspec(dllexport) int CalculateSplines(
        const int non_uniform_points_count, const double* x, const double* y,
        const double second_der_left, const double second_der_right, 
        const int uniform_points_count, double* res
    ) {
        DFTaskPtr desc;

        int status = dfdNewTask1D(&desc, non_uniform_points_count, x, DF_NON_UNIFORM_PARTITION, 1, y, DF_NO_HINT);
        if (status != DF_STATUS_OK)
        {
            return status;
        }

        double second_der_range_vals[] = { second_der_left, second_der_right };
        auto memHolder1 = DoubleMemoryHolder((non_uniform_points_count - 1) * DF_PP_CUBIC);
        auto scoeff = memHolder1.get();
        status = dfdEditPPSpline1D(
            desc, DF_PP_CUBIC, DF_PP_NATURAL, DF_BC_2ND_LEFT_DER | DF_BC_2ND_RIGHT_DER,
            second_der_range_vals, DF_NO_IC, NULL, scoeff, DF_NO_HINT);
        if (status != DF_STATUS_OK)
        {
            return status;
        }

        status = dfdConstruct1D(desc, DF_PP_SPLINE, DF_METHOD_STD);
        if (status != DF_STATUS_OK)
        {
            return status;
        }

        MKL_INT dorder[] = { 1 };
        double range[] = { x[0], x[non_uniform_points_count - 1] };
        status = dfdInterpolate1D(desc, DF_INTERP, DF_METHOD_PP, uniform_points_count, range, DF_UNIFORM_PARTITION, 1, dorder, NULL, res, DF_NO_HINT, NULL);
        if (status != DF_STATUS_OK)
        {
            return status;
        }

        if (status != DF_STATUS_OK)
        {
            return status;
        }

        return DF_STATUS_OK;
    }

#ifdef __cplusplus
}
#endif