# Feature: Average Fuel Consumption

## Overview
As a user, I want to see my average fuel consumption (L/100km) for my car so that I can understand how efficiently my car uses fuel, even when my historical data contains partial fill-ups.

---

## Background & Context
The user has years of historical fuel data, the majority of which are partial fill-ups (tank not filled to the top). Per fill-up consumption calculation is not suitable for this data. Instead, average consumption is calculated between **full fill-up anchor points**, which produces accurate results regardless of partial fill-ups in between.

---

## Required Data Model Change

Add an `IsFullTank` boolean field to the `FuelEntry` entity and database table.

- **Default value:** `true`
- **Purpose:** Marks whether the tank was filled completely to the top
- **Migration required:** Yes — a new column must be added to the `FuelEntries` table
- **Historical data:** All existing entries default to `true` (assumed full fill-up)

---

## Calculation Logic

### Anchor Points
- Only entries where `IsFullTank = true` qualify as **anchor points**
- The **first anchor point** is the oldest full fill-up entry
- The **last anchor point** is the most recent full fill-up entry
- Everything after the last anchor point is **excluded** from the calculation

### Formula
- **Total distance** = last anchor odometer − first anchor odometer
- **Total liters** = sum of liters for all entries **between and including anchor points** (both full and partial)
- **Average consumption** = (total liters ÷ total distance) × 100
- Result in **L/100km**, rounded to 2 decimal places

### Example
| # | Date | Odometer | Liters | Full Tank? | Included? |
|---|------|----------|--------|------------|-----------|
| 1 | 2022-01-01 | 10,000 km | 45L | ✅ Yes | ✅ Anchor (start) |
| 2 | 2022-06-01 | 11,200 km | 20L | ❌ No | ✅ Liters counted |
| 3 | 2022-09-01 | 12,100 km | 35L | ❌ No | ✅ Liters counted |
| 4 | 2023-01-01 | 13,500 km | 48L | ✅ Yes | ✅ Anchor (end) |
| 5 | 2023-03-01 | 14,200 km | 25L | ❌ No | ❌ Excluded |

**Calculation:**
- Total distance: 13,500 − 10,000 = **3,500 km**
- Total liters: 20 + 35 + 48 = **103L** (entry #1 liters excluded — it was the opening anchor, representing a full tank at the start)
- Average consumption: (103 ÷ 3,500) × 100 = **2.94 L/100km**

> **Note:** The liters from the **first anchor point** are excluded from the total liters count. That first fill-up established a "full tank" baseline — it does not represent fuel consumed within the measured period.

---

## API Changes

### 1. Updated `FuelEntryRequest`
Add optional field:
- `IsFullTank` (boolean, optional, defaults to `true`)

### 2. Updated `FuelEntryResponse`
Add field:
- `IsFullTank` (boolean)

### 3. New Endpoint
**`GET /api/cars/{carId}/statistics/average-consumption`**

**Success Response (200 OK):**
```json
{
  "averageConsumption": 8.43,
  "totalDistanceKm": 12450,
  "totalLiters": 1049.5,
  "calculatedFrom": "2022-01-15",
  "calculatedTo": "2025-03-10",
  "fullFillUpCount": 18,
  "partialFillUpCount": 47
}
```

**Insufficient Data Response (200 OK):**
```json
{
  "averageConsumption": null,
  "totalDistanceKm": null,
  "totalLiters": null,
  "calculatedFrom": null,
  "calculatedTo": null,
  "fullFillUpCount": 1,
  "partialFillUpCount": 3,
  "message": "At least 2 full fill-up entries are required to calculate average consumption."
}
```

---

## Acceptance Criteria

1. `IsFullTank` field is added to `FuelEntry` entity, `FuelEntryRequest`, and `FuelEntryResponse`
2. `IsFullTank` defaults to `true` if not provided in the request
3. All existing entries in the database default to `true` via migration
4. The statistics endpoint returns `averageConsumption` in L/100km rounded to 2 decimal places
5. Only full fill-up entries are used as **anchor points** for distance calculation
6. All entries **between anchor points** (full and partial) are included in total liters
7. The liters from the **first anchor point** are excluded from total liters
8. Entries **after the last full fill-up** are excluded entirely from the calculation
9. If fewer than 2 full fill-up entries exist, `averageConsumption` returns `null` with a descriptive message
10. The endpoint is protected — requires a valid JWT token
11. The endpoint only returns data for cars belonging to the authenticated user
12. If the car does not exist or does not belong to the user, return `404 Not Found`

---

## Out of Scope (Future Features)
- Per fill-up consumption
- Consumption trends over time
- Date range filtering
- Monthly or yearly breakdowns
- Bulk update of historical entries
- Comparison between cars