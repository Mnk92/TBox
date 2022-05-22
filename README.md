[![GitHub license](https://img.shields.io/github/license/Mnk92/tbox?cacheSeconds=3600&color=informational&label=License)](./LICENSE.md)
[![Build](https://github.com/Mnk92/tbox/actions/workflows/build.yml/badge.svg?cacheSeconds=3600)](https://github.com/Mnk92/tbox/actions?query=workflow%3A%22tbox+build%22+branch%3Amaster)

This project have started as a c# "hello world application" years ago. But now it is powerful tool that help me to work much faster. I will be glad if it will help anybody else.

I have worked mostly with c++ and WinApi. As for me, I think that dotnet applications works very slow and uses to much memory. But optimization is interesting for me. I have decided to create one application with a lot of plugins. This can help reduce memory usage.

This tool have written in my free time for using with the real projects. It interface can be hard for novices. The start point is tray icon. Using tray icon you can open settings window or run any commands from menu.

Technologies: c#, WPF, WCF, EntityFramework, WinForms, LinQ, c++, WinApi

Dependencies: Scintilla.Net, ServiceStack.Text, nunit, ICSharpCode.SharpZipLib, 


Existing plugins:

1) AppSettings manager - Ability to change appsettings in the set of app or web configs. For example you can enable/disable features or configure endpoint for the big set of projects in one click.

2) Market - Simple plugins market. Allows you to download and upload plugins and send feedback for its creators. Also include server. (Development suspended a year ago).

3) Automater - Simple tool to automate everything what is possible. You can write your own scripts in c# and save arguments for all scripts, also you can specify arguments from UI.

4) Availability checker - Check by timer, availability of the shared folders and web sites.

5) BookletPagesGenerator - Page numbers generator, to print information in the book format on your printer. Very useful if your printer works unstable..

6) DevServer runner - Tool to run standard developer server without visual studio. Very useful if you should run many projects in one time, but don't have enough memory

7) Directory processor - Ability to run any software for any subdirectory of the directory. For example you can run localization tool by one click for resx files in the folders with localizations.

8) Encoder - You can easy encode/decode strings and change the formatting for easier reading. Format c++, json. xml,html, minimize text to one line and many others.

9) File watcher - Ability to watch for the set of logs at real time. And see all the information in one stream with tray indicator. Very useful if you have a lot of projects, because you control all logs.

10) HtmlPad - Simple plugin to edit HTML in WYSIWYG style

11) Leaks Interceptor - Created for the analysis of the applications and find any leaks. You can store information for weeks, but not hours as usual.

12) NUnit runner - Tool to run nunit tests in parallel. Support x86, admin rights, cloning of the tests folders and tests synchronization (for example when your tests kill all devservers on end). On the quad core cpu tests run up to 3-4 times faster. Very useful if your tests runs more than 10 minutes, especially if more than hour. 

13) Project manager - Easy way to manage your projects. You can rebuild, work with svn and run it's scripts from tray, collect all your changes for all projects in repo in the one place. Very useful if you have 5 or more projects.

14) RegExp tester - Simple plugin to build regular expressions and test it.

15) Request maker - Ability to build and send any requests. Also you can do DDos to find perfomance issues or memory leaks.

16) Requests watcher - Ability to watch for the set of requests at realtime. And see all the information in one stream with tray indicator. Analog of the Fiddler, but not requre admin rights, because it uses trace information.

17) Searcher - Ability to very fast search in the big set of files for words and file names. I use it to search in the more than 512Mb of the sources files

18) Services commander - Tool to start and stop web services from menu or by hotkey.

19) Sources Uniter - Simple plugin to unite big set of sources in to the one document. Designed mostly for students.

20) Sql Runner - Ability to build, store and run any sql scripts. Also you can do DDos to find perfomance issues.

21) Sql watcher - Ability to watch for the set of SQL logs at realtime. And see all the information in one stream with tray indicator. (Works with NHibernate). Very useful if you can't use profiler, also it format sql and apply arguments.

22) Templates - Ability to create group of files and folders by template. For example you can create localization, qunit test fixture or project by template.

23) Text generator - Small tool to generate text., guid by hotkey. Also it help when you export tables to excel or onenote.


This tool support:

1) auto update from the network folder

2) sending of the crash logs to the network folder

3) showing of the changelog on start.

4) global hotkeys for all operations

5) scheduler for all operations

6) powerfull plugins system with very simple interface.

7) settings are stored for the all plugins

8) themes, but unfortunately I not an expert in wpf and we have only one theme. :)

9) Save positions of the dialogs.

10) Warming up of the slow start code.

11) Multithreading anywhere where it make sense. Optimized for multi core cpu.

12) Lazy initializations to reduce memory usage.


Existing sub tools:

1) ConsoleScriptRunner - can run configured by you scripts with the last provided in UI parameters

2) runAsx86 - run any dotnet AnyCpu application as x86

3) sudo - run any application with admin rights 

4) NUnitAgent - used for running of the tests in parallel

5) msbuild.bat - run build project and stop if error


If you need binaries, you can find them here: https://tbox.codeplex.com/
