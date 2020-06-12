#!/bin/bash -ex

config=Release
rid=linux-x64

host=W56B1HhBEq66IOfG3qCSv
app_path=web/roshart/app

dotnet publish -r "$rid" -c "$config"
ssh $host "rm -rf $app_path"
rsync -av "bin/$config/netcoreapp3.1/$rid/publish/" "$host:$app_path"