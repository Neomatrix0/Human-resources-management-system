using System;
using System.Collections.Generic;

namespace Gestionale_Personale_mvc.Models;

public partial class Mansione
{
    public int Id { get; set; }

    public string? Titolo { get; set; }

    public double? Stipendio { get; set; }

    public virtual ICollection<Dipendente> Dipendentes { get; set; } = new List<Dipendente>();
}
