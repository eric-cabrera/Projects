﻿namespace Assurity.AgentPortal.Accessors.SubAccount.DTOs;

public class CreateMFADevice
{
    public string Type { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;
}