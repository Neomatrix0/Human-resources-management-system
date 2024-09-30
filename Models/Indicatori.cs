using System;
using System.Collections.Generic;

namespace Gestionale_Personale_mvc.Models;

public partial class Indicatori
{
    public int Id { get; set; }

    public double? Fatturato { get; set; }

    public int? Presenze { get; set; }

    public virtual ICollection<Dipendente> Dipendentes { get; set; } = new List<Dipendente>();
}
