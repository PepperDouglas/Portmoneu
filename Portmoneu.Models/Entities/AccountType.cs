﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portmoneu.Models.Entities;

public partial class AccountType
{
    [Key]
    public int AccountTypeId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string TypeName { get; set; } = null!;

    [StringLength(500)]
    [Unicode(false)]
    public string? Description { get; set; }

    [InverseProperty("AccountTypes")]
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
