#pragma once

#define WIN32_LEAN_AND_MEAN             // Windows �w�b�_�[����قƂ�ǎg�p����Ă��Ȃ����������O����
// Windows �w�b�_�[ �t�@�C��
#include <windows.h>

extern "C" {
    __declspec(dllexport) float __stdcall add(float x, float y);
}