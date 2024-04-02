using System;
using System.Collections.Generic;

namespace MatchingJob.Entities;

public partial class Skill
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid? UsersId { get; set; }

    public virtual User? Users { get; set; }
}
