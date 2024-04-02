using System;
using System.Collections.Generic;

namespace MatchingJob.Entities;

public partial class Role
{
    public int Id { get; set; }

    public String? Name { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
