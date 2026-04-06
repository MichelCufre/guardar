set environment=%1
set msbuild=C:\Program Files\Microsoft Visual Studio\2022\Enterprise\
set vs_version=17.0
set net_core_framework_version=net8.0
set net_framework_version=v4.8

if not exist "%msbuild%" (
  set msbuild=C:\Program Files\Microsoft Visual Studio\2022\Community\
) 

set msbuild="%msbuild%Msbuild\Current\Bin\MSBuild.exe"

:: WIS.TRAFFIC.OFFICER

%msbuild% "..\trafficofficer\1.FrontEnd\WIS.TrafficOfficerApp\WIS.TrafficOfficerApp.csproj" -p:Configuration=Release -p:OutputPath="..\..\..\..\Publish\Sites\WIS.TrafficOfficerApp" -target:Rebuild;Publish -p:VisualStudioVersion=%vs_version% -p:DeleteExistingFiles=true -p:Platform=x64 -p:TargetFrameworkVersion=%net_framework_version%

del "..\..\Publish\Sites\WIS.TrafficOfficerApp\*" /q

xcopy /s "..\..\Publish\Sites\WIS.TrafficOfficerApp\_PublishedWebsites\WIS.TrafficOfficerApp" "..\..\Publish\Sites\WIS.TrafficOfficerApp" /y

rmdir "..\..\Publish\Sites\WIS.TrafficOfficerApp\_PublishedWebsites" /s /q

dotnet publish "..\trafficofficer\2.BackEnd\1.Services\WIS.TrafficOfficer\WIS.TrafficOfficer.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Sites\WIS.TrafficOfficer" --sc false -c Release

dotnet publish "..\trafficofficer\2.BackEnd\1.Services\Jobs\WIS.TimeOutBloqueos\WIS.TimeOutBloqueos.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Jobs\WIS.TimeOutBloqueos" --sc false -c Release

:: WisColectores

%msbuild% "..\WisColectores\1.Services\WIS.InternalMobileServices\WIS.InternalMobileServices.csproj" -p:Configuration=Release -p:OutputPath="..\..\..\..\Publish\Sites\WIS.InternalMobileServices" -target:Rebuild;Publish -p:VisualStudioVersion=%vs_version% -p:DeleteExistingFiles=true -p:Platform=x64 -p:TargetFrameworkVersion=%net_framework_version% 

del "..\..\Publish\Sites\WIS.InternalMobileServices\*" /q

xcopy /s "..\..\Publish\Sites\WIS.InternalMobileServices\_PublishedWebsites\WIS.InternalMobileServices" "..\..\Publish\Sites\WIS.InternalMobileServices" /y

rmdir "..\..\Publish\Sites\WIS.InternalMobileServices\_PublishedWebsites" /s /q

%msbuild% "..\WisColectores\0.Presentation\WIS.WebMobileApplication\WIS.WebMobileApplication.csproj" -p:Configuration=Release -p:OutputPath="..\..\..\..\Publish\Sites\WIS.WebMobileApplication" -target:Rebuild;Publish -p:VisualStudioVersion=%vs_version% -p:DeleteExistingFiles=true -p:Platform=x64 -p:TargetFrameworkVersion=%net_framework_version% 

del "..\..\Publish\Sites\WIS.WebMobileApplication\*" /q

xcopy /s "..\..\Publish\Sites\WIS.WebMobileApplication\_PublishedWebsites\WIS.WebMobileApplication" "..\..\Publish\Sites\WIS.WebMobileApplication" /y

rmdir "..\..\Publish\Sites\WIS.WebMobileApplication\_PublishedWebsites" /s /q

%msbuild% "..\WisColectores\1.Services\Jobs\WIS.LocalizationImportProcess\WIS.LocalizationImportProcess.csproj" -p:Configuration=Release -p:OutputPath="..\..\..\..\Publish\Jobs\WIS.LocalizationImportProcessColectores" -target:Rebuild -p:VisualStudioVersion=%vs_version% -p:DeleteExistingFiles=true -p:Platform=x64 -p:TargetFrameworkVersion=%net_framework_version% 

robocopy "C:\WIS\l10n" "..\..\Publish\l10n" /e

:: WISBase40

dotnet publish "WIS.APIGateway\WIS.APIGateway.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Sites\WIS.APIGateway" --sc false -c Release

dotnet publish "WIS.AutomationGateway\WIS.AutomationGateway.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Sites\WIS.AutomationGateway" --sc false -c Release

dotnet publish "WISBase40\WIS.WebApplication.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Sites\WIS.WebApplication" --sc false -c Release

dotnet publish "WIS.AttributeValidator\WIS.AttributeValidator.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Sites\WIS.AttributeValidator" --sc false -c Release

dotnet publish "WIS.AutomationInterpreter\WIS.AutomationInterpreter.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Sites\WIS.AutomationInterpreter" --sc false -c Release

