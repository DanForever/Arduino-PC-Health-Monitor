#include "amd.h"

#include <Arduino.h>

const unsigned char Images::Amd::CpuCore::Width PROGMEM = 88;
const unsigned char Images::Amd::CpuCore::Height PROGMEM = 82;
const unsigned char Images::Amd::CpuCore::Data[] PROGMEM =
{
    // '88x82 AMD CORE ONLY template INTEL', 88x82px

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
    0xff, 0xff, 0xff, 0xff, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xf3, 0xc0,
    0x00, 0x00, 0x03, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xe1, 0xc0, 0x00, 0x00, 0x03, 0x7e, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x33, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3f,
    0xc0, 0x00, 0x00, 0x03, 0x7f, 0x80, 0x00, 0x00, 0x00, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7f,
    0xc0, 0x00, 0x00, 0x00, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xe0, 0x00, 0x00, 0x00, 0x00,
    0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03,
    0x7f, 0xf8, 0x00, 0x00, 0x00, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xfc, 0x00, 0x00, 0x00,
    0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xfe, 0x00, 0x00, 0x00, 0x00, 0x3e, 0xc0, 0x00, 0x00,
    0x03, 0x7f, 0xff, 0x00, 0x00, 0x00, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff, 0x80, 0x00,
    0x00, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff, 0xc0, 0x00, 0x00, 0x00, 0x3e, 0xc0, 0x00,
    0x00, 0x03, 0x7f, 0xff, 0xff, 0xff, 0xfc, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff, 0xff,
    0xff, 0xfc, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff, 0xff, 0xff, 0xfc, 0x00, 0x3e, 0xc0,
    0x00, 0x00, 0x03, 0x7f, 0xff, 0xbf, 0xff, 0xfc, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xff,
    0x3f, 0xff, 0xfc, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xfe, 0x3f, 0xff, 0xfc, 0x00, 0x3e,
    0xc0, 0x00, 0x00, 0x03, 0x7f, 0xfc, 0x3f, 0xff, 0xfc, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7f,
    0xf8, 0x3f, 0xff, 0xfc, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xf0, 0x3f, 0xff, 0xfc, 0x00,
    0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0xe0, 0x3f, 0xff, 0xfc, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03,
    0x7f, 0xc0, 0x3f, 0xff, 0xfc, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0x80, 0x3f, 0xff, 0xfc,
    0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7f, 0x00, 0x3f, 0xff, 0xfc, 0x00, 0x3e, 0xc0, 0x00, 0x00,
    0x03, 0x7e, 0x00, 0x3f, 0xff, 0xfc, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7c, 0x00, 0x3f, 0xff,
    0xfc, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7c, 0x00, 0x3f, 0xff, 0xfc, 0x00, 0x3e, 0xc0, 0x00,
    0x00, 0x03, 0x7c, 0x00, 0x3f, 0xff, 0xfc, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7c, 0x00, 0x3f,
    0xff, 0xfc, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7c, 0x00, 0x3f, 0xff, 0xfc, 0x00, 0x3e, 0xc0,
    0x00, 0x00, 0x03, 0x7c, 0x00, 0x00, 0x00, 0x1e, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7c, 0x00,
    0x00, 0x00, 0x3f, 0x00, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7c, 0x00, 0x00, 0x00, 0x7f, 0x80, 0x3e,
    0xc0, 0x00, 0x00, 0x03, 0x7c, 0x00, 0x00, 0x00, 0xff, 0xc0, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7c,
    0x00, 0x00, 0x01, 0xff, 0xe0, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7c, 0x00, 0x00, 0x03, 0xff, 0xf0,
    0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7c, 0x00, 0x00, 0x07, 0xff, 0xf8, 0x3e, 0xc0, 0x00, 0x00, 0x03,
    0x7c, 0x00, 0x00, 0x0f, 0xff, 0xfc, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7c, 0x00, 0x00, 0x1f, 0xff,
    0xfe, 0x3e, 0xc0, 0x00, 0x00, 0x03, 0x7c, 0x00, 0x00, 0x3f, 0xff, 0xff, 0x3e, 0xc0, 0x00, 0x00,
    0x03, 0x7c, 0x00, 0x00, 0x7f, 0xff, 0xff, 0xbe, 0xc0, 0x00, 0x00, 0x03, 0x7c, 0x00, 0x00, 0xff,
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
