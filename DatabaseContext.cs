using Microsoft.EntityFrameworkCore;

public class Database : DbContext
{
    public DbSet<Dipendente> Dipendenti { get; set; }
    public DbSet<Mansione> Mansioni { get; set; }
    public DbSet<Statistiche> Statistiche { get; set; }

    // Configura il database SQLite
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=database.db");
    }

    // Configurazione iniziale (ad esempio, se vuoi aggiungere dati predefiniti come le mansioni)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Mansione>().HasData(
            new Mansione { Id = 1, Titolo = "Impiegato", Stipendio = 20000 },
            new Mansione { Id = 2, Titolo = "Programmatore", Stipendio = 25000 },
            new Mansione { Id = 3, Titolo = "Dirigente", Stipendio = 70000 }
        );
    }

public void AggiungiDipendente(string nome, string cognome, DateTime dataDiNascita, string mail, int mansioneId)
{
    var mansione = Mansioni.Find(mansioneId); // Trova la mansione esistente

    if (mansione == null)
    {
        throw new Exception("Mansione non trovata.");
    }

    var dipendente = new Dipendente
    {
        Nome = nome,
        Cognome = cognome,
        DataDiNascita = dataDiNascita.ToString("yyyy-MM-dd"),
        Mail = mail,
        Mansione = mansione
    };

    Dipendenti.Add(dipendente); // Aggiunge il dipendente nel contesto
    SaveChanges(); // Salva i cambiamenti nel database
}


public void AggiungiMansione(Mansione mansione)
{
    Mansioni.Add(mansione); // Aggiunge la mansione nel contesto
    SaveChanges(); // Salva i cambiamenti nel database
}
public List<string> GetDipendentiConId()
{
    return Dipendenti.Select(d => $"ID: {d.Id}, Nome: {d.Nome}, Cognome: {d.Cognome}").ToList();
}

public List<Dipendente> GetUsers()
{
    return Dipendenti.Include(d => d.Mansione).Include(d => d.Statistiche).ToList(); // Include per caricare le relazioni Mansione e Statistiche
}

public bool ModificaDipendente(int dipendenteId, string campoDaModificare, string nuovoValore)
{
    var dipendente = Dipendenti.Include(d => d.Mansione).FirstOrDefault(d => d.Id == dipendenteId);
    if (dipendente == null) return false;

    switch (campoDaModificare)
    {
        case "nome":
            dipendente.Nome = nuovoValore;
            break;
        case "cognome":
            dipendente.Cognome = nuovoValore;
            break;
        case "dataDiNascita":
            dipendente.DataDiNascita = DateTime.Parse(nuovoValore).ToString("yyyy-MM-dd");
            break;
        case "mail":
            dipendente.Mail = nuovoValore;
            break;
        case "stipendio":
            dipendente.Mansione.Stipendio = double.Parse(nuovoValore);
            break;
        default:
            return false;
    }
    SaveChanges();
    return true;
}

public List<Mansione> MostraMansioni()
{
    return Mansioni.ToList();
}

public bool RimuoviDipendente(int dipendenteId)
{
    var dipendente = Dipendenti.Find(dipendenteId); // Trova il dipendente

    if (dipendente != null)
    {
        Dipendenti.Remove(dipendente); // Rimuove il dipendente dal contesto
        SaveChanges(); // Salva i cambiamenti nel database
        return true;
    }
    return false;
}

public void AggiungiIndicatori(int dipendenteId, double fatturato, int presenze)
{
    var dipendente = Dipendenti.Find(dipendenteId); // Trova il dipendente

    if (dipendente != null)
    {
        var statistiche = new Statistiche { Fatturato = fatturato, Presenze = presenze };
        dipendente.Statistiche = statistiche; // Assegna le nuove statistiche
        Statistiche.Add(statistiche); // Aggiunge le statistiche nel contesto
        SaveChanges(); // Salva i cambiamenti nel database
    }
}

public Dipendente CercaDipendentePerMail(string email)
{
    return Dipendenti.Include(d => d.Mansione).Include(d => d.Statistiche).FirstOrDefault(d => d.Mail == email);
}


public void AggiornaIndicatori(int dipendenteId, double nuovoFatturato, int nuovePresenze)
{
    var dipendente = Dipendenti.Include(d => d.Statistiche).FirstOrDefault(d => d.Id == dipendenteId); // Trova il dipendente con le statistiche

    if (dipendente != null && dipendente.Statistiche != null)
    {
        dipendente.Statistiche.Fatturato = nuovoFatturato;
        dipendente.Statistiche.Presenze = nuovePresenze;
        SaveChanges(); // Salva i cambiamenti nel database
    }
}



}

