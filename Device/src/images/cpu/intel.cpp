#include "intel.h"

#include <Arduino.h>

const unsigned char Images::Intel::CpuCore::Width PROGMEM = 88;
const unsigned char Images::Intel::CpuCore::Height PROGMEM = 82;
const unsigned char Images::Intel::CpuCore::Data[] PROGMEM =
{
    // '88x82 gpu2 CORE ONLY template INTEL', 88x82px

    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x3f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfc, 0x00, 0x00, 0x00, 0x00, 0x7f, 0xff, 0xff, 0xff,
    0xff, 0xff, 0xfe, 0x00, 0x00, 0x00, 0x00, 0xe0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00,
    0x00, 0x01, 0xdf, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x80, 0x00, 0x00, 0x03, 0xbf, 0xff, 0xff,
    0xff, 0xff, 0xff, 0xf7, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xe3, 0xc0,
    0x00, 0x00, 0x03, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xc1, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff,
    0xff, 0xff, 0xff, 0xff, 0xc1, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xe3,
    0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xf6, 0xc0, 0x00, 0x00, 0x03, 0x7f,
    0xff, 0xff, 0xff, 0xff, 0xff, 0xfe, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff, 0xff, 0xe0, 0x00, 0x1f,
    0xfe, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff, 0xfe, 0x01, 0xfc, 0x03, 0xfe, 0xc0, 0x00, 0x00, 0x03,
    0x7f, 0xff, 0xf0, 0x7f, 0xff, 0xe0, 0xfe, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff, 0xc7, 0xff, 0xff,
    0xfc, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff, 0x1f, 0xff, 0xff, 0xfe, 0x1e, 0xc0, 0x00, 0x00,
    0x03, 0x7f, 0xfc, 0xff, 0xff, 0xff, 0xff, 0x0e, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff, 0xff, 0xff,
    0xff, 0xff, 0x8e, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xc6, 0xc0, 0x00,
    0x00, 0x03, 0x7f, 0xc7, 0xff, 0xff, 0xff, 0x8f, 0xc6, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xc7, 0xff,
    0x8f, 0xff, 0x8f, 0xe6, 0xc0, 0x00, 0x00, 0x03, 0x7e, 0xc7, 0xff, 0x8f, 0xff, 0x8f, 0xe6, 0xc0,
    0x00, 0x00, 0x03, 0x7c, 0xff, 0xff, 0x8f, 0xff, 0x8f, 0xe6, 0xc0, 0x00, 0x00, 0x03, 0x79, 0xc6,
    0x03, 0x83, 0x87, 0x8f, 0xe6, 0xc0, 0x00, 0x00, 0x03, 0x7b, 0xc6, 0x01, 0x83, 0x01, 0x8f, 0xe6,
    0xc0, 0x00, 0x00, 0x03, 0x73, 0xc6, 0x71, 0x8e, 0x31, 0x8f, 0xc6, 0xc0, 0x00, 0x00, 0x03, 0x77,
    0xc6, 0x79, 0x8e, 0x78, 0x8f, 0xc6, 0xc0, 0x00, 0x00, 0x03, 0x67, 0xc6, 0x79, 0x8e, 0x78, 0x8f,
    0x8e, 0xc0, 0x00, 0x00, 0x03, 0x67, 0xc6, 0x79, 0x8e, 0x00, 0x8f, 0x8e, 0xc0, 0x00, 0x00, 0x03,
    0x67, 0xc6, 0x79, 0x8e, 0x7f, 0x8f, 0x1e, 0xc0, 0x00, 0x00, 0x03, 0x6f, 0xc6, 0x79, 0x8e, 0x7f,
    0x8e, 0x1e, 0xc0, 0x00, 0x00, 0x03, 0x4f, 0xc6, 0x79, 0x8e, 0x39, 0x8c, 0x3e, 0xc0, 0x00, 0x00,
    0x03, 0x47, 0xc6, 0x79, 0xc3, 0x01, 0xcc, 0xfe, 0xc0, 0x00, 0x00, 0x03, 0x67, 0xe6, 0x79, 0xe3,
    0x83, 0xed, 0xfe, 0xc0, 0x00, 0x00, 0x03, 0x67, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfe, 0xc0, 0x00,
    0x00, 0x03, 0x67, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfe, 0xc0, 0x00, 0x00, 0x03, 0x63, 0xff, 0xff,
    0xff, 0xff, 0xf7, 0xfe, 0xc0, 0x00, 0x00, 0x03, 0x71, 0xff, 0xff, 0xff, 0xff, 0xc7, 0xfe, 0xc0,
    0x00, 0x00, 0x03, 0x70, 0xff, 0xff, 0xff, 0xff, 0x07, 0xfe, 0xc0, 0x00, 0x00, 0x03, 0x78, 0x7f,
    0xff, 0xff, 0xf8, 0x07, 0xfe, 0xc0, 0x00, 0x00, 0x03, 0x7c, 0x0f, 0xff, 0xff, 0xc0, 0x0f, 0xfe,
    0xc0, 0x00, 0x00, 0x03, 0x7e, 0x01, 0xff, 0xf8, 0x00, 0x3f, 0xfe, 0xc0, 0x00, 0x00, 0x03, 0x7f,
    0x80, 0x00, 0x00, 0x01, 0xff, 0xfe, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xe0, 0x00, 0x00, 0x0f, 0xff,
    0xfe, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xfc, 0x00, 0x00, 0xff, 0xff, 0xfe, 0xc0, 0x00, 0x00, 0x03,
    0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfe, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff, 0xff, 0xff, 0xff,
    0xff, 0xfe, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfe, 0xc0, 0x00, 0x00,
    0x03, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfe, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff, 0xff, 0xff,
    0xff, 0xff, 0xfe, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfe, 0xc0, 0x00,
    0x00, 0x03, 0xbf, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfd, 0xc0, 0x00, 0x00, 0x01, 0xdf, 0xff, 0xff,
    0xff, 0xff, 0xff, 0xfb, 0x80, 0x00, 0x00, 0x00, 0xe0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0x00,
    0x00, 0x00, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x00, 0x00, 0x00, 0x00, 0x3f, 0xff,
    0xff, 0xff, 0xff, 0xff, 0xfc, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00
};
