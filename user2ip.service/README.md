
# User2ip service
A windows service designed to run on an Active Directory server which populates a REDIS database with a ip (key) to user (value) mapping based on network logon events.

## User2ip configuration
The following variables in user2ip.service.exe.config controls the service behaviour:
* RedisServers: a comma separated list of redis connection strings compatible with [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) that points to the redis databases which will host the user to IP mapping.
* WindowsDomainRegex: This is a .NET 4.0 compatible regular expression used to tell which Network Logon events will generate a user mapping. This filtering process will try to match the event's Domain field with the informed regex and if its a match, will update the REDIS database. Most of the times its safe to use the domain's NETBIOS name.
* RedisTTL: This variable will tell for how many seconds REDIS will keep a user to ip mapping that haven't seem an update in that time frame.

This project was coded to be compatible with .NET 4.0 client so it could be compatible with all servers running versions >= Windows Server 2003.
