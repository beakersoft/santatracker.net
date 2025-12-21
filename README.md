# santatracker.net

Simple API for tracking Santa's journey over Christmas

# Docker

Build

docker build -f src/SantaTracker.Net.WebApi/Dockerfile -t santa-tracker-api:local src

Run

docker run -p 8080:8080 --name santa-tracker santa-tracker-api:local


2025-12-24T21:15:00Z