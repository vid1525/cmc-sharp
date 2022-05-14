#pragma once

#ifdef __cplusplus
extern "C" {
#endif

    __declspec(dllexport) int CalculateSplines(
        const int non_uniform_points_count, const double* x, const double* y,
        const double second_der_left, const double second_der_right,
        const int uniform_points_count, double* res
    );

#ifdef __cplusplus
}
#endif