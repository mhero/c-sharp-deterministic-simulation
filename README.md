# c-sharp-deterministic-simulation

A deterministic, replayable simulation engine with snapshot support and
a clean HTTP API. Built to demonstrate **determinism, immutability, and
replay integrity at scale**.

------------------------------------------------------------------------

## Requirements

-   .NET SDK **9.0**
-   macOS / Linux / Windows

Check version:

``` bash
dotnet --version
```

------------------------------------------------------------------------

## Build

From the repository root:

``` bash
dotnet build
```

------------------------------------------------------------------------

## Run Tests

``` bash
dotnet test
```

This runs determinism, replay, and snapshot integrity tests.

------------------------------------------------------------------------

## Run the API

``` bash
dotnet run --project DeterministicSimulation.Api
```

Default URL:

    http://localhost:5278

------------------------------------------------------------------------

## API Endpoints

### POST `/simulation/run`

Runs a simulation deterministically.

### ✅ Valid Request

``` bash
curl -X POST http://localhost:5278/simulation/run   -H "Content-Type: application/json"   -d '{
    "initialState": {
      "tick": 0,
      "entities": {
        "E1": { "x": 0, "y": 0 }
      }
    },
    "targetTick": 3,
    "events": [
      { "type": "MoveEntity", "tick": 1, "entityId": "E1", "dx": 1000, "dy": 0 },
      { "type": "MoveEntity", "tick": 2, "entityId": "E1", "dx": 0, "dy": 500 },
      { "type": "MoveEntity", "tick": 3, "entityId": "E1", "dx": -250, "dy": -250 }
    ]
  }'
```

### ✅ Response (example)

``` json
{
  "tick": 3,
  "entities": {
    "E1": { "x": 750, "y": 250 }
  }
}
```

------------------------------------------------------------------------

## ❌ Invalid Request (Bad Types)

``` bash
curl -X POST http://localhost:5278/simulation/run   -H "Content-Type: application/json"   -d '{
    "initialState": {
      "tick": "0",
      "entities": {
        "E1": { "x": "e", "y": 0 }
      }
    },
    "targetTick": 3,
    "events": []
  }'
```

### ❌ Error Response

``` json
{
  "error": "Invalid request",
  "details": {
    "initialState.entities.E1.x": [
      "The JSON value could not be converted to System.Int32."
    ]
  }
}
```

------------------------------------------------------------------------

## Design Principles

-   Deterministic ordering
-   Immutable state transitions
-   Snapshot-safe replay
-   DTO-only API boundary
-   No domain object deserialization
-   Replay integrity provable via hashing

------------------------------------------------------------------------

## Project Structure

    DeterministicSimulation.Core   # Engine, state, events
    DeterministicSimulation.Tests  # Determinism & replay tests
    DeterministicSimulation.Api    # HTTP API (DTO boundary)

------------------------------------------------------------------------

## License

MIT
