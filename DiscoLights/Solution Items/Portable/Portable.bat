set packer=C:\Program Files\7-Zip\7z.exe
set package=DiscoLights
set build=Release

del /f /q %package%.7z

"%packer%" a "%package%.7z" "..\..\DiscoLights\Bin\%build%\DiscoLights.config"
"%packer%" a "%package%.7z" "..\..\DiscoLights\Bin\%build%\DiscoLights.exe"
"%packer%" a "%package%.7z" "..\..\DiscoLights\Bin\%build%\DiscoLights.Audio.dll"
"%packer%" a "%package%.7z" "..\..\DiscoLights\Bin\%build%\DiscoLights.Shared.dll"
"%packer%" a "%package%.7z" "..\..\DiscoLights\Bin\%build%\log4net.dll"
"%packer%" a "%package%.7z" "..\..\DiscoLights\Bin\%build%\NAudio.dll"

copy /b "%package%.sfx" + "%package%.ini" + "%package%.7z" "%package%Portable.exe"