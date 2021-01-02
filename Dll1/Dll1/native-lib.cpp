#include "pch.h"
#include "native-lib.h"
#include <string>

extern "C" {
    __declspec(dllexport) float __stdcall add(float x, float y)
    {
        return x + y;
    }
}
