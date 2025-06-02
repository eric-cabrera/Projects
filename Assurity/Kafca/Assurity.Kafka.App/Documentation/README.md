# Assurity.Kafka.Consumer
Last Revision Date: 2/4/2025

## Contents
  - [Introduction](#introduction)
    - [What Is Kafka?](#what-is-kafka)
  - [Application Setup](#application-setup)
     - [Access Mongo DB](#access-mongo-db)
     - [Assurity Nuget Server](#assurity-nuget-server)
     - [Running the App](#running-the-applications)
        - [Consumer](#consumer)
        - [MigrationWorker](#migrationworker)
  - [Architecture](#architecture)
  - [Managing Kafka](#managing-kafka)
    - [Deployments](#deployments)
  - [Topics](#topics)
  - [Observability](#observability)
  - [Known Issues](#known-issues)

## Introduction

### What Is Kafka?
- [https://kafka.apache.org/intro](https://kafka.apache.org/intro)
> Kafka is a distributed system consisting of servers and clients that communicate via a high-performance TCP network protocol. 
It can be deployed on bare-metal hardware, virtual machines, and containers in on-premise as well as cloud environments.

In a nutshell, Kafka allows us to monitor data (say, in a SQL database) and, when that data changes, propagate those changes automatically for our business needs.
Apache refers to this as "event streaming":
> [E]vent streaming is the practice of capturing data in real-time from event sources like databases, sensors, mobile devices, cloud services, 
and software applications in the form of streams of events; storing these event streams durably for later retrieval; 
manipulating, processing, and reacting to the event streams in real-time as well as retrospectively; 
and routing the event streams to different destination technologies as needed. 
Event streaming thus ensures a continuous flow and interpretation of data so that the right information is at the right place, at the right time.

- There are two projects that live in this solution.  Assurity.Kafka.Consumer and MigrationWorker.
- The Consumer is a background service that consumes events from Confluent Kafka, gathers complimentary data, and then stores the result in Mongo Db.
- The MigrationWorker can be used to mass-migrate data or selectively regenerate data. This behavior is set via the `MongoDbExecutionMode` release variable / appsettings variable.
    - More info is given below.

## Application Setup
  Required to run this project locally.
  -  [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
  - [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/)
  - MongoDb
     - [MongoDb Enterprise](https://www.mongodb.com/try/download/enterprise) is free to use for development purposes and may be installed locally.
     - You can also run the following Docker command to pull the MongoDb image and run it.
       ````docker run --name mongoDb -d -p 27017:27017 mongo:latest````
     - If you would like to use the Docker option but do not have Docker setup locally, instructions can be found [here](https://docs.docker.com/desktop/install/windows-install/).

### Access Mongo DB 
Contact Assurity Helpdesk (ahelpdesk@assurity.com) and request access to the Mongo DB. You will given some files to place on your local disk. 
You can then provide MongoDB Compass with a connection string like the one below and then configure it for:
- Username
- Password
- CRT file location
- PEM file location

#### Example Connection String (Dev)
`mongodb://ky3645:HugsAndKisses@mongodbD1.assurity.local,mongodbD2.assurity.local,mongodbD3.assurity.local/?replicaSet=mongodev&readPreference=secondaryPreferred&tls=true&tlsCAFile=CrtFile.crt&tlsCertificateKeyFile=PemFile.pem&authMechanism=PLAIN&authSource=%24external`

#### Example Configuration
![credentials](./Mongo_Authentication.png)
![certs](./Mongo_Certs.png)

### Optional tools to install
  -  [MongoDb Compass](https://www.mongodb.com/try/download/compass)
     - MongoDb Compass will make it easier to view data in your local MongoDb instance.

### Assurity NuGet Server
Assurity.Kafka uses a few NuGet packages that are hosted internally on Assurity's servers. Because of this, you will need to establish the following 
connection from within the NuGet Package Manager Settings from within Visual Studio. This can be accomplished by:
- Navigate to Tools -> NuGet Package Manager -> Package Manager Settings from within Visual Studio
- Once here, select _Package Sources_ and add a new entry. It can be named whatever you like, but the source location must be 
**http://tfs1/nuget/nuget**

### Running the Applications
Both of these apps should run locally out of the box.

#### Consumer
Set the Assurity.Kafka.Consumer as the startup project and press start in Visual Studio.  
The Consumer will begin pulling topics from Kafka and importing them into the local Mongo Db instance.
**The topics it receives are determined by the `GroupId` appsetting and release variable.**

#### MigrationWorker
Set the MigrationWorker project as the startup project and press start in Visual Studio.  

#### MongoDbExecutionMode
Kafka runs, processes data, and stored it in MongoDB. This is managed by a background service (the Migrator) that is always running.

The `MongoDbExecutionMode` value in the `appsettings.json` file (and release pipeline) control how it behaves. There are three modes:

1. Update
    - Rebuilding Mongo data for policies that have had their 'flagged' property set to 1.
    - Consider this to be the "default" value.
2. Migrate
    - "Backfill" Mongo data for older policies that are NOT actively passing real-time Kafka events.
3. UpdateAndMigrate
    - The above two options in sequence.

Migrate is more database and resource-intensive and is intended for "on-demand" or "from the ground up" population of MongoDB data.
Therefore, **the default setting for this value should be `Update`**.

## Architecture
This application implements .NET's BackgroundService class which implements IHostedService.  
More about that can be found [here](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio).

See below for a diagram of Assurity's event driven architecture flow.  The Consumer App in the diagram represents the Transform App.  
Tables in LifePro are monitored for changes. When changes to those tables are detected, the data is pushed to Kafka.  
Kafka then will notify consumer apps like the Transform Application that a change was made.  
Kafka then takes the data, gathers supplementary data if needed, and stores data in a database.

The MigrationWorker is labeled as ""Back Fill App."

![Architecture Diagram](ConsumerArchitecture.png)

## Managing Kafka
In order to fully review, manage, and update Kafka processes, you will need access to [Confluent Cloud](https://confluent.cloud/login).
Our Kafka processes live and run from this cloud-hosted Service.
Here's a blurb from their website:
> Confluent Cloud is a resilient, scalable, streaming data service based on Apache Kafka®, delivered as a fully managed service. 
Confluent Cloud has a web interface called the Cloud Console, a local command line interface, and REST APIs. 
You can manage cluster resources, settings, and billing with the Cloud Console. 
You can use the Confluent CLI and REST APIs to create and manage Kafka topics and more.

In other words, they are the "Server" layer of our Kafka setup. The codebase you are looking at is the "Client" layer.

The responsibilities of the Server layer are defined by Kafka as follows:

>  Kafka is run as a cluster of one or more servers that can span multiple datacenters or cloud regions. 
Some of these servers form the storage layer, called the brokers. Other servers run Kafka Connect to continuously 
import and export data as event streams to integrate Kafka with your existing systems such as relational 
databases as well as other Kafka clusters. 
To let you implement mission-critical use cases, a Kafka cluster is highly scalable and fault-tolerant: 
if any of its servers fails, the other servers will take over their work to ensure continuous operations without any data loss.

### Deployments
- [MigrationWorker Release Pipeline](https://tfs1.assurity.local:8080/Assurity%20Projects%20Collection/Kafka%20Connect/_release?_a=releases&view=mine&definitionId=23)
- [Consumer Release Pipeline](https://tfs1.assurity.local:8080/Assurity%20Projects%20Collection/Kafka%20Connect/_release?_a=releases&view=mine&definitionId=22)
- [Confluent](https://tfs1.assurity.local:8080/Assurity%20Projects%20Collection/Kafka%20Connect/_release?_a=releases&view=mine&definitionId=2)
  - This release interacts with the schemas stored in Confluent cloud.

### OpenShift
This app is deployed to OpenShift.

#### OpenShift Release Variables
  To add release variables, the following must be performed:
  1. Add the variables to the release pipeline.
  1. Add the variables to the Deploy tasks.
     1. Select the Load Environment step of the Deploy task.
     1. Add the variables to the ConfigMap properties.
        - If the variable is named "PingUrl", add "-PingUrl $(PingUrl)" to the ConfigMap properties option.

## Topics
From Confluent's documentation:
> Apache Kafka's most fundamental unit of organization is the topic, which is something like a table in a relational database. 
As a developer using Kafka, the topic is the abstraction you probably think the most about. 
You create different topics to hold different kinds of events and different topics to hold filtered and transformed versions of the same 
kind of event.

Topics are the discrete packets of information that are sent to partitions to be consumed and processed. 
We define the schema for a topic within Confluent.

A list of Kafka topics that are consumed by the Consumer App can be found in the [Topics class](../Assurity.Kafka/Assurity.Kafka.Utilities/Constants/Topics.cs).  
These topic names represent LifePro database change events.

## Observability

### Consumer

#### New Relic
APM
- [Dev](https://onenr.io/07j9z96VaQO)
- [Test](https://onenr.io/0MR229yEGRY)
- [Prod](https://onenr.io/0EjO8ZNKAQ6)

#### Logs
The consumer can be observed using New Relic. The APM name is Assurity.Kafka.Consumer.Dev/Test/Prod (depending on the environment).
Logs can be found in New Relic using multiple methods, though a quick and easy one is to enter `kafka-consumer-dev` as the search query.

#### Confluent
It can be useful to review consumer lag for a given environment or topic. In order to do so, follow these steps.
Note that a Confluent login is required for this.

1. Login to https://confluent.cloud/login.
2. Click on "Confluent Cloud".
3. Select an environment.
4. Click the "cluster" box.
5. On the left-hand side, you should see a "Clients" button. Click that.
6. Click the "Consumer Lag" tab.
7. You can review any and all of the options. To review a specific environment overview, locate the `groupId` for a given environment.
    1. E.g., the `groupId` release variable for Test is currently set to `assurity-connect-test-transform-os-grp1-temp1`.
8. At the top of the screen, you will find the total "lag," measured in messages, for the environment.
    1. 1.  E.g., if the number is 3,300, it means that the Consumer has a backlog of events to process.
    1. If the number is 0, the Consumer is "caught up" and not currently processing events.
9. Below the top-line figure, each topic is broken out and you can review as needed.

### MigrationWorker
APM
- [Dev](https://onenr.io/0ERP8ZM76RW)
- [Test](https://onenr.io/08jq4ya80Rl)
- [Prod](https://onenr.io/0MR229yOJRY)

#### Logs
The consumer can be observed using New Relic. The APM name is Assurity.Kafka.MigrationWorker.Dev/Test/Prod (depending on the environment).
Logs can be found in New Relic using multiple methods, though a quick and easy one is to enter `kafka-migrationworker-dev` as the search query.

## Known Issues
1. Some PPOLC events are not resulting in Mongo updates. 
    1. To date, no cause has been found. Instead, a daily scheduled job was implemented to locate these policies by examining Sink logs, then flagging the policies for rebuilding. 
    If possible, identify and rectify the underlying issue and disable the scheduled CRON job. 
    See [AC24-1248](https://assurityenterprise.atlassian.net/browse/AC24-1248) and [AC24-1148](https://assurityenterprise.atlassian.net/browse/AC24-1148) for more information.
    1. [Pull request implementing the CRON job](https://tfs1.assurity.local:8080/Assurity%20Projects%20Collection/Kafka%20Connect/_git/Assurity.Kafka.App/pullrequest/21849?_a=files)
    1. The CRON job triggers BackFillManager.FlagPreviousDayChangedPolicies().
