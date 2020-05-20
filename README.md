# Status
[![Project Status: Unsupported – The project has reached a stable, usable state but the author(s) have ceased all work on it. A new maintainer may be desired.](https://www.repostatus.org/badges/latest/unsupported.svg)](https://www.repostatus.org/#unsupported)

# powertools-service
This is a windows service that runs and manages all the EA PowerTools services running in node.js

### Usage

You must build the solution to produce a .exe binary file. Once this is done, you can install the .exe with `sc create` (info here: https://technet.microsoft.com/en-ca/library/bb490995.aspx). There must be a `services.conf` file beside the exe which tells the service which node projects to start. Read below for infor on the `serices.conf` file.

#### Config File

The configuration file is the same format as the `Qlik Sense Service Dispatcher`, and looks as follows:
```
[]
Identity=
Enabled=
DisplayName=
ExecType=
ExePath=
Script=
```

An example looks like:
```
[iportal]
Identity=iportal
Enabled=true
DisplayName=iPortal
ExecType=nodejs
ExePath=C:\Program Files\Qlik\Sense\ServiceDispatcher\Node\node.exe
Script=C:\Program Files\Qlik\Sense\EAPowerTools\iportal\server.js
```
The `ExePath` and `Script` can be relative paths as well.