dotnet publish "WIS.AutomationManager\WIS.AutomationManager.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Sites\WIS.AutomationManager" --sc false -c Release

dotnet publish "WIS.BackendService\WIS.BackendService.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Sites\WIS.BackendService" --sc false -c Release

dotnet publish "WIS.GalysServices\WIS.GalysServices.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Sites\WIS.GalysServices" --sc false -c Release

dotnet publish "WIS.MultidataCodeConverter\WIS.MultidataCodeConverter.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Sites\WIS.MultidataCodeConverter" --sc false -c Release

dotnet publish "WIS.TaskQueue\WIS.TaskQueue.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Sites\WIS.TaskQueue" --sc false -c Release

dotnet publish "WIS.WebhookClient\WIS.WebhookClient.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Sites\WIS.WebhookClient" --sc false -c Release

dotnet publish "WIS.WMS_API\WIS.WMS_API.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Sites\WIS.WMS_API" --sc false -c Release

dotnet publish "WIS.WMSTrackingAPI\WIS.WMSTrackingAPI.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Sites\WIS.WMSTrackingAPI" --sc false -c Release

dotnet publish "WIS.AjusteStockDocumentalProcess\WIS.AjusteStockDocumentalProcess.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Jobs\WIS.AjusteStockDocumentalProcess" --sc false -c Release

dotnet publish "WIS.AnulacionDocumentalProcess\WIS.AnulacionDocumentalProcess.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Jobs\WIS.AnulacionDocumentalProcess" --sc false -c Release

dotnet publish "WIS.AnulacionPreparacionProcess\WIS.AnulacionPreparacionProcess.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Jobs\WIS.AnulacionPreparacionProcess" --sc false -c Release

dotnet publish "WIS.CierreCamionAuto\WIS.CierreCamionAuto.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Jobs\WIS.CierreCamionAuto" --sc false -c Release

dotnet publish "WIS.ClearLog\WIS.ClearLog.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Jobs\WIS.ClearLog" --sc false -c Release

dotnet publish "WIS.EjecucionesFacturacionProcess\WIS.EjecucionesFacturacionProcess.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Jobs\WIS.EjecucionesFacturacionProcess" --sc false -c Release

dotnet publish "WIS.LiberacionAutomaticaProcess\WIS.LiberacionAutomaticaProcess.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Jobs\WIS.LiberacionAutomaticaProcess" --sc false -c Release

dotnet publish "WIS.LocalizationImportProcess\WIS.LocalizationImportProcess.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Jobs\WIS.LocalizationImportProcess" --sc false -c Release

dotnet publish "WIS.NotificationProcess\WIS.NotificationProcess.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Jobs\WIS.NotificationProcess" --sc false -c Release

dotnet publish "WIS.ReportProcessor\WIS.ReportProcessor.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Jobs\WIS.ReportProcessor" --sc false -c Release

dotnet publish "WIS.SincronizarAutomatismoProcess\WIS.SincronizarAutomatismoProcess.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Jobs\WIS.SincronizarAutomatismoProcess" --sc false -c Release

dotnet publish "WIS.SincronizarTrackingProcess\WIS.SincronizarTrackingProcess.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Jobs\WIS.SincronizarTrackingProcess" --sc false -c Release

dotnet publish "WIS.TicketPrintingProcess\WIS.TicketPrintingProcess.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Jobs\WIS.TicketPrintingProcess" --sc false -c Release

:: WIS.AuthorizationServer

dotnet publish "..\authorizationservice\WIS.AuthorizationServer\WIS.AuthorizationServer.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Sites\WIS.AuthorizationServer" --sc false -c Release

dotnet publish "..\authorizationservice\WIS.AuthorizationAPI\WIS.AuthorizationAPI.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Sites\WIS.AuthorizationAPI" --sc false -c Release

:: PrintingServerFabrica

dotnet publish "..\printingservice\PrintingServerFabrica\PrintingServerFabrica.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Sites\PrintingServerFabrica" --sc false -c Release

dotnet publish "..\printingservice\PrintingClientFabrica\PrintingClientFabrica.csproj" -r win-x64 -f %net_core_framework_version% -o "..\..\Publish\Jobs\PrintingClientFabrica" --sc false -c Release

::

rmdir "..\..\..\Publish" /s /q

:: 

for %%n in (appsettings.json web.config nlog.config ocelot.json) do (
	for /f "tokens=1,2 delims=." %%a in ("%%n") do ( 
		if "%environment%" == "" (
			for /r "..\..\Publish" %%f in (%%a.*.%%b) do (
				del %%f
			)
		) else (
			for /r "..\..\Publish" %%f in (%%a.%%b) do (
				if exist %%f (
					del %%f
				)
			)
			for /r "..\..\Publish" %%f in (%%a.*.%%b) do (
				if /i "%%~nxf" == "%%a.%environment%.%%b" (
					ren "%%f" "%%a.%%b"
				) else (			
					del %%f
				)
			)
		) 
	)
)