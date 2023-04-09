# AppReg

[![Nuget version](https://img.shields.io/nuget/v/TerevintoSoftware.AadAppRegistry.Tool)](https://www.nuget.org/packages/TerevintoSoftware.AadAppRegistry.Tool/)

![Sample image of the help screen for the publish api command](https://github.com/CamiloTerevinto/TerevintoSoftware.AadAppRegistry/blob/main/assets/sample.png?raw=true)

## 1. Introduction

This project aims to provide an opinionated way to facilitate creationg Azure App Registrations compatible with both Azure Active Directory (AAD) and Azure B2C. 
While AAD and B2C use the same underlying App Registrations service, they have different ways of dealings with scope names and application uris by default. 

Typically, OAuth 2 applications fall under one of these categories:

* API (provides scopes for other applications to consume)
* Web (for traditional server-side web applications)
* SPA (for client-side web applications and some scenarios like newer desktop applications)
* Native (for native desktop or mobile applications)
* Confidential (for back-end systems that do *not* have a user present)

You can mix all of these together in the same App Registration and nothing bad *should* happen. 

The opinionated side of this package is that it's meant to simplify the registrations of these applications as *separate* App Registrations. Why?

1. Separation of concerns  
As each part of a system is separated into its unique components, changing the registration or the needs of one component maintains the rest of the system intact.

2. Principle of Least Privilege.  
By splitting the components of the system, you can avoid giving more privileges than needed to particular components.   
For example, you can give the mobile application access to the API only, while giving the web application access to the API and other systems like Microsoft Graph.

## 2. Installing this tool

Assuming you already have .NET installed, the first thing to do is install this tool:

```
dotnet tool install -g TerevintoSoftware.AadAppRegistry.Tool
```

After that, the command `appreg` will become available. 

## 3. Using this tool

Notice that all commands in the tool support the `--help` switch thanks to [Spectre.Console](https://spectreconsole.net/).

### 3.1 First steps
There are two critical commands that you'll want to run first - `configure credentials` and `configure mode`:

1. Run `configure credentials -t {TenantId} -c {ClientId} -s {ClientSecret}`, where:
   * TenantId: the ID of the tenant you want to register applications in.
   * ClientId: the Client ID of an App Registration with an API permission of `Application.ReadWrite.All`.
   * ClientSecret: a secret generated for the client used in the previous step.

2. If you need to switch to B2C mode, run `configure mode --use-b2c`.

**Note**: both `configure` commands use a json file stored in the directory where the tool is run form. Future versions will support changing this.

### 3.2 Creating applications

Applications can be created using one of the following commands:

* `publish api`
* `publish web`
* `publish spa`
* `publish confidential`

**Note**: As of release 0.1.0, only the `publish api` command is supported. The rest of the commands are under development.

### 3.3 Sample command

If you run the following: 

```
appreg publish api some-test-api-client --set-default-uri --access-as-user
```

Assuming you are under the AAD mode, you would get an output like:

```json
{
   "Name": "some-test-api-client",
   "ClientId": "2a42e61a-c75b-4f65-93ac-30d12bde9b33",
   "ObjectId": "62ff0583-92bb-43ec-af9d-1c3ee88c8cd6",
   "Uri": "api://2a42e61a-c75b-4f65-93ac-30d12bde9b33",
   "Scope": "api://2a42e61a-c75b-4f65-93ac-30d12bde9b33/access_as_user"
}
```

## 4. Building 

To build this application locally, you only need the .NET 7 SDK. No other dependency is needed at this time.

## 5. Contributing

For feedback/questions/issues, please use the [issue tracker](https://github.com/CamiloTerevinto/TerevintoSoftware.AadAppRegistry/issues) and ensure your question/feedback was not previously reported.

For code contributions, I'm glad to accept [Pull Requests](https://github.com/CamiloTerevinto/TerevintoSoftware.AadAppRegistry/pulls).


## 6. License

This project is licensed under the [MIT license](license.txt).
