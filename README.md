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