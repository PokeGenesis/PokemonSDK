---
sidebar_position: 4
---

# FakemonPartsCatalog

Scans a directory for Fakemon part images and provides filtering/listing.

```csharp
var catalog = new FakemonPartsCatalog(partsDirectory);
var parts = catalog.Scan();
```
