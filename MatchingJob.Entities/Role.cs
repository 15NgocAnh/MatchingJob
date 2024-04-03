using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MatchingJob.Entities;

public partial class Role
{
    [Key]
    public int Id { get; set; }

    public String? Name { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
