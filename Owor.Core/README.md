# Owor.Core

This lib provides access to 1W devices.

## HowTo

### Setup

Simple usage

```cs
# Startup.cs
services.AddOwor();
```

You can also pass in custom configuration

```cs
# Startup.cs
services.AddOwor(Configuration);
```

```json
# appsettings.json
{
    "OwConfig": {
        "BasePath": "/home/pi/owroot"
    }
}
```

### Usage

After injecting `IOwAccessor` wherever, you can retrieve data like so:

```cs
owAccessor.GetDevices();
owAccessor.GetDevice(deviceId);
```

## Third Party Extensions

Besides "basic" devices (temperature sensors, ...), this lib also offers extensions to identify and make use of proprietary components. Implementing `IThirdPartyExtension` allows to hook into the device processing pipeline and customize found devcies accordingly.

```plantuml
(*) --> "Identify Basic Devices"
--> "Identify Special Devices"
--> "Return Final Device List"
--> (*)
```

Custom device types (namespace `DeviceTypes`) needed for this *must* be derived from `SpecialOwDevice`!