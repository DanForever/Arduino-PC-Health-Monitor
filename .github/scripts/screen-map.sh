#!/bin/bash

# A script by Dan Forever

declare -A screens

screens["IDENTITY_S_ILI9341"]="ili9341"
screens["IDENTITY_S_ILI9481"]="ili9481"
screens["IDENTITY_S_ILI9486"]="ili9486"
screens["IDENTITY_S_ILI9488"]="ili9488"

echo "::set-output name=screen-name::$(echo ${screens[$1]})"