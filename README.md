# Run the project locally

Run the following command to run the database for the local development:

```shell
docker run --name fueltracker-postgres \
  -e POSTGRES_DB=fueltracker \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  -d postgres:latest
```

Next, apply migrations if the database is empty:

```shell
dotnet ef database update
```

## Run the frontend

The Angular UI lives in `frontend/`. Run it next to the backend:

```shell
dotnet run --project FuelTracker.API   # http://localhost:5158
cd frontend && npm install && npx ng serve fuel-tracker-ui   # http://localhost:4200
```

The dev server proxies `/api` to the backend (see `frontend/projects/fuel-tracker-ui/proxy.conf.json`).

In production the UI is built and copied into `FuelTracker.API/wwwroot`, then served by the API on the same origin as `/api` (see `Dockerfile`).
