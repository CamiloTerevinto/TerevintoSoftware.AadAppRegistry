﻿namespace TerevintoSoftware.AadAppRegistry.Tool.Models;

internal class WebAppRegistrationResult
{
    public string Name { get; set; }
    public Guid ClientId { get; set; }
    public Guid ObjectId { get; set; }
}