# RobotCleaner

# ⚠ Warning: Still lacking proper handling of crossing paths ⚠

This means that some testcases are failing and the general results should not be trusted.

## Setup

### Postgres

Create a `.env` file containing wanted Postgres credentials, in the root of the solution

```dotenv
POSTGRES_USER=username
POSTGRES_PASSWORD=password
```

### Service

Create a `secrets.json` file with the following format. Replace `{REDACTED}` username and password with the options from
postgres `.env` file

```json
{
  "ConnectionStrings": {
    "Postgres": "Host=database:5432;Username={REDACTED};Password={REDACTED};Database=ps_db"
  }
}
```

### Running

To run just run 
```shell
docker compose build
docker compose up
```

####  

## Usage

There are two endpoints available to use

###  `GET developer-test/executions`

Fetches all performed executions

### `POST developer-test/execute`

Performs an execution

## Platform environment

The platform consists of a swarm of microservices running as
Docker containers. The primary development platforms are .NET,
Node JS and Python in conjunction with other technologies. Our
main ways of data storage are through PostgreSQL as
relational/document storage and Amazon S3 as blob storage.

## Technical Case

Create a new microservice that could fit into the Platform environment as
described above. The created service will simulate a robot moving in an grid space
and will be cleaning the places this robot visits. The path of the robot's movement is
described by the starting coordinates and move commands. After the cleaning has
been done, the robot reports the number of unique places cleaned. The service will
store the results into the database and return the created record in JSON format. The
service listens to HTTP protocol on port 5000.

### Technical details

```
Request method: POST
Request path: /developer-test/execute
Input criteria:
0 ≤ number of commmands elements ≤ 10000
−100 000 ≤ x ≤ 100 000, x ∈ Z
−100 000 ≤ y ≤ 100 000, y ∈ Z
direction ∈ {north, east, south, west}
0 < steps < 100000, steps ∈ Z
Request body example:
```

#### Example input

```json
{
  "start": {
    "x": 10,
    "y": 22
  },
  "commands": [
    {
      "direction": "east",
      "steps": 2
    },
    {
      "direction": "north",
      "steps": 1
    }
  ]
}
```

The resulting value will be stored in a table named executions together with a
timestamp of insertion, number of command elements and duration of the calculation
in seconds.
Stored record example:
ID Timestamp Commands Result Duration
1234 2018-05-12 12:45:10.851596 2 4 0.000123

### Notes

- You can assume, for the sake of simplicity, that the grid can be viewed as a
  grid where the robot moves only on the vertices.
- The robot cleans at every vertex it touches, not just where it stops.
- All inputs should be considered well-formed and syntactically correct. There is no
  need, therefore, to implement elaborate input validation.
- The robot will never be sent outside the bounds of the grid.
- Ensure that database connection is configurable using environment variable.
- Think about structure, readability, maintainability, performance, re-usability and
  test-ability of the code. Like the solution is going to be deployed into the
  production environment. You should be proud of what you deliver.
- Use only open source dependencies if needed.
- Include Dockerfile and docker-compose configuration files in the solution.
