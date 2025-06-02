# Assurity.AgentCenter.Api

## Contents 
  - [About The Project](#about-the-project)
  - [Getting Started](#getting-started)
  - [Health Checks](#health-checks)
  - [Related Projects](#related-projects)

## About The Project
REST API that serves as a backend for the 2022 rewrite of AgentPortal

### Naming
The project this API was created for went through a name change, which is not reflected everywhere
in this codebase.  The rename was Agent Portal => Agent Center.

### Built With
* [.NET 6](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-6)

### Architecture Diagrams
![AgentCenter-Web Lucid Export](./_Documentation/AgentCenter-Web.png)
**Important**: This diagram is an export of a work-in-progress Lucid chart that has interactive
functionality.  A link will be provided once access to lucid charts is normalized.  Until then,
individuals with an Assurity Lucid chart account can request access from Paul Bartunek.

### Helpful Tips
When formatting data for excel spreadsheets or the UI, we should try to use the utility DataFormatter.
This will help create consistency across reports and the excel spreadsheets and reduce copied code.

## Getting Started

### Prerequisites

* Visual Studio 2022
* .NET 6 SDK

### Installation

1. Clone the repo
2. Set AgentPortalAPI.Service as startup project
3. If using the default configuration, ensure you have access to Assurity's MongoDb cluster,
which requires locally installed certificates. Certificates may be obtained from the system engineers.
   - Personal: *computerName*.assurity.local
      - Certificate must have a private key (example file extension: *.pfx)
   - Trusted Root Certification Authorities: linuxca.assurity.local

### Docker
**Warning**: As of 10/19/2023 docker support is non-functional.

To run the project as a docker image 
1. Install Docker CLI tools
2. Ensure the CLI is using Linux containers
3. Run the following commands in root
   '''
   docker build -t agentportalapi .

   docker run -it -p 5000:80 --rm --name agentportal agentportalapi
   '''

### Swagger Testing
Authorization against Ping is configured in Program.cs. This means that testing via swagger requires authorization within the Swagger interface using a bearer token acquired from Ping.
Follow the steps below to acquire a Ping bearer token using Postman to allow authorization in Swagger while running in DEV or Local. Note that the move from Vue to Nuxt may require changes to this approach.

1. Create/Add a new request in Postman and click the Authorization tab.
   - The method (e.g. POST) and URL are not relevant for our purposes because we'll be using only the Authorization tab's OAuth 2.0 auth type to get a new access token.
2. Make the following selections/entries for the Authorization tab: 
   - Type: `OAuth 2.0`
   - Add authorization data to: `Request Headers`
   - Header Prefix: `Bearer`
   - Auto-refresh token: selected
   - Token Name: This can be left blank
   - Grant Type: `Authorization Code (With PKCE)`
   - Callback URL: `https://localhost:7000/pingone/callback`
   - Authorize using browser: not selected
   - Auth URL: `https://devaccounts.assurity.com/as/authorize`
   - Access Token URL: `https://devaccounts.assurity.com/as/token`
   - Client ID: `ff0cc137-06c5-4478-86b1-04f75772e898`
   - Client Secret: `mFgxCQvriB9AS8wgQDkX0gwN5IBMDcmw0tu~FVwEkUa.T~PqNqAtcd1s8I3hy7DY`
   - Code Challenge Method: `SHA-256`
   - Code Verifier: This can be left blank
   - Scope: `AgentPortalAPI`
   - State: This can be left blank
   - Client Authentication: `Send client credentials in body`
3. Scroll to the bottom and click the `Get New Access Token`
4. If this is your first time attempting to get a new access token, Postman will pop up a Ping log-in window.
It requires credentials of a Ping user which should have an agent id associated with it via a claim. Currently,
the agent id AAXB can be used to login (for the DEV environment) with the following creds:
   - Username: `unAAXB`
   - Password: `ujp!wdg2BQF@kdf@fpk`
     - If you require a user with a different agent id associated with it, you'll need to acquire the credentials (or create them) in Ping.
5. Once signed in, the pop-up should disappear and you'll be presented with the `MANAGE ACCESS TOKENS` window.
6. To authorize Swagger, copy the long string from the `Access Token` field.
   - You may optionally click the `Use Token` button if you wish to make requests via Postman.
7. In the Swagger page for the AgentPortalAPI, click on the `Authorize` button at the top of the page (or the padlock icon for a particular endpoint) and paste the access token into the `Value` field for the Bearer scheme.
8. Click Authorize and close the authorization pop-up. You may now use the endpoint(s) you've authorized.

Note that testing via Swagger in TEST requires a similar process, but with a different URL, client_id, and client_secret. See your supervisor for TEST values.

## Health Checks
This project has 2 health check endpoints.

### Live - /
On success this health check returns a 200 OK along with text of "Healthy".

This is used by the load balancer to check if the service is up.

### Ready - /healthcheck/ready
This health check returns JSON with details of the health of the service and most direct dependencies.

This leverages [AspNetCore.HealthChecks.* NuGet packages | github](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)

This was introduced for use with New Relic Synthetic Monitoring.

## Related Projects
Here a few closely related projects.

1. [Assurity.AgentCenter.Web](http://tfs1:8080/tfs/Assurity%20Projects%20Collection/AssureLinkRewrite/_git/AgentPortal-Dotnet?_a=readme)
    - Public facing web application that consumes Assurity.AgentCenter.Api (this service).
2. [Assurity.Kafka.Consumer & Assurity.Kafka.MigrationWorker](http://tfs1:8080/tfs/Assurity%20Projects%20Collection/Kafka%20Connect/_git/Assurity.Kafka.App?path=%2FDocumentation%2FREADME.md&version=GBmaster)
    - Services responsible for managing the Events MongoDb database Assurity.AgentCenter.Api uses.
3. [Assurity.PolicyInformation.Api](http://tfs1:8080/tfs/Assurity%20Projects%20Collection/Policy%20Information%20API/_git/PolicyInfoAPI?_a=readme)
    - Service Assurity.AgentCenter.Api consumes to get policy information. 