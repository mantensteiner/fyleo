# fyleo

## A simple online file manager

This is a basic .NET Core 3 application that uses Razor Pages. It provides a file-explorer like handling and supports some simple customizations:

- Logo
- Favicon
- Title
- Terms of Service contents
- Translations

It uses the local file system on the server as data store. The application does not use an additional database-system, all data files (user accounts, logs and configurations) are stored directly on the servers file system. 

This application is meant be used for simple and small deployments, e.g. data stores for personal use or as a shared space for small groups of users.

The suggested deployment method via Docker Swarm is defined in the *Setup* folder. With this approach all customizations and data assets are stored on the host system and are mounted into the docker container.