# SharpGetSystem

C# implementation of the [classic](https://www.offensive-security.com/metasploit-unleashed/privilege-escalation/) "GetSystem" using WIN32 function [ImpersonateNamedPipeClient](https://docs.microsoft.com/en-us/windows/win32/api/namedpipeapi/nf-namedpipeapi-impersonatenamedpipeclient).

![example](example.PNG)

### How this works

This code runs both a service and the console.

The console is responsible for installing itself as a system service, creating a Named Pipe server using CreateNamedPipe() and waiting for the service to connect using CreateFile() with the "SECURITY_SQOS_PRESENT | SECURITY_IMPERSONATION" flags. With the incoming connection, the ImpersonateNamedPipeClient() function is executed, enabling the impersonation process. The execution of a new process is performed with DuplicateTokenEx() that duplicates the token and the use of the CreateProcessWithTokenW() function that allows us to spawn a new process with the duplicated token.
