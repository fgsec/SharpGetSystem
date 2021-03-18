# SharpGetSystem

C# implementation of the [classic](https://www.offensive-security.com/metasploit-unleashed/privilege-escalation/) "GetSystem" using WIN32 function [ImpersonateNamedPipeClient](https://docs.microsoft.com/en-us/windows/win32/api/namedpipeapi/nf-namedpipeapi-impersonatenamedpipeclient).

![example](example.PNG)

### How this works

This code runs both a service and the console.

The console is responsible for installing itself as a system service, creating a Named Pipe server using [CreateNamedPipe()](https://docs.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-createnamedpipea) and waiting for the service to connect using [CreateFile()](https://docs.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-createfilea) with the "SECURITY_SQOS_PRESENT | SECURITY_IMPERSONATION" [flags](https://docs.microsoft.com/en-us/windows/win32/ipc/impersonating-a-named-pipe-client). With the incoming connection, the [ImpersonateNamedPipeClient()](https://docs.microsoft.com/en-us/windows/win32/api/namedpipeapi/nf-namedpipeapi-impersonatenamedpipeclient) function is executed, enabling the impersonation process. The execution of a new process is performed with [DuplicateTokenEx()](https://docs.microsoft.com/en-us/windows/win32/api/securitybaseapi/nf-securitybaseapi-duplicatetokenex) that duplicates the token and the use of the [CreateProcessWithTokenW()](https://docs.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-createprocesswithtokenw) function that allows us to spawn a new process with the duplicated token.
