#!/bin/bash

pushd () {
    command pushd "$@" > /dev/null
}

popd () {
    command popd "$@" > /dev/null
}

function rarFolder {
    rm "$1.rar"
    rar a -m5 "$1.rar" "$1"
}

for d in *; do
    if [ -d "$d" ]; then
        rarFolder "$d"
    fi
done