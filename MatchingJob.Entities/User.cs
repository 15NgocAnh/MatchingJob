using System;
using System.Collections.Generic;

namespace JobMatching.DAL.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public DateOnly BirthDay { get; set; }

    public string? Experience { get; set; }

    public string? Education { get; set; }

    public string? Location { get; set; }

    public bool IsEmailConfirmed { get; set; }

    public bool IsMale { get; set; }

    public bool IsLocked { get; set; }

    public bool IsDeleted { get; set; }

    public Guid? JobsId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public virtual Job? Jobs { get; set; }

    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
