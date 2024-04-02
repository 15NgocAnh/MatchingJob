using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatchingJob.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string UserName { get; set; } = null!;

    [Column(TypeName = "date")]
    public DateTime BirthDay { get; set; }

    public string? Experience { get; set; }

    public string? Education { get; set; }

    public string? Location { get; set; }

    public bool? IsEmailConfirmed { get; set; } = false;

    public bool IsMale { get; set; }

    public bool? IsLocked { get; set; } = false;

    public bool? IsDeleted { get; set; } = false;

    public Guid? JobsId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public virtual Job? Jobs { get; set; }

    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
