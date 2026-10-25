#!/usr/bin/env bash
set -Eeuo pipefail

readonly script_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
readonly health_url="https://api.sleighwatch.com/health"

cd "$script_dir"

echo "Pulling the latest Santa Tracker image..."
docker compose pull santa-tracker

echo "Recreating the Santa Tracker container..."
docker compose up -d --no-deps --force-recreate santa-tracker

echo "Waiting for the health check..."
for attempt in {1..12}; do
    if health_response=$(curl --fail --silent --show-error \
        --resolve "api.sleighwatch.com:443:127.0.0.1" \
        "$health_url"); then
        echo "Deployment succeeded. Health response: $health_response"
        exit 0
    fi

    sleep 5
done

echo "Deployment failed: the API did not become healthy within 60 seconds." >&2
docker compose logs --tail=100 santa-tracker >&2
exit 1
