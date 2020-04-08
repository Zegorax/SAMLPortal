<h1 align="center">
SAMLPortal  
</h1>

<h2 align="center">
The bridge between LDAP and SAML with RBAC !
</h2>
[.NET Core](https://github.com/Zegorax/SAMLPortal/workflows/.NET%20Core/badge.svg)

## Introduction
SAMLPortal is a web app designed for companies using a lot of self-hosted services. It act as a SAML IdP (Identity Provider) for your apps. It also doubles as welcome portal for users by presenting apps and categories for which the user has access to.
Every app can be linked to one or more LDAP group, and can be used to initiate a SAML request. The app will only be displayed if the user is part of the LDAP group linked to it. 

## Collaboration
Isses and pull requests are welcome!

## Development environment installation
First, install .NET Core EF
`dotnet tool install --global dotnet-ef`

Copy `.env.example` to `.env` and fill your required environment variables.

Update your database with
`dotnet ef database update`



## VSCode extensions
- .NET Core Add Reference
- .NET Core Extension Pack
- .NET Core Snippet Pack
- .NET Core Test Explorer
- .NET Core Tools
- ASP.NET Core Switcher
- ASP.NET Helper
- Better Comments
- C#
- C# Extensions
- C# FixFormat
- C# XML Documentation Comments
- EditorConfig for VS Code
- Essential ASP.NET Core 3 Snippets
- gitignore
- NuGet Package Manager
- Path Intellisense
- Super Sharp
- Test Explorer UI
- XML