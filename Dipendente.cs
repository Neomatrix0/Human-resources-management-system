// Classe Dipendente estende Persona quindi ne prende i campi ovvero i dati anagrafici
public class Dipendente : Persona
{
   // Proprietà 'Id' rappresenta l'identificatore univoco del dipendente, gestito dal database
    public int Id { get; set; }

    // Chiave esterna per Mansione
    public int MansioneId { get; set; } // Proprietà chiave esterna per collegare la mansione
    public Mansione Mansione { get; set; }  // Proprietà di navigazione per la relazione con Mansione

    // Proprietà 'Mail' rappresenta l'indirizzo email aziendale del dipendente
    public string Mail { get; set; }

    // Chiave esterna per Statistiche (opzionale, potrebbe essere nullo)
    public int? StatisticheId { get; set; }  // Chiave esterna opzionale per Statistiche
    public Statistiche Statistiche { get; set; } // Proprietà di navigazione per la relazione con Statistiche

    // Costruttore vuoto richiesto da Entity Framework
    public Dipendente() { }

    // Costruttore per inizializzare un oggetto Dipendente con tutti i suoi campi
    public Dipendente(int id, string nome, string cognome, string dataDiNascita, string mail, Mansione mansione, Statistiche statistiche = null)
        : base(nome, cognome, dataDiNascita) // Fa riferimento alla proprietà 'DataDiNascita' della classe base 'Persona'
    {
        // Assegna l'Id del dipendente
        this.Id = id;

        // Assegna l'email aziendale
        this.Mail = mail;

        // Assegna la mansione del dipendente
        this.Mansione = mansione;
        this.MansioneId = mansione.Id;  // Aggiunge l'Id della mansione come chiave esterna

        // Se viene passato un oggetto 'Statistiche', lo assegna, altrimenti crea nuove statistiche con valori predefiniti (0 per Fatturato e Presenze)
        this.Statistiche = statistiche ?? new Statistiche(0, 0);  
        this.StatisticheId = statistiche?.Id;  // Se esiste, aggiungi l'Id delle Statistiche, altrimenti nullo
    }

    // Proprietà che restituisce lo stipendio del dipendente preso da Mansione
    // Serve a ottenere il valore dello stipendio dalla mansione senza dover accedere direttamente a dipendente.Mansione.Stipendio, ma sarà dipendente.Stipendio
    public double Stipendio => Mansione.Stipendio;

    // Override del metodo ToString per restituire una rappresentazione leggibile del dipendente in formato stringa
    public override string ToString()
    {
        return $"ID:{Id}, Nome: {Nome}, Cognome: {Cognome}, Data di Nascita: {DataDiNascita}, Mail: {Mail}, Mansione: {Mansione.Titolo}, Stipendio: {Stipendio}, Fatturato: {Statistiche.Fatturato}, Presenze: {Statistiche.Presenze}";
    }
}
