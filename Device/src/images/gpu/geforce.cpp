#include "geforce.h"

#include <Arduino.h>

const unsigned char Images::Nvidia::Gpu::Width PROGMEM = 82;
const unsigned char Images::Nvidia::Gpu::Height PROGMEM = 82;
const unsigned char Images::Nvidia::Gpu::Data[] PROGMEM =
{
  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff,
  0xff, 0xff, 0xff, 0xf8, 0x00, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfc, 0x00,
  0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x00, 0x00, 0x7f, 0xff, 0xff, 0xff,
  0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
  0x80, 0x00, 0x7f, 0x75, 0xcb, 0x0d, 0xef, 0xff, 0xff, 0xff, 0xff, 0xc0, 0x00, 0x7f, 0x34, 0xdb,
  0x6d, 0xc7, 0xff, 0xff, 0xff, 0xff, 0xe0, 0x00, 0x7f, 0x36, 0xdb, 0x65, 0xd7, 0xff, 0xff, 0xff,
  0xff, 0xf0, 0x00, 0x7f, 0x16, 0x9b, 0x65, 0xd7, 0xff, 0xff, 0xff, 0xff, 0xf8, 0x00, 0x7f, 0x46,
  0x3b, 0x65, 0x93, 0xff, 0xff, 0xff, 0xff, 0xfc, 0x00, 0x7f, 0x67, 0x3b, 0x65, 0x83, 0xff, 0xff,
  0xff, 0xff, 0xfe, 0x00, 0x7f, 0x67, 0x3b, 0x0d, 0x3b, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x7f,
  0xf7, 0xfb, 0x3f, 0x79, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff,
  0xff, 0xff, 0xff, 0xff, 0x00, 0x7f, 0x80, 0x60, 0x30, 0x0c, 0x07, 0x01, 0xe0, 0x20, 0x1f, 0x00,
  0x7f, 0x00, 0x60, 0x30, 0x08, 0x03, 0x00, 0xc0, 0x20, 0x1f, 0x00, 0x7f, 0x00, 0x60, 0x30, 0x08,
  0x03, 0x00, 0xc0, 0x20, 0x1f, 0x00, 0x7f, 0x3f, 0xe7, 0xf3, 0xf9, 0xf3, 0x3c, 0xcf, 0xe7, 0xff,
  0x00, 0x7f, 0x3f, 0xe7, 0xf3, 0xf9, 0xf3, 0x3c, 0xcf, 0xe7, 0xff, 0x00, 0x7f, 0x30, 0x60, 0x70,
  0x19, 0xf3, 0x00, 0xcf, 0xe0, 0x3f, 0x00, 0x7f, 0x30, 0x60, 0x70, 0x19, 0xf3, 0x00, 0xcf, 0xe0,
  0x3f, 0x00, 0x7f, 0x3e, 0x67, 0xf3, 0xf9, 0xf3, 0x31, 0xcf, 0xe7, 0xff, 0x00, 0x7f, 0x3e, 0x67,
  0xf3, 0xf9, 0xf3, 0x39, 0xcf, 0xe7, 0xff, 0x00, 0x7f, 0x00, 0x60, 0x33, 0xf8, 0x03, 0x38, 0xc0,
  0x20, 0x1f, 0x00, 0x7f, 0x80, 0x60, 0x33, 0xf8, 0x03, 0x3c, 0xc0, 0x20, 0x1f, 0x00, 0x7f, 0xc0,
  0xe0, 0x33, 0xfc, 0x07, 0x3c, 0x60, 0x20, 0x1f, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
  0xff, 0xff, 0xff, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x7f,
  0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x00, 0x1f, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0,
  0x00, 0x00, 0x00, 0x1f, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x00, 0x1f, 0x00,
  0x7f, 0xff, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x00, 0x1f, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff,
  0xcf, 0x00, 0x00, 0x00, 0x1f, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xfe, 0x0f, 0xf0, 0x00, 0x00, 0x1f,
  0x00, 0x7f, 0xff, 0xff, 0xff, 0xf0, 0x0f, 0xfe, 0x00, 0x00, 0x1f, 0x00, 0x7f, 0xff, 0xff, 0xff,
  0xc0, 0x70, 0x3f, 0x00, 0x00, 0x1f, 0x00, 0x7f, 0xff, 0xff, 0xff, 0x03, 0xf0, 0x0f, 0xc0, 0x00,
  0x1f, 0x00, 0x7f, 0xff, 0xff, 0xfe, 0x0f, 0xf0, 0x03, 0xe0, 0x00, 0x1f, 0x00, 0x7f, 0xff, 0xff,
  0xf8, 0x1f, 0x8f, 0xc1, 0xf0, 0x00, 0x1f, 0x00, 0x7f, 0xff, 0xff, 0xf0, 0x7c, 0x0f, 0xe0, 0xfc,
  0x00, 0x1f, 0x00, 0x7f, 0xff, 0xff, 0xe0, 0xf8, 0x17, 0xf8, 0x7c, 0x00, 0x1f, 0x00, 0x7f, 0xff,
  0xff, 0xc0, 0xf0, 0xf1, 0xfc, 0x3e, 0x00, 0x1f, 0x00, 0x7f, 0xff, 0xff, 0xc1, 0xe1, 0xf0, 0xf8,
  0x3e, 0x00, 0x1f, 0x00, 0x7f, 0xff, 0xff, 0xe0, 0xe1, 0xf0, 0x70, 0x7c, 0x00, 0x1f, 0x00, 0x7f,
  0xff, 0xff, 0xe0, 0xe1, 0xf0, 0x60, 0xf8, 0x00, 0x1f, 0x00, 0x7f, 0xff, 0xff, 0xf0, 0xf0, 0xf0,
  0x01, 0xf0, 0x00, 0x1f, 0x00, 0x7f, 0xff, 0xff, 0xf0, 0x70, 0x70, 0x03, 0xc0, 0x60, 0x1f, 0x00,
  0x7f, 0xff, 0xff, 0xf8, 0x38, 0x70, 0x07, 0x81, 0xf8, 0x1f, 0x00, 0x3f, 0xff, 0xff, 0xfc, 0x3c,
  0x10, 0x1f, 0x03, 0xfe, 0x1f, 0x00, 0x1f, 0xff, 0xff, 0xfc, 0x1e, 0x00, 0x7c, 0x07, 0xfc, 0x1f,
  0x00, 0x0f, 0xff, 0xff, 0xfe, 0x0f, 0x8f, 0xf0, 0x1f, 0xf8, 0x1f, 0x00, 0x47, 0xff, 0xff, 0xff,
  0x07, 0xef, 0xc0, 0x3f, 0xe0, 0x1f, 0x00, 0x63, 0xff, 0xff, 0xff, 0x81, 0xf0, 0x00, 0xff, 0x80,
  0x1f, 0x00, 0x71, 0xff, 0xff, 0xff, 0xe0, 0x70, 0x03, 0xfe, 0x00, 0x1f, 0x00, 0x78, 0xff, 0xff,
  0xff, 0xf0, 0x00, 0x3f, 0xf0, 0x00, 0x1f, 0x00, 0x7c, 0x7f, 0xff, 0xff, 0xfc, 0x0f, 0xff, 0x80,
  0x00, 0x1f, 0x00, 0x7e, 0x3f, 0xff, 0xff, 0xff, 0x0f, 0xf8, 0x00, 0x00, 0x1f, 0x00, 0x7f, 0x1f,
  0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x00, 0x1f, 0x00, 0x7f, 0x8f, 0xff, 0xff, 0xff, 0xf0, 0x00,
  0x00, 0x00, 0x1f, 0x00, 0x7f, 0xc7, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x00, 0x1f, 0x00, 0x7f,
  0xe3, 0xff, 0xff, 0xff, 0xf0, 0x00, 0x00, 0x00, 0x1f, 0x00, 0x7f, 0xf1, 0xff, 0xff, 0xff, 0xff,
  0xff, 0xff, 0xff, 0xff, 0x00, 0x7f, 0xf8, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00,
  0x7f, 0xfc, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7f, 0xfe, 0x00, 0x00, 0x00,
  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7f, 0xfe, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
  0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x3f, 0xff, 0xff, 0xff,
  0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x1f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
  0xff, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x07, 0xff, 0xff,
  0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x03, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
  0xff, 0xff, 0x00, 0x01, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0xff,
  0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x7f, 0xff, 0xff, 0xff, 0xff, 0xff,
  0xff, 0xff, 0xff, 0x00, 0x00, 0x3f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x00,
  0x1f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x0f, 0xff, 0xff, 0xff, 0xff,
  0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x07, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00,
  0x00, 0x03, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
  0x00, 0x00, 0x00, 0x00, 0x00, 0x00
};