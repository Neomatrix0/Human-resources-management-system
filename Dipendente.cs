// classe Dipendente estende Persona quindi ne prende i campi ovvero i dati anagrafici
public class Dipendente : Persona
{
   //  Proprietà 'Id' rappresenta l'identificatore univoco del dipendente, gestito dal database
    public int Id { get; set; }
    // Proprietà 'Mansione' rappresenta la mansione del dipendente connette Dipendente a Mansione
    public Mansione Mansione { get; set; }  
    

    // Proprietà 'Mail' rappresenta l'indirizzo email aziendale del dipendente
    public string Mail { get; set; }

    // Proprietà 'Statistiche' connette il dipendente alle sue statistiche personali (Fatturato, Presenze)
    // Se non vengono passate statistiche al momento della creazione, vengono impostati i valori di default (0 per Fatturato e Presenze)
  
    public Statistiche Statistiche { get; set; } 

// costruttore per inizializzare un oggetto Dipendente con tutti i suoi campi
    public Dipendente(int id,string nome, string cognome, string dataDiNascita, string mail, Mansione mansione, Statistiche statistiche = null)
        : base(nome, cognome, dataDiNascita) // Fa riferimento alla proprietà 'DataDiNascita' della classe base 'Persona'
                                            // Chiamata al costruttore della classe base Persona per impostare i dati anagrafici
    {
        // Assegna l'Id del dipendente
        this.Id =id;
        // Assegna la mansione del dipendente
        this.Mansione = mansione;
        // Assegna l'email aziendale
        this.Mail = mail;
        // Se viene passato un oggetto 'Statistiche', lo assegna, altrimenti crea nuove statistiche con valori predefiniti (0 per Fatturato e Presenze)
        this.Statistiche = statistiche ?? new Statistiche(0, 0);  
    }

    //  Proprietà che  restituisce lo stipendio del dipendente preso da Mansione
    // serve a ottenere il valore dello stipendio dalla  mansione senza dover accedere direttamente a dipendente.Mansione.Stipendio. ma sarà dipendente.Stipendio
    public double Stipendio => Mansione.Stipendio;

  
     // Override del metodo ToString per restituire una rappresentazione leggibile del dipendente in formato stringa
    // Fornisce tutti i dettagli del dipendente, inclusi Id, Nome, Cognome, Data di Nascita, Mansione, Stipendio, Fatturato e Presenze
    public override string ToString()
    {
        return $"ID:{Id},Nome: {Nome}, Cognome: {Cognome}, Data di Nascita: {DataDiNascita}, Mail: {Mail}, Mansione: {Mansione.Titolo}, Stipendio: {Stipendio}, Fatturato: {Statistiche.Fatturato}, Presenze: {Statistiche.Presenze}";
    }
}
