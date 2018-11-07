# OWOR (OneWireOnRaspberry)

Owor allows you to connect your OneWire sensors to your Raspberry via the GPIO pins an process them in various ways

* WebUI
* MQTT
* HTTP

## Installation

*TODO: Provide a ready-made package*

For now, Owor has to be compiled and started by hand:

1. Install .NET Core SDK 2.1
2. `cd Owor.All`
3. `dotnet publish -r linux-arm`
4. Copy to target Raspi
5. *optional* Adjust settings in `appsettings.json`
6. Start with `./Owor.All`

## Examples

## API

```
GET
api/devices
-----------
returns all devices available on the bus

GET
api/devices/{DEVICE_ID}
-----------------------
returns a single device by its device id
```

### Third Party Extensions

To add support for OEM devices, simply create an extension which implements `IThirdPartyExtension`.

### Web Ui

## Architecture

```plantuml
component Owor.Core
interface Owor.Api
component Owor.Ui
component Owor.Shared
component Owor.Mqtt

Owor.Core ..> Owor.Shared : "<include>"
Owor.Api -- Owor.Core : "<include>"
Owor.Ui --> Owor.Api : "<http>"
Owor.Mqtt --> Owor.Api : "<http>"
```

* `Owor.Shared`: Shared models
* `Owor.Core`: Base lib to read and assemble 1W sensors and their data
* `Owor.Api`: Http interface to retrieve data via `Owor.Core`
* `Owor.Ui`: Razor lib
* `Owor.Mqtt`: Mqtt connector

Also, there exists a catch-all project:

* `Owor.All`: "Base" project to assemble all other projects and run them within the same process

## Data Processing Pipeline

1. Read device ids in configured directory
2. Assemble "default" devices
3. Identify special devices via `ThirdPartyExtensions`
4. Return devices