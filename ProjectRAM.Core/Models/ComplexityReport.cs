﻿namespace ProjectRAM.Core.Models;

public readonly record struct ComplexityReport(ulong LogTimeCost, ulong LogSpaceCost, ulong UniTimeCost,
	ulong UniSpaceCost);