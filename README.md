# santatracker.net

Simple API for tracking Santa's journey over Christmas

## Route data

The year-independent route lives in `src/SantaTracker.Net.Infrastructure/Data/santa-route.json`. It contains 128 delivery stops ordered by `stopNumber`, plus separate North Pole departure and return endpoints. Stable `id` values identify stops across route revisions.

The file is copied to `Data/santa-route.json` in build and publish output, including the published application used by Docker. A provider can resolve it relative to `AppContext.BaseDirectory`. The API currently still uses the Firebase provider; adding this file does not switch providers.

### Data sources and attribution

- City coordinates: [GeoNames](https://www.geonames.org/) via geonamescache, licensed under [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/). Coordinates are rounded to five decimal places and GeoNames IDs are retained. Stop selection, numbering and Santa's schedule are custom to this project.
- Kiritimati atoll coordinates: [NGA-derived listing on geographic.org](https://geographic.org/geographic_names/name.php?c=kiribati&fid=3399&uni=-1947347).
- Time-zone rules: [IANA Time Zone Database](https://www.iana.org/time-zones), used to convert local arrival times into each season's UTC schedule.

# Docker

Build

docker build -f src/SantaTracker.Net.WebApi/Dockerfile -t santa-tracker-api:local src

Run

docker run -p 8080:8080 --name santa-tracker santa-tracker-api:local


2025-12-24T21:15:00Z
