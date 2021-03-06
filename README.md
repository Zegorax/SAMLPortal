<h1 align="center">
SAMLPortal  
</h1>

<h2 align="center">
The bridge between LDAP and SAML with RBAC !
</h2>

![.NET Core](https://github.com/Zegorax/SAMLPortal/workflows/.NET%20Core/badge.svg)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/5f0fda20e96042ceae2f98ccbb4c2e30)](https://app.codacy.com/manual/luca_6/SAMLPortal?utm_source=github.com&utm_medium=referral&utm_content=Zegorax/SAMLPortal&utm_campaign=Badge_Grade_Settings)

## Introduction
SAMLPortal is a web app designed for companies using a lot of self-hosted services. It act as a SAML IdP (Identity Provider) for your apps. It also doubles as welcome portal for users by presenting apps and categories for which the user has access to.
Every app can be linked to one or more LDAP group, and can be used to initiate a SAML request. The app will only be displayed if the user is part of the LDAP group linked to it. 

## How to install
Install Docker and docker-compose
Create a new directory and copy docker-compose.yml inside of it.

Open a new Terminal inside your previously created directory and run :
`docker-compose up -d`

You should have SAMLPortal accessible to [http://localhost:8081](http://localhost:8081) and a PHPLDAPAdmin at [http://localhost:8082](http://localhost:8082)

The memberOf overlay is also required on OpenLDAP. You can activate it by following [this tutorial](https://tylersguides.com/guides/openldap-memberof-overlay/)
To get a bash inside the LDAP container, run : `docker-compose exec openldap bash`

To stop everything, run : docker-compose down

## Collaboration
Isses and pull requests are welcome!

## Development environment installation
1. Install Visual Studio Code
2. Install [Docker](https://www.docker.com/products/docker-desktop)
3. Clone the project and open it with VSCode
4. Confirm the popup from VSCode recommended extensions
5. Confirm the popup from Remote-Containers
6. Everything is ready, nothing to install on your host machine