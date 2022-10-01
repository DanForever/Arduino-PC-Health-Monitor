#!/bin/bash

version_input=$1

echo input=$version_input

if [ -z "${version_input}" ]
then
	version_input="0.0.1"
fi

echo output=$version_input

echo "::set-output name=version::$(echo $version_input)"