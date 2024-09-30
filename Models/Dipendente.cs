using System;
using System.Collections.Generic;

namespace Gestionale_Personale_mvc.Models;

public partial class Dipendente
{
    public int Id { get; set; }

    public string? Nome { get; set; }

    public string? Cognome { get; set; }

    public DateOnly? DataDiNascita { get; set; }

    public string? Mail { get; set; }

    public int? MansioneId { get; set; }

    public int? IndicatoriId { get; set; }

    public virtual Indicatori? Indicatori { get; set; }

    public virtual Mansione? Mansione { get; set; }
}
