## System Name
ctf-sandbox

## Contributors
- [Stéphane Denommé](https://github.com/CurlyFire)

## Licence
MIT

## Background Context
This is a TDD Sandbox to work out the kinks to migrate from a big ball of mud legacy project to a microservice hexagonal architecture with unit tests that respect the modern test pyramid.

The main goals of this project are:
- ✅ Generate a big ball of mud to represent the initial legacy application
- 🔄 Migrate to a microservices hexagonal architecture with unit tests
- 🔄 Get my DDD Bounded Contexts framed correctly
- 🔄 Develop all this code in TDD, following [Valentina Jemuovic's](https://github.com/valentinajemuovic) suggested way.  Her site is [Optivem journal](https://journal.optivem.com).  You can also follow her on [LinkedIn](https://www.linkedin.com/in/valentinajemuovic)

The big ball of mud was created with github copilot agent mode using Claude Sonnet 3.5.  I just wanted to create something that worked without being clean.

## System use cases
- As a competitor, I want to sign up for future CTF competitions
- As a competitor, I want to create a team of competitors
- As a team leader, I want to assign competitors to my team
- As a team leader, I want to particate in a CTF competition
- As a challenge creator, I want to create CTF challenges
- As a CTF organizer, I want to create upcoming CTF competitions
- As a CTF organizer, I want to assign challenges to an upcoming CTF competition
- As a CTF organizer, I want to assign teams to an upcoming CTF competition

## External systems
- Email using [mailpit](https://mailpit.axllent.org/)
- System clock

## System architecture style
MVC monolith

## Architecture diagrams

### System context diagram
```mermaid
C4Context
    Person(user, "Competitor", "Creates or participates in CTF competitions")

    Enterprise_Boundary(b0, "CTF") {
        System(systemctf, "CTF Sandbox", "Manage challenges, teams, and competitions")
        System_Ext(systememail, "E-mail system", "SMTP testing tool for development")

        Rel(user, systemctf, "Registers, creates teams, participates")
        UpdateRelStyle(user, systemctf, $offsetX="-230", $offsetY="-45")

        Rel(systemctf, systememail, "Sends email")
        UpdateRelStyle(systemctf, systememail, $offsetX="-40", $offsetY="-10")
    }
```

### Container diagram
```mermaid
C4Container
    UpdateLayoutConfig($c4ShapeInRow="2", $c4BoundaryInRow="1")

    Person(user, "User", "Creates or participates in CTF competitions")

    System_Boundary(ctf, "CTF Sandbox") {
        Container(webapp, "Web Application (ASP.NET MVC)", "C#", "Handles UI, business logic, and data access")
        Container_Ext(mailpit, "E-mail System", "SMTP / Dev Tool", "Receives email notifications from the app")
        ContainerDb(sqlite, "SQLite Database", "SQLite", "Stores users, teams, challenges, competitions")

        Rel(user, webapp, "Uses")
        UpdateRelStyle(user, webapp,  $offsetX="-50", $offsetY="-20")

        Rel(webapp, sqlite, "Reads from and writes to", "SQL")
        UpdateRelStyle(webapp, sqlite,  $offsetX="-50", $offsetY="-20")

        Rel(webapp, mailpit, "Sends email via SMTP")
        UpdateRelStyle(webapp, mailpit, $offsetX="-50", $offsetY="-30")
    }
```

## Tech stack
Programming language: C#

Frameworks: ASP.Net Core MVC

Database: Sqlite

## Repository Strategy
Mono-Repo

## Branching Strategy
Feature Branching

## Deployment Model
Microsoft Azure
