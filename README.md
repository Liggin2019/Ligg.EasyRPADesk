# About Ligg.EasyWinApp
English | [简体中文](./README.zh-CN.md)
- [History](https://www.cnblogs.com/liggin2019/p/11780431.html)
- [Over all introduction](https://www.cnblogs.com/liggin2019/p/11824064.html)
- [Development and application guide](https://liggin2019.gitee.io/projguide), based on version 3.5.2, currently is under construction...
- Current version: 3.5.2.0 
- Version 3.5.2 has greatly enhanced the reusability of the interface and can realize  reusability at all levels（View/Area/Zone/Control）
- 3.5.2 will be a long-term stable release.
- [Gitee Mirror](https://www.gitee.com/liggin2019/Ligg.EasyWinApp)

## Introduction
### This solution  is a Windows application programming framework and UI library, includding 2 projects: Ligg.EasyWinform and Ligg.EasyWinConsole. By this framework, never need any code, only by config file(support: .xlsx、.csv、.xml、.json)
- to build any complex Windows winform GUI,  console program input/output user interface;
- to implement basic process control (value assignment, conditional judgment, loop, jump, etc) in an Excel formular like manner; 
- to implement  basic functionalities (string/file basic functionality, logic judgment, mathematical operation, data input/output and input verification, data conversion, encryption/decryption, form field validation, etc.) in an Excel formular like manner; 
- to process Windows script and Python script;
- to achieve specific business logic processing functionality by dynamically loading 'Plug and Play' .Net component or COM component (CBPL DLL);
- supports various executing mode  like Sync/Async/AsyncAwaited/Managed ThreadPool, supports multi-language.

### Ligg.EasyWinform
Ligg.EasyWinform is a Winform programming framework and UI library. It can excellently imitate the UI of  SAP GUI, 360 security guard software and Symantec endpoint client, and so on.

###  Ligg.EasyWinConsole
Ligg.EasyWinConsole is a Windows console programming framework, can be called by EasyWinform or used alone. It can be used for automation device debugging, software testing, configuration&deployment of IT operation and maintenance , server of instant messaging / message queue, etc.

## Development environment
- Microsoft Visual Studio 2017, version: 15.8.9
- Microsoft .NET Framework version: 4.6.01586
- Microsoft Visual Studio Enterprise 2019 Version 16.9.2
- VisualStudio.16.Release/16.9.2+31112.23
- .NET Framework Version 4.7.2
- .NET Core Version 3.1
- .NET Standard Version 2.0

## Development/Test
#### Please go to the demo folder and run each case against the source code. As shown below
![case](https://liggin2019.gitee.io/Static/images/EasyWinApp/cases.png)

## Usage：
### - For automation equipment development, debugging, operation and maintenance, that is so called 'host computer development', which is also the starting point of this framework. 
- For Embedded / hardware engineers, you do not need to write human-computer interaction code, only need to configure config file, through the built-in SerialConnector /SocketConnector /WebSocketConnector /OpcConnector /OpcUaConnector /MqttConnector interfaces to call the corresponding hardware control program (CBPL DLL), you can create a beautiful equipment operation and maintenance management system by your own. 

### - For testing or prototype design in the process of software development
- No need to write test case code , only use the built-in HttpClientHandler for server-side test, and use the built-in JobScheduler&ThreadDispatcher to carry out stress / robustness test; 
- Generate prototype form or report by configuration , to carry out deep communication and interaction among project manager, product manager, architect, programmer and user in requirements analysis stage, outline design stage, detailed design stage and development stage.

### - For configuration, deployment, monitoring of IT operation and maintenance automation
- Of course, is only for windows. For the  windows + UNIX, we have a solution based on zabbix/granfana.

### - For  tool software especially software with high security requirements
- It can overcome the security weakness of browser-based front-end  (browser client is non blind), and realize various verification and encryption --by lisense dog, by permitting specific hosts/OS to run, by limitting running in specific LAN , by authorizing Windows user to run , request-response data encryption, etc.

### - As web client
- never need any code, you can  customize interface and form to  communication with server by Restful protocol through the encapsulation of HttpClient
- It is especially suitable for the front-end such as MES or WMS, which needs to be connected to the device. After all, WinForm is easier to connect to the device than the browser based front-end.


## License
[MIT](https://github.com/Liggin2019/Ligg.EasyWinApp/blob/master/LICENSE) license.
Copyright (c) 2019-present Liggin2019

## Running snapshot
#### login
![login](https://liggin2019.gitee.io/Static/images/EasyWinApp/login-en.png)
#### software cover
![software cover](https://liggin2019.gitee.io/Static/images/EasyWinApp/software-cover-en.png)
#### about
![about](https://liggin2019.gitee.io/Static/images/EasyWinApp/about-en.png)
#### main UI
![main UI](https://liggin2019.gitee.io/Static/images/EasyWinApp/main-ui-en.png)  
#### main UI 
![main UI1](https://liggin2019.gitee.io/Static/images/EasyWinApp/main-ui1-en.png)  