using System;
using System.Collections.Generic;

namespace DreamTeamAPI.Models;

public partial class Team
{
    public Guid Id { get; set; }

    public string Fullname { get; set; } = null!;

    public string JobTitle { get; set; } = null!;
}
