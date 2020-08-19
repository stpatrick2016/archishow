# ArchiShow
Tool that lists processor architecture of native and .NET executable files (.exe, .dll) in any folder. For my own goal I didn't need much more, but if you have a feature request (or of course a bug), please don't hesitate to open an issue.

## Usage
Without any parameters, the exe will output to console list of all .exe and .dll files in the current directory in CSV format. This can be useful just copying it into target folder and running from command line like this:
```
ArchiShow.exe > _files.csv
```

### Optional switches
Full syntax:
```
archishow.exe [-p|--path "path to file or folder"]
```
|Switch|Description|
|---|---|
|-p, --path|Full path to a file or folder|

### Examples:
List all executable files in the current directory and save in \_files.csv file:
```
archishow.exe > _files.csv
```

List all executable files in specific folder _C:\My Folder_
```
archishow.exe -p "C:\My Folder"
```
