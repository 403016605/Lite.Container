@echo off

if not exist packages (
    md packages
)

for /R "packages" %%s in (*) do (
    del %%s
)

dotnet pack src/AspectCore.Lite.Container.Autofac --configuration Release --output packages
dotnet pack src/AspectCore.Lite.Container.DependencyInjection --configuration Release --output packages

set /p key=input key:
set source=http://servicepackages.chinacloudsites.cn/nuget

for /R "packages" %%s in (*symbols.nupkg) do ( 
    call nuget push %%s -s %source% %key%
)

pause
