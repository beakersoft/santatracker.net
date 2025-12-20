# santatracker.net

Simple API for tracking Santa's journey over Christmas

# Docker

Build

docker build -f src/SantaTracker.Net.WebApi/Dockerfile -t santa-tracker-api:local src

Run

docker run -p 8080:8080 --name santa-tracker santa-tracker-api:local
