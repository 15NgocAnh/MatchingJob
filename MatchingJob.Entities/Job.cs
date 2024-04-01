using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobMatching.DAL.Models;

public partial class Job
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool Status { get; set; }

    public string? Company { get; set; }

    public string? Location { get; set; }

    public DateTime Created { get; set; }

    public DateTime LastUpdated { get; set; }

    public double Salary { get; set; }

    public string? TypeOfWork { get; set; }

    public string? RequiredSkills { get; set; }

    public bool IsDelete { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
