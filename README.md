inotify-win
===========

This is a fork from the [inotify-win](https://github.com/thekid/inotify-win) port of the **InotifyWait** tool for Windows, see [inotify-tools](https://github.com/rvoicilas/inotify-tools).


Compiling
=========

It is recommended to use Visual Studio 2022 for compiling. The Community Edition is sufficient.
Open the solution and build or publish **InotifyWait**.

Manual compilation with additional packaging a Zip archive can be done as cake build by executing the following script and following the instructions:

```
CakeBuild.bat
```


Usage
=====
The command line arguments are similar to the original inotify arguments:

```sh
$ inotifywait.exe
Usage: inotifywait [options] path [...]

Options:
-r/--recursive:  Recursively watch all files and subdirectories inside path
-m/--monitor:    Keep running until killed (e.g. via Ctrl+C)
-q/--quiet:      Do not output information about actions
-e/--event list: Events (create, modify, delete, move) to watch, comma-separated (Default: all)
--format format: Format string for output
--exclude:       Do not process any events whose filename matches the specified regex
--excludei:      Do not process any events whose filename matches the specified regex (case-insensitive)
--include:       Only process events whose filename matches the specified regex
--includei:      Only process events whose filename matches the specified regex (case-insensitive)

Formats:
%e             : Event name
%f             : File name
%w             : Path name
%T             : Current date and time
```

Remarks:<br/>
-When a path contains a whitespace put the full path into doublequotes<br />
-It is possible to add multiple paths. Also here it is recommended to put every path in its own doublequotes


Commandline example:
```
InotifyWait.exe -m -r --format "%w\%f [%e]" "C:\"
```


Known issues
============
When moving files, not all events are reported consistently with the original. See [issue #7](https://github.com/thekid/inotify-win/issues/7) for an explanation. **Pull requests welcome!**
