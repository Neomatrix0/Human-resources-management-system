// classe generica Persona con dati anagrafici come proprietà
// verrà estesa dalla classe Dipendente per sfruttare i campi relativi all'anagrafica
public class Persona
{
     // Proprietà per il nome della persona
    public string Nome { get; set; }

     // Proprietà per il cognome della persona
    public string Cognome { get; set; }

     // Proprietà per la data di nascita della persona
    public string DataDiNascita { get; set; }

    // Costruttore della classe 'Persona' che accetta e inizializza nome, cognome e data di nascita
    public Persona(string nome, string cognome, string dataDiNascita)
    {
        // Inizializza la proprietà 'Nome' con il valore passato al costruttore
        this.Nome = nome;
        // Inizializza la proprietà 'Cognome' con il valore passato al costruttore
        this.Cognome = cognome;
        // Inizializza la proprietà 'DataDiNascita' con il valore passato al costruttore
        this.DataDiNascita = dataDiNascita;
    }
}