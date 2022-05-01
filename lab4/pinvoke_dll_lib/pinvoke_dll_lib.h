#pragma once

#ifdef __cplusplus
extern "C" {
#endif

    // enum VMf
    // vmsExp == 1
    // vmdExp == 2
    // vmsErf == 3
    // vmdErf == 4

    __declspec(dllexport) void CalculateFunction(
        const int function_code, const int points_count, const float* range,
        float& noMKLtime, float& haTime, float& epTime, float& maxAbsDiff,
        float& maxAbsDiffArg, float& maxAbsDiffHaValue, float& maxAbsDiffEpValue,
        int& err
    );

#ifdef __cplusplus
}
#endif