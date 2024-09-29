# Applicazione Gestionale del personale 

- Conversione da programmazione funzionale ad oggetti del programma Gestionale dipendenti
- Lo scopo è quello di gestire i dati e gli indicatori relativi al personale con l'inserimento o la rimozione dei dati nel database (operazioni CRUD)
- L'applicazione segue il patter MVC per renderla più strutturata e modulare
- Il programma si basa sulla programmazione ad oggetti per mantenere  il codice ordinato generico e riutilizzabile
- Per la gestione del database utilizzeremo SQLite
- Implementazione di Spectre Console per la visione dei dati su console

# Funzionalità principali

- Aggiunta di dipendenti: Inserisci nuovi dipendenti con nome, cognome, data di nascita, e-mail, mansione e altri dettagli.
- Visualizzazione dei dipendenti: Mostra l'elenco completo dei dipendenti e i relativi dettagli, come mansione, stipendio, presenze e fatturato.
- Ricerca dei dipendenti: Cerca i dipendenti tramite la loro e-mail aziendale.
- Modifica dei dati dei dipendenti: Aggiorna singoli campi, come nome, cognome, data di nascita, e-mail, stipendio e indicatori.
- Eliminazione dei dipendenti: Rimuovi dipendenti dal sistema.
- Ordine per stipendio: Ordina i dipendenti in base al loro stipendio, dal più alto al più basso.
- Valutazione per fatturato: Valuta i dipendenti in base al loro fatturato e visualizza i più e meno performanti.
- Tasso di presenza: Calcola il tasso di presenza dei dipendenti in un anno di lavoro.
- Aggiornamento di fatturato e presenze: Aggiungi e aggiorna gli indicatori di performance dei dipendenti.

# Tecnologie utilizzate
- C# (OOP): Implementazione ad oggetti per migliorare modularità e riusabilità.
- SQLite: Per la gestione del database locale che memorizza dipendenti, mansioni e indicatori.
- Spectre.Console: Per la visualizzazione dei dati in tabelle nella console.

## Strutturazione del database

- Il database è suddiviso in tre tabelle:

    - Dipendente: Memorizza le informazioni anagrafiche e collega il dipendente a una mansione e a degli indicatori di performance.
    - Mansione: Contiene le informazioni relative al titolo della mansione e allo stipendio.
    - Indicatori: Memorizza i valori di fatturato e presenze del dipendente.

<details>

<summary>Visualizza il codice sqlite</summary>

```sql

CREATE TABLE IF NOT EXISTS dipendente (id INTEGER PRIMARY KEY, nome TEXT, cognome TEXT, dataDiNascita DATE, mail TEXT,  mansioneId INTEGER,indicatoriId INTEGER,  FOREIGN KEY(mansioneId) REFERENCES mansione(id),FOREIGN KEY(indicatoriId) REFERENCES indicatori(id)) ;

CREATE TABLE IF NOT EXISTS mansione (id INTEGER PRIMARY KEY AUTOINCREMENT, titolo TEXT UNIQUE,stipendio REAL);

CREATE TABLE IF NOT EXISTS indicatori (id INTEGER PRIMARY KEY AUTOINCREMENT, fatturato DOUBLE,presenze INTEGER);



```

</details>

# MODELLO MVC E STRUTTURAZIONE CLASSI

- Classe Database -> gestisce creazione e operazioni direttamente sul database
<details>

<summary>visualizza codice </summary>

```c#

// importazione librerie per Sqlite per la gestone del database e Spectre Console per la visualizzazione in console
using System.Data.SQLite;
using Spectre.Console;

// creazione classe Database per creare e gestire il database del gestionale tramite operazioni CRUD 
class Database
{
    private SQLiteConnection _connection; // SQLiteConnection è una classe che rappresenta una connessione a un database SQLite si definisce classe



    // costruttore della classe Database  si occupa di aprire la connessione e creare le tabelle se non esistono
    public Database()
    {
        _connection = new SQLiteConnection("Data Source=database.db"); // Creazione di una connessione al database
        _connection.Open(); // Apertura della connessione

        // creazione della tabella principale chiamata dipendente se non esiste
        var command1 = new SQLiteCommand(
          "CREATE TABLE IF NOT EXISTS dipendente (id INTEGER PRIMARY KEY, nome TEXT, cognome TEXT, dataDiNascita DATE, mail TEXT,  mansioneId INTEGER,indicatoriId INTEGER,  FOREIGN KEY(mansioneId) REFERENCES mansione(id),FOREIGN KEY(indicatoriId) REFERENCES indicatori(id)) ;",
          _connection);
        command1.ExecuteNonQuery(); // Esecuzione del comando

        // Creazione della tabella mansione collegata alla tabella dipendente
        var command2 = new SQLiteCommand(
            "CREATE TABLE IF NOT EXISTS mansione (id INTEGER PRIMARY KEY AUTOINCREMENT, titolo TEXT UNIQUE,stipendio REAL);",
            _connection);
        command2.ExecuteNonQuery(); // Esecuzione del comando

        // creazione della tabella indicatori collegata alla tabella dipendente
        var command3 = new SQLiteCommand(
            "CREATE TABLE IF NOT EXISTS indicatori (id INTEGER PRIMARY KEY AUTOINCREMENT, fatturato REAL,presenze INTEGER);",
            _connection);
        command3.ExecuteNonQuery(); // Esecuzione del comando
        AggiungiMansioniPredefinite();  // metodo che aggiunge delle mansioni di default nella tabella mansione
    }

    // metodo AggiungiDipendente che permette l'aggiunta del dipendente nel database 
    //mettendo come input i valori richiesti  nome, cognome, data di nascita, email e ID della mansione
    
    public void AggiungiDipendente(string nome, string cognome, DateTime dataDiNascita, string mail, int mansioneId)
    {
        var command = new SQLiteCommand($"INSERT INTO dipendente (nome,cognome,dataDiNascita,mail,mansioneId) VALUES (@nome,@cognome,@dataDiNascita,@mail,@mansioneId)", _connection); // Creazione di un comando per inserire un nuovo utente
        command.Parameters.AddWithValue("@nome", nome);
        command.Parameters.AddWithValue("@cognome", cognome);
        command.Parameters.AddWithValue("@dataDiNascita", dataDiNascita.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("@mail", mail);
        command.Parameters.AddWithValue("@mansioneId", mansioneId);
        command.ExecuteNonQuery(); // Esecuzione del comando
    }

    // metodo AggiungiMansione che permette l'aggiunta della mansione alla tabella mansione con valori titolo e stipendio
    public int AggiungiMansione(Mansione mansione)
    {
        // Il comando SELECT last_insert_rowid():  seleziona l'ID della riga che è stata appena inserita
        var command = new SQLiteCommand("INSERT INTO mansione(titolo,stipendio) VALUES (@titolo,@stipendio);SELECT last_insert_rowid()", _connection);
        command.Parameters.AddWithValue("@titolo", mansione.Titolo);
        command.Parameters.AddWithValue("@stipendio", mansione.Stipendio);


        //esegue la query SQL e restituisce il valore della prima colonna della prima riga del risultato (in questo caso, l'ID appena inserito).
        //Convert.ToInt32(...) converte il risultato (che è di tipo object) in un tipo int.

        return Convert.ToInt32(command.ExecuteScalar());


    }

    // metodo AggiungiIndicatori che aggiunge valori del fatturato e delle presenze del dipendente nel database

    public void AggiungiIndicatori(int dipendenteId, double fatturato, int presenze)
    {
        // Creare un nuovo record nella tabella `indicatori` che memorizza il fatturato e le presenze del dipendente
        var commandIndicatori = new SQLiteCommand("INSERT INTO indicatori (fatturato, presenze) VALUES (@fatturato, @presenze); SELECT last_insert_rowid();", _connection);
        // Aggiunge i valori di fatturato e presenze come parametri per evitare SQL injection
        commandIndicatori.Parameters.AddWithValue("@fatturato", fatturato);
        commandIndicatori.Parameters.AddWithValue("@presenze", presenze);
        // Esegue l'inserimento e ottiene l'ID del nuovo record `indicatori` appena inserito.
        int indicatoriId = Convert.ToInt32(commandIndicatori.ExecuteScalar());

        // Aggiorna il record del dipendente associando il nuovo `indicatoriId` al campo `indicatoriId` nella tabella `dipendente`
        var commandDipendente = new SQLiteCommand("UPDATE dipendente SET indicatoriId = @indicatoriId WHERE id = @dipendenteId", _connection);
        commandDipendente.Parameters.AddWithValue("@indicatoriId", indicatoriId);
        commandDipendente.Parameters.AddWithValue("@dipendenteId", dipendenteId);
        commandDipendente.ExecuteNonQuery(); // Esegui l'aggiornamento
    }


    // Metodo AggiornaIndicatori che aggiorna i valori di fatturato e presenze di un dipendente specifico
    public void AggiornaIndicatori(int dipendenteId, double nuovoFatturato, int nuovePresenze)
    {
        // Aggiorna il record nella tabella `indicatori` collegato al dipendente
        // Si seleziona l'`indicatoriId` dalla tabella `dipendente`, corrispondente al dipendente con `dipendenteId` fornito
        var command = new SQLiteCommand("UPDATE indicatori SET fatturato = @fatturato, presenze = @presenze WHERE id = (SELECT indicatoriId FROM dipendente WHERE id = @dipendenteId)", _connection);
        command.Parameters.AddWithValue("@fatturato", nuovoFatturato);
        command.Parameters.AddWithValue("@presenze", nuovePresenze);
        command.Parameters.AddWithValue("@dipendenteId", dipendenteId);
        command.ExecuteNonQuery(); // Esegui l'aggiornamento
    }

    // metodo AggiungiMansioniPredefinite inserisce delle mansioni di default al database nella tabella mansione
    //Ogni mansione è un oggetto della classe Mansione  con valori predefiniti per titolo e stipendio

    private void AggiungiMansioniPredefinite()
    {
        // Crea una lista di mansioni di tipo Mansione predefinite con titolo e stipendio
        var mansioni = new List<Mansione>{
            new Mansione("impiegato",20000),
             new Mansione("programmatore",25000),
              new Mansione("dirigente",70000),
              new Mansione("receptionist",15000),
               new Mansione("general manager",120000),
                new Mansione("ceo",4000000)
        };
        // Itera attraverso ciascuna mansione predefinita 
        foreach (var mansione in mansioni)
        {

            // Crea un comando SQL per verificare se una mansione con lo stesso titolo esiste già nel database
            // SELECT COUNT(*) conta quante righe nella tabella `mansione` hanno un valore nel campo `titolo`
            var checkCommand = new SQLiteCommand("SELECT COUNT(*) FROM mansione WHERE titolo = @titolo", _connection);
            checkCommand.Parameters.AddWithValue("@titolo", mansione.Titolo);  // Aggiunge il titolo della mansione come parametro
                                                                               // Esegue la query e restituisce il conteggio delle righe con lo stesso titolo di mansione
                                                                               // Se il conteggio restituito è maggiore di 0, significa che esiste già una mansione con quel titolo e quindi non verrà aggiunta al database
            var count = Convert.ToInt32(checkCommand.ExecuteScalar());
            // Se il titolo della mansione non esiste nel database (count == 0), la mansione viene aggiunta

            if (count == 0)
            {
                AggiungiMansione(mansione);
            }

        }
    }

    // Metodo RimuoviDipendente per eliminare un dipendente dal database usando il suo ID
    public bool RimuoviDipendente(int dipendenteId)
    {
        var command = new SQLiteCommand("DELETE from dipendente WHERE id= @id", _connection);
        // Aggiunge il parametro `@id` al comando SQL e lo imposta come l'ID del dipendente che si vuole rimuovere
        command.Parameters.AddWithValue("@id", dipendenteId);
        // Esegue il comando di eliminazione e restituisce il numero di righe interessate
        int affectedRows = command.ExecuteNonQuery();
        // Restituisce `true` se almeno una riga è stata eliminata, altrimenti `false`
        return affectedRows > 0;

    }

    // Il metodo GetDipendentiConId ha il compito di recuperare l'ID, il nome e il cognome di tutti i dipendenti nel database e restituirli in una lista di stringhe.
    public List<string> GetDipendentiConId()
    {
        // Crea un comando SQL per selezionare l'ID, il nome e il cognome di tutti i dipendenti

        var command = new SQLiteCommand("SELECT id, nome, cognome FROM dipendente", _connection);
        var reader = command.ExecuteReader();

        // Crea una lista di stringhe per memorizzare i risultati (ID, Nome, Cognome dei dipendenti)
        var dipendenti = new List<string>();

        // Cicla attraverso i risultati letti dal database

        while (reader.Read())
        {
            // Crea una stringa con l'ID, il nome e il cognome del dipendente
            string info = $"ID: {reader.GetInt32(0)}, Nome: {reader.GetString(1)}, Cognome: {reader.GetString(2)}";
            // Aggiunge la stringa alla lista 'dipendenti'
            dipendenti.Add(info);
        }
        //restituisce lista dipendenti
        return dipendenti;
    }

    // Metodo GetUsers permette la lettura dei dati dei dipendenti dal database SQLite 
    //aggrega questi dati in oggetti Dipendente infine restituisce una lista di questi oggetti. 
    public List<Dipendente> GetUsers()
    {
        var command = new SQLiteCommand(
            "SELECT dipendente.id, dipendente.nome, dipendente.cognome, strftime('%d/%m/%Y', dataDiNascita) AS data_formattata, dipendente.mail, mansione.titolo, mansione.stipendio, indicatori.fatturato, indicatori.presenze " +
            "FROM dipendente " +
            "JOIN mansione ON dipendente.mansioneId = mansione.id " +           //JOIN  collega la tabella mansione alla tabella dipendente
            "LEFT JOIN indicatori ON dipendente.indicatoriId = indicatori.id;", //Left Join permette di includere anche valori nulli negli indicatori
            _connection);

        var reader = command.ExecuteReader();
        // Crea una lista vuota di oggetti Dipendente
        var dipendenti = new List<Dipendente>();

        while (reader.Read())
        {
            // Crea un oggetto Mansione
            var mansione = new Mansione(reader.GetString(5), reader.GetDouble(6));

            // Crea un oggetto Statistiche
            var statistiche = new Statistiche(
                reader.IsDBNull(7) ? 0 : reader.GetDouble(7),  // Fatturato
                reader.IsDBNull(8) ? 0 : reader.GetInt32(8)     // Presenze
            );

            // Crea un nuovo oggetto Dipendente utilizzando il nuovo costruttore con l'ID
            var dipendente = new Dipendente(
                reader.GetInt32(0),  // ID
                reader.GetString(1), // Nome
                reader.GetString(2), // Cognome
                reader.GetString(3), // Data di Nascita
                reader.GetString(4), // Mail
                mansione,            // Mansione
                statistiche          // Statistiche
            );

            dipendenti.Add(dipendente);
        }

        return dipendenti;
    }

    // metodo CercaDipendentePerMail  permette di cercare il dipendente tramite mail e mostra nel terminale i dettagli correlati una volta trovato

    public Dipendente CercaDipendentePerMail(string email)
    {
        var command = new SQLiteCommand(
            @"SELECT dipendente.id, 
                 dipendente.nome, 
                 dipendente.cognome, 
                 strftime('%d/%m/%Y', dataDiNascita) AS data_formattata, 
                 dipendente.mail, 
                 mansione.titolo, 
                 mansione.stipendio, 
                 indicatori.fatturato, 
                 indicatori.presenze
          FROM dipendente
          JOIN mansione ON dipendente.mansioneId = mansione.id
          LEFT JOIN indicatori ON dipendente.indicatoriId = indicatori.id 
          WHERE dipendente.mail = @mail;",
            _connection);

        command.Parameters.AddWithValue("@mail", email);
        var reader = command.ExecuteReader();

        if (reader.Read())
        {
            // Creazione dell'oggetto Mansione con i campi corretti
            var mansione = new Mansione(reader.GetString(5), reader.GetDouble(6));

            // Gestione dei campi indicatori con controlli per i valori NULL
            double fatturato = reader.IsDBNull(7) ? 0.0 : reader.GetDouble(7); // Usa GetDouble per i valori reali
            int presenze = reader.IsDBNull(8) ? 0 : reader.GetInt32(8);        // Usa GetInt32 per valori interi

            // Creazione dell'oggetto Statistiche con fatturato e presenze
            var statistiche = new Statistiche(fatturato, presenze);

            // Creazione dell'oggetto Dipendente con ID
            var dipendente = new Dipendente(
                reader.GetInt32(0),  // ID
                reader.GetString(1),  // Nome
                reader.GetString(2),  // Cognome
                reader.GetString(3),  // Data di Nascita
                reader.GetString(4),  // Mail
                mansione,             // Mansione
                statistiche           // Statistiche
            );
            // Restituisce la lista di dipendenti
            return dipendente;
        }

        // Restituisci null se il dipendente non viene trovato
        return null;
    }


    // metodo MostraMansioni permette la lettura dei dati dalla tabella mansione per id,titolo,stipendio 

    public List<Mansione> MostraMansioni()
    {
        var command = new SQLiteCommand("SELECT id, titolo, stipendio FROM mansione;", _connection);
        var reader = command.ExecuteReader(); // Esecuzione del comando e creazione di un oggetto per leggere i risultati
        var mansioni = new List<Mansione>(); // Creazione di una lista per memorizzare i nomi degli utenti
        while (reader.Read())
        {
            // Creazione di un oggetto Mansione e aggiunta alla lista
            var mansione = new Mansione(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(2)); // Aggiunta dati utente alla lista
            mansioni.Add(mansione);
        }
        return mansioni;
    }

    // il metodo ModificaDipendente permette di modificare un singolo campo del dipendente nel database 
    // Restituisce `true` se la modifica ha successo e `false` se si verifica un errore o nessuna riga viene modificata
    public bool ModificaDipendente(int dipendenteId, string campoDaModificare, string nuovoValore)
    {
        try
        {
            string query = "";  // Variabile per memorizzare la query SQL che verrà eseguita
            object indicatoriId = null;   // Variabile per memorizzare l'ID degli indicatori, se necessario

            // Verifica se stai modificando "fatturato" o "presenze", che appartengono alla tabella `indicatori`
            if (campoDaModificare == "fatturato" || campoDaModificare == "presenze")
            {
                // Prima controlla se il dipendente ha un ID indicatori associato
                var checkCommand = new SQLiteCommand("SELECT indicatoriId FROM dipendente WHERE id = @id", _connection);
                checkCommand.Parameters.AddWithValue("@id", dipendenteId);
                indicatoriId = checkCommand.ExecuteScalar(); // Esegue la query per recuperare l'ID degli indicatori
                                                             // Se il dipendente non ha un indicatoriId associato, restituisce un errore
                if (indicatoriId == null || indicatoriId == DBNull.Value)
                {
                    Console.WriteLine("Errore: Il dipendente non ha un ID indicatori associato.");
                    return false;
                }

                // Aggiorna la tabella `indicatori`
                query = $"UPDATE indicatori SET {campoDaModificare} = @nuovoValore WHERE id = @indicatoriId";
            }
            else
            {
                // Se il campo da modificare non è "fatturato" o "presenze", costruisce la query per aggiornare la tabella `dipendente`
                query = $"UPDATE dipendente SET {campoDaModificare} = @nuovoValore WHERE id = @id";
            }

            // Crea il comando SQLite utilizzando la query costruita

            using (var command = new SQLiteCommand(query, _connection))
            {
                // Aggiunge i parametri alla query
                command.Parameters.AddWithValue("@nuovoValore", nuovoValore);
                command.Parameters.AddWithValue("@id", dipendenteId);

                // Se stiamo aggiornando la tabella `indicatori`, aggiunge anche l'ID degli indicatori come parametro
                if (campoDaModificare == "fatturato" || campoDaModificare == "presenze")
                {
                    command.Parameters.AddWithValue("@indicatoriId", indicatoriId);
                }

                // Esegue l'aggiornamento e restituisce il numero di righe modificate

                int rowsAffected = command.ExecuteNonQuery();

                // Verifica se almeno una riga è stata modificata
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Aggiornamento riuscito.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Nessuna riga modificata.");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            // Se si verifica un errore, viene gestito qui
            Console.WriteLine("Errore durante la modifica del dipendente: " + ex.Message);
            return false;
        }
    }





}

```
</details>

- Classe View -> gestisce visione del menu principale
<details>
<summary>visualizza codice</summary>

```c#

using System.Data.SQLite;
using Spectre.Console;
class View
{
    private Database _db; // Riferimento al modello Database

// Costruttore della classe View che riceve un oggetto 'Database' come argomento e lo assegna al campo privato _db.
    public View(Database db)
    {
        _db = db; // Inizializzazione del riferimento al modello Database
    }

      // Metodo per mostrare il menu principale e restituire la scelta dell'utente
    public string MostraMenuPrincipale()
    {
        // Genera e visualizza il menu con Spectre.Console
        var input = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("GESTIONALE DIPENDENTI")
            .PageSize(8)
            .MoreChoicesText("[grey](Move up and down to reveal more)[/]")
            .AddChoices(new[] {
                "Aggiungi Dipendente", "Mostra Dipendenti", "Rimuovi Dipendente",
                "Cerca Dipendente", "Modifica Dipendente","Ordina stipendi","Aggiungi indicatori","Tasso di presenza","Valutazione per fatturato","Incidenza percentuale", "Esci",
            }));

        return input; // Restituisce la scelta dell'utente
    }


  // Metodo per mostrare la lista dei dipendenti (come stringhe) ricevuta in input
    public void MostraDipendenti(List<string> users)
    {
        foreach (var user in users)
        {
            Console.WriteLine(user); // Visualizzazione dei nomi dei dipendenti
        }
    }

// Metodo per raccogliere input dell'utente da console
    public string GetInput()
    {
        return Console.ReadLine();  // Ritorna il testo inserito dall'utente nella console
    }
}

```
</details>

- Classe Controller -> Gestisce la logica del programma e funge da collegamento tra View e Database

<details>

<summary>Visualizza il codice class Controller</summary>

```c#

using System.Data.SQLite;
using Spectre.Console;
// classe Controller funge da collegamento tra il Model Database e View
// gestice la logica dell'applicazione
class Controller
{
    private Database _db; // Riferimento al modello
    private View _view; // Riferimento alla vista

    // Costruttore della classe Controller
    public Controller(Database db, View view)
    {
        _db = db; // Inizializzazione del riferimento al modello
        _view = view; // Inizializzazione del riferimento alla vista
    }
    
  // Metodo principale che gestisce il menu tramite spectre console  richiamato dalla funzione MonstraMenuPrincipale
    public void MainMenu()
    {
        while (true)
        {
            // Chiede alla View di mostrare il menu e restituisce la scelta dell'utente
            var input = _view.MostraMenuPrincipale();

            // Esegui le operazioni in base alla scelta dell'utente
            if (input == "Aggiungi Dipendente")
            {
                AggiungiDipendente(); // Aggiunta di un utente
            }
            else if (input == "Mostra Dipendenti")
            {
                MostraDipendenti(); // Visualizzazione degli utenti
            }
            else if (input == "Rimuovi Dipendente")
            {
                RimuoviDipendente();    // Rimuove un dipendente
            }
            else if (input == "Cerca Dipendente")
            {
                CercaDipendente(); // Cerca un dipendente tramite email
            }
            else if (input == "Modifica Dipendente")
            {
                ModificaDipendente(); // Modifica un dipendente
            }
            else if (input == "Ordina stipendi")
            {
                OrdinaStipendi(); // Ordina i dipendenti per stipendio
            }
            else if (input == "Aggiungi indicatori")
            {
                AggiungiIndicatoriDipendente(); // Aggiungi indicatori fatturato e presenze a un dipendente
            }
            else if (input == "Tasso di presenza")
            {
                TassoDiPresenza(); // Calcola e visualizza il tasso di presenza in percentuale
            }
            else if (input == "Valutazione per fatturato")
            {
                ValutazioneFatturatoProdotto(); // Valutazione dei dipendenti per fatturato
            }
            else if (input == "Incidenza percentuale")
            {
                IncidenzaPercentuale(); // Da completare
            }
            else if (input == "Esci")
            {
                break; // Uscita dal programma
            }
        }
    }
 
// il metodo AggiungiDipendente gestisce l'intero processo di raccolta dei dati per un nuovo dipendente
// verifica la validità delle informazioni e inserisce il nuovo dipendente nel database del sistema.
    private void AggiungiDipendente()
    {
        Console.WriteLine("Inserisci il nome:"); // Richiesta del nome dell'utente
        var nome = _view.GetInput(); // Lettura del nome dell'utente
        Console.WriteLine("Inserisci il cognome:");
        var cognome = _view.GetInput();
        Console.WriteLine("Inserisci la data di nascita DD/MM/YYYY:");
        var dataDiNascitaString = _view.GetInput();
        Console.WriteLine("Inserisci la mail aziendale:");
        var mail = _view.GetInput();
         // Mostra le mansioni disponibili recuperate dal database per consentire all'utente di scegliere
        var mansioni = _db.MostraMansioni();
         // Ottiene la lista delle mansioni dal database
        foreach (var mansione in mansioni)
        {
            // Stampa ogni mansione con il suo ID e altri dettagli per visionare l'id di quale mansione  aggiungere
            Console.WriteLine($"ID: {mansione.Id}, Titolo: {mansione.Titolo}, Stipendio: {mansione.Stipendio}");
        }

        Console.WriteLine("Scegli tra le mansioni disponibili per id:");
        var mansioneInput = _view.GetInput();
        // Verifica se l'input della mansione è un intero valido
        if (int.TryParse(mansioneInput, out int mansioneId))
        {
            var mansioneid = Console.ReadLine();
            // Verifica se la data di nascita inserita è valida nel formato "dd/MM/yyyy"
            if (DateTime.TryParseExact(dataDiNascitaString, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataDiNascita))
            {
                // Aggiunta del dipendente nel database con nome, cognome, data di nascita, email e ID della mansione
                _db.AggiungiDipendente(nome, cognome, dataDiNascita, mail, mansioneId);
                Console.WriteLine("Dipendente aggiunto con successo."); // Aggiunta del dipendente al database
            }
            else
            {
                Console.WriteLine("Formato data di nascita non valido. Riprova.");
            }

        }
        else
        {
            Console.WriteLine("ID mansione non valido. Riprova.");
        }
    }

// il metodo RimuoviDipendente gestisce il processo di eliminazione di un dipendente dal database
//fornendo all'utente un elenco dei dipendenti verificando l'input e rimuovendo il dipendente selezionato in modo sicuro
    private void RimuoviDipendente()
    {
        // Mostra l'elenco dei dipendenti con il loro ID, nome e cognome
        Console.WriteLine("Elenco dei dipendenti:");
        var dipendentiConId = _db.GetDipendentiConId();
        // Itera attraverso i dipendenti e visualizza le loro informazioni
        foreach (var dipendente in dipendentiConId)
        {
            Console.WriteLine(dipendente);
        }
        Console.WriteLine("Inserisci l'ID del dipendente da rimuovere:");
        try
        {
            // Usa Convert.ToInt32 per convertire l'input dell'utnete (ID) in un intero
            var dipendenteId = Convert.ToInt32(Console.ReadLine());

            // Prova a rimuovere il dipendente con l'ID specificato
            bool successo = _db.RimuoviDipendente(dipendenteId);
            if (successo)
            {
                Console.WriteLine("Dipendente rimosso con successo.");
            }
            else
            {
                Console.WriteLine("Dipendente non trovato o ID non valido.");
            }
        }
        catch (FormatException)
        {
            // Gestisce l'errore se l'input non è un numero valido
            Console.WriteLine("ID non valido. Inserisci un numero.");
        }

    }

    //Il metodo CercaDipendente consente di cercare un dipendente nel database
    // utilizzando la sua email aziendale come chiave di ricerca.ad esempio nome.cognome@gmail.com
    private void CercaDipendente()
    {
        Console.WriteLine("Cerca il dipendente usando la sua mail aziendale:");
        // Lettura dell'input dell'utente
        var cercaMail = _view.GetInput();
        // Cerca il dipendente nel database usando la mail fornita
        var dipendente = _db.CercaDipendentePerMail(cercaMail);
        // Se il dipendente viene trovato
        if (dipendente != null)
        {
            // Crea una nuova tabella per visualizzare i dati del dipendente
            var table = new Table();
            table.AddColumn("Nome");
            table.AddColumn("Cognome");
            table.AddColumn("Data di Nascita");
            table.AddColumn("Mansione");
            table.AddColumn("Stipendio");
            table.AddColumn("Fatturato");
            table.AddColumn("Presenze");
            table.AddColumn("Email");

            // Aggiungi i dati del dipendente alla tabella come una nuova riga
            table.AddRow(
                dipendente.Nome,
                dipendente.Cognome,
                dipendente.DataDiNascita,
                dipendente.Mansione.Titolo,
                dipendente.Stipendio.ToString(),
                dipendente.Statistiche.Fatturato.ToString(),
                dipendente.Statistiche.Presenze.ToString(),
                dipendente.Mail
            );

            // Mostra la tabella con i dettagli del dipendente 
            AnsiConsole.Write(table);

        }
        else
        {
            // Messaggio se il dipendente non viene trovato
            Console.WriteLine("Dipendente non trovato con questa email.");
        }

    }

    // Il metodo MostraDipendenti visualizza a schermo l'elenco completo dei dipendenti con tutti i dati correlati
    private void MostraDipendenti()
    {
        var dipendenti = _db.GetUsers(); // Lettura di tutti i dipendenti dal database
        var table = new Table();
        // Aggiungere le colonne alla tabella
        table.AddColumn("Nome");
        table.AddColumn("Cognome");
        table.AddColumn("Data di Nascita");
        table.AddColumn("Mansione");
        table.AddColumn("Stipendio annuale");
        table.AddColumn("Fatturato");
        table.AddColumn("Presenze");
        table.AddColumn("Email aziendale");
        //  _view.MostraDipendenti(dipendenti); // Visualizzazione degli utenti

        // Itera attraverso ogni dipendente recuperato dal database
        foreach (var dipendente in dipendenti)
        {
            table.AddRow(
                dipendente.Nome,                            // Nome del dipendente
                dipendente.Cognome,                         // Cognome del dipendente
                dipendente.DataDiNascita,                   // Data di nascita
                dipendente.Mansione.Titolo,                 // Nome della mansione
                $"{dipendente.Stipendio}",                  // stipendio
                $"{dipendente.Statistiche.Fatturato}",      // Fatturato generato dal dipendente
                $"{dipendente.Statistiche.Presenze}",        // Giorni di presenza del dipendente
                dipendente.Mail                             // mail 
            );
        }


        AnsiConsole.Write(table);
    }

    // Il metodo TassoDiPresenza è utilizzato per calcolare e visualizzare il tasso di presenza dei dipendenti in percentuale 
    //rispetto al numero totale di giorni lavorativi in un anno (250 giorni).
    private void TassoDiPresenza()
    {
        Console.WriteLine("\nDi seguito l'elenco con il tasso di presenza per ogni dipendente su 250 giorni lavorativi equivalente ad 1 anno\n");
        int giorniLavorativiTotali = 250; // Numero di giorni lavorativi in un anno

        try
        {
            var dipendenti = _db.GetUsers(); // Ottieni la lista dei dipendenti dal database

            // Crea una tabella per visualizzare i risultati
            var table = new Table();
            table.AddColumn("Dipendente");
            table.AddColumn("Tasso di Presenza (%)");

            // Ordina i dipendenti dal tasso di presenza più alto al più basso
            dipendenti.Sort((y, x) => x.Statistiche.Presenze.CompareTo(y.Statistiche.Presenze));

            foreach (var dipendente in dipendenti)
            {
                int presenze = dipendente.Statistiche.Presenze;

                // Calcolo del tasso di presenza
                double tassoPresenza = ((double)presenze / giorniLavorativiTotali) * 100;
                tassoPresenza = Math.Round(tassoPresenza, 2); // Arrotonda il risultato a due cifre decimali

                // Aggiungi una riga alla tabella con il nome del dipendente e il tasso di presenza
                table.AddRow($"{dipendente.Nome} {dipendente.Cognome}", $"{tassoPresenza}%");
            }

            // Visualizza la tabella nella console
            AnsiConsole.Write(table);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Errore generale: {e.Message}");
        }
    }

    // metodo che divide in 2 gruppi i dipendenti in base al fatturato 
    //il primo gruppo sono i più performanti e l'ultimo gruppo i meno performanti
    // mostra anche il 15% dei dipendenti meno performanti
    private void ValutazioneFatturatoProdotto()
    {
        Console.WriteLine("\nDivide i dipendenti in 2 gruppi in base al fatturato prodotto");

        var dipendenti = _db.GetUsers(); // Ottiene la lista dei dipendenti dal database

        // Ordina i dipendenti per fatturato in ordine decrescente (dal più alto al più basso)
        dipendenti.Sort((x, y) => y.Statistiche.Fatturato.CompareTo(x.Statistiche.Fatturato));

        // Divide i dipendenti in due gruppi: i migliori e i peggiori
        int split = dipendenti.Count / 2;
        List<Dipendente> gruppoMigliori = dipendenti.GetRange(0, split);
        List<Dipendente> gruppoPeggiori = dipendenti.GetRange(split, dipendenti.Count - split);

        // Crea le tabelle per visualizzare i risultati
        var tableMigliori = new Table();
        tableMigliori.AddColumn("Dipendente");
        tableMigliori.AddColumn("Fatturato");

        var tablePeggiori = new Table();
        tablePeggiori.AddColumn("Dipendente");
        tablePeggiori.AddColumn("Fatturato");

        var tablePeggiori15 = new Table();
        tablePeggiori15.AddColumn("Dipendente");
        tablePeggiori15.AddColumn("Fatturato");

        // Aggiungi i dipendenti con fatturato più alto nella prima tabella
        foreach (var dipendente in gruppoMigliori)
        {
            tableMigliori.AddRow($"{dipendente.Nome} {dipendente.Cognome}", $"{dipendente.Statistiche.Fatturato}");
        }

        // Aggiungi i dipendenti con fatturato più basso nella seconda tabella
        foreach (var dipendente in gruppoPeggiori)
        {
            tablePeggiori.AddRow($"{dipendente.Nome} {dipendente.Cognome}", $"{dipendente.Statistiche.Fatturato}");
        }

        // Mostra la tabella dei dipendenti con fatturato più alto
        Console.WriteLine("\nGruppo con il fatturato più alto:\n");
        AnsiConsole.Write(tableMigliori);

        // Mostra la tabella dei dipendenti con fatturato più basso
        Console.WriteLine("\nGruppo con il fatturato più basso:\n");
        AnsiConsole.Write(tablePeggiori);

        // Ordina il gruppo con fatturato più basso per trovare il 15% delle performance peggiori
        gruppoPeggiori.Sort((x, y) => x.Statistiche.Fatturato.CompareTo(y.Statistiche.Fatturato));

        // Calcola il 15% dei dipendenti con fatturato più basso
        int index = (15 * gruppoPeggiori.Count) / 100;

        // Se il 15% è 0 ma ci sono dipendenti, mostra almeno un dipendente
        if (index == 0 && gruppoPeggiori.Count > 0)
        {
            index = 1;
        }

        // Aggiungi i dipendenti con il peggior 15% di fatturato alla terza tabella
        for (int i = 0; i < index; i++)
        {
            var dipendente = gruppoPeggiori[i];
            tablePeggiori15.AddRow($"{dipendente.Nome} {dipendente.Cognome}", $"{dipendente.Statistiche.Fatturato}");
        }

        // Mostra il 15% dei dipendenti con fatturato più basso
        Console.WriteLine("\nDi seguito il 15% delle performance peggiori per fatturato\n");
        AnsiConsole.Write(tablePeggiori15);
    }

    // metodo che mostra l'incidenza percentuale dello stipendi osul fatturato
    // il metodo è da costruire
    public void IncidenzaPercentuale()
    {
        /* Console.WriteLine("Calcolo dell'incidenza percentuale dello stipendio in rapporto al fatturato.");

         // Recupera i dipendenti dal database
         var dipendenti = _db.GetUsers();

         // Verifica se ci sono dipendenti nel database
         if (dipendenti.Count == 0)
         {
             Console.WriteLine("Non ci sono dipendenti nel sistema.");
             return;
         }

         // Chiedi l'inserimento del fatturato totale, potrebbe essere ottenuto anche da un altro metodo
         Console.WriteLine("Inserisci il fatturato totale dell'azienda:");
         double fatturatoTotale = Convert.ToDouble(Console.ReadLine());

         // Crea una tabella per visualizzare i dati
         var table = new Table();
         table.AddColumn("Nome");
         table.AddColumn("Cognome");
         table.AddColumn("Data di Nascita");
         table.AddColumn("Mansione");
         table.AddColumn("Stipendio");
         table.AddColumn("Incidenza sul fatturato (%)");
         table.AddColumn("Fatturato prodotto");
         table.AddColumn("Presenze");

         // Ordina i dipendenti per stipendio in ordine discendente
         dipendenti.Sort((x, y) => y.Stipendio.CompareTo(x.Stipendio));

         // Itera sui dipendenti e calcola l'incidenza percentuale
         foreach (var dipendente in dipendenti)
         {
             double stipendio = dipendente.Stipendio;

             // Calcola l'incidenza percentuale dello stipendio rispetto al fatturato totale
             double incidenza = (stipendio / fatturatoTotale) * 100;
             double incidenzaPercentuale = Math.Round(incidenza, 2); // Arrotonda a 2 cifre decimali

             // Aggiungi i dati del dipendente alla tabella
             table.AddRow(
                 dipendente.Nome,
                 dipendente.Cognome,
                 dipendente.DataDiNascita,
                 dipendente.Mansione.Titolo,
                 stipendio.ToString(),
                 $"{incidenzaPercentuale}%",
                 dipendente.Statistiche.Fatturato.ToString(),
                 dipendente.Statistiche.Presenze.ToString()
             );
         }

         // Mostra la tabella nella console
         AnsiConsole.Write(table); */
    }


    // metodo per aggiungere fatturato e presenze da assegnare al dipendente
    private void AggiungiIndicatoriDipendente()
    {
        Console.WriteLine("Elenco dei dipendenti:");

        // Recupera i dipendenti con ID
        var dipendentiConId = _db.GetUsers(); // Ottieni la lista dei dipendenti

        // Crea una tabella per visualizzare i dipendenti
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Nome");
        table.AddColumn("Cognome");
        table.AddColumn("Mansione");
        table.AddColumn("Stipendio");
        table.AddColumn("Fatturato");
        table.AddColumn("Presenze");

        // Aggiungi le righe con i dati dei dipendenti
        foreach (var dipendente in dipendentiConId)
        {
            table.AddRow(
                dipendente.Id.ToString(), // Visualizza l'ID del dipendente
                dipendente.Nome,
                dipendente.Cognome,
                dipendente.Mansione.Titolo,
                dipendente.Stipendio.ToString(),
                dipendente.Statistiche.Fatturato.ToString(),
                dipendente.Statistiche.Presenze.ToString()
            );
        }

        // Mostra la tabella
        AnsiConsole.Write(table);

        // Chiedi all'utente l'ID del dipendente
        Console.WriteLine("Inserisci l'ID del dipendente per aggiungere indicatori:");
        int dipendenteId = Convert.ToInt32(Console.ReadLine());

        // Chiedi i nuovi valori per fatturato e presenze
        Console.WriteLine("Inserisci il fatturato del dipendente:");
        double fatturato = Convert.ToDouble(Console.ReadLine());

        Console.WriteLine("Inserisci il numero di presenze del dipendente:");
        int presenze = Convert.ToInt32(Console.ReadLine());

        // Chiamata al metodo AggiungiIndicatori del Database
        _db.AggiungiIndicatori(dipendenteId, fatturato, presenze);

        Console.WriteLine("Indicatori aggiunti con successo.");
    }



    // da rivedere
    private void AggiornaIndicatoriDipendente()
    {
        // Mostra elenco dipendenti con ID
        Console.WriteLine("Elenco dei dipendenti:");
        var dipendentiConId = _db.GetDipendentiConId(); // Ottieni la lista dei dipendenti con ID
        foreach (var dipendente in dipendentiConId)
        {
            Console.WriteLine(dipendente); // Mostra ID e nome di ogni dipendente
        }

        // Richiedi l'ID del dipendente per il quale aggiornare gli indicatori
        Console.WriteLine("Inserisci l'ID del dipendente per aggiornare gli indicatori:");
        int dipendenteId = Convert.ToInt32(Console.ReadLine()); // Legge l'ID del dipendente

        // Chiedi i nuovi valori di fatturato e presenze
        Console.WriteLine("Inserisci il nuovo fatturato del dipendente:");
        double nuovoFatturato = Convert.ToDouble(Console.ReadLine()); // Legge il nuovo fatturato

        Console.WriteLine("Inserisci il nuovo numero di presenze del dipendente:");
        int nuovePresenze = Convert.ToInt32(Console.ReadLine()); // Legge il nuovo numero di presenze

        // Chiamata al metodo AggiornaIndicatori del Database
        _db.AggiornaIndicatori(dipendenteId, nuovoFatturato, nuovePresenze);

        // Conferma che gli indicatori sono stati aggiornati
        Console.WriteLine("Indicatori aggiornati con successo.");
    }

    // metodo che ordina nel terminale stipendi dal più alto al più basso
    private void OrdinaStipendi()
    {
        // Recupera i dipendenti dal database
        var dipendenti = _db.GetUsers();

        var table = new Table();

        table.AddColumn("Nome");
        table.AddColumn("Cognome");
        table.AddColumn("Stipendio");
        table.AddColumn("Mansione");
        table.AddColumn("Fatturato");
        table.AddColumn("Presenze");

        // Algoritmo bubble sort per ordinare i dipendenti in base allo stipendio in ordine discendente
        for (int i = 0; i < dipendenti.Count - 1; i++)
        {
            for (int j = 0; j < dipendenti.Count - i - 1; j++)
            {
                if (dipendenti[j].Stipendio < dipendenti[j + 1].Stipendio)
                {
                    // Scambia i dipendenti se lo stipendio del primo è inferiore a quello del successivo
                    var temp = dipendenti[j];
                    dipendenti[j] = dipendenti[j + 1];
                    dipendenti[j + 1] = temp;
                }
            }
        }

        foreach (var dipendente in dipendenti)
        {
            table.AddRow(
                dipendente.Nome,
                dipendente.Cognome,
                dipendente.Stipendio.ToString(),
                dipendente.Mansione.Titolo.ToString(),
                dipendente.Statistiche.Fatturato.ToString(),
                dipendente.Statistiche.Presenze.ToString()
            );
        }

        // Mostra la tabella ordinata
        Console.WriteLine("\nDipendenti ordinati per stipendio (dal più alto al più basso):\n");
        AnsiConsole.Write(table);
    }



    private void ModificaDipendente()
    {
        try
        {
            Console.WriteLine("Elenco dei dipendenti:");
            var dipendentiConId = _db.GetDipendentiConId();

            // Mostra l'elenco dei dipendenti con ID
            foreach (var dipendente in dipendentiConId)
            {
                Console.WriteLine(dipendente);
            }

            // Richiedi l'ID del dipendente
            Console.WriteLine("Inserisci l'ID del dipendente da modificare:");
            int dipendenteId = Convert.ToInt32(_view.GetInput());

            // Menu di selezione per i campi da modificare
            var inserimento = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("MODIFICA DIPENDENTE")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more)[/]")
                .AddChoices(new[] {
                "Cambia nome", "Cambia cognome", "Cambia data di nascita formato DD/MM/YYYY",
                "Cambia mansione", "Cambia stipendio", "Cambia fatturato", "Cambia giorni di presenza", "Cambia mail","Aggiorna indicatori", "Esci",
                }));

            string campoDaModificare = "";
            string nuovoValore = "";

            // Switch case per la modifica del campo selezionato
            switch (inserimento)
            {
                case "Cambia nome":
                    Console.WriteLine("Inserisci il nuovo nome");
                    campoDaModificare = "nome";
                    nuovoValore = Console.ReadLine().Trim();
                    break;

                case "Cambia cognome":
                    Console.WriteLine("Inserisci il nuovo cognome");
                    campoDaModificare = "cognome";
                    nuovoValore = Console.ReadLine().Trim();
                    break;

                case "Cambia data di nascita formato DD/MM/YYYY":
                    Console.WriteLine("Inserisci nuova data di nascita");
                    // Legge l'input dell'utente rimuove eventuali spazi vuoti e lo converte in un oggetto DateTime
                    // Utilizza il formato "dd/MM/yyyy" per garantire che la data sia inserita correttamente (giorno/mese/anno)
                    DateTime dataDiNascita = DateTime.ParseExact(Console.ReadLine().Trim(), "dd/MM/yyyy", null);
                    campoDaModificare = "dataDiNascita";
                    // Converte l'oggetto DateTime in una stringa formattata per essere compatibile con SQLite
                    nuovoValore = dataDiNascita.ToString("yyyy-MM-dd");  // Formattazione per SQLite
                    break;

                case "Cambia mansione":
                    Console.WriteLine("Inserisci la nuova mansione (ID):");
                    int mansioneId = Convert.ToInt32(Console.ReadLine().Trim());
                    campoDaModificare = "mansioneId";
                    nuovoValore = mansioneId.ToString();
                    break;

                case "Cambia stipendio":
                    Console.WriteLine("Inserisci il nuovo stipendio:");
                    campoDaModificare = "stipendio";
                    nuovoValore = Console.ReadLine().Trim();
                    break;

                case "Cambia fatturato":
                    Console.WriteLine("Inserisci il nuovo fatturato:");
                    campoDaModificare = "fatturato";
                    nuovoValore = Console.ReadLine().Trim();
                    break;

                case "Cambia giorni di presenza":
                    Console.WriteLine("Inserisci il numero di giorni di presenze:");
                    campoDaModificare = "presenze";
                    nuovoValore = Console.ReadLine().Trim();
                    break;

                case "Cambia mail":
                    Console.WriteLine("Inserisci il nuovo indirizzo email aziendale:");
                    campoDaModificare = "mail";
                    nuovoValore = Console.ReadLine().Trim();
                    break;



                case "Esci":
                    Console.WriteLine("\nL'applicazione si sta per chiudere\n");
                    return;

                default:
                    Console.WriteLine("\nScelta errata. Prego scegliere tra le opzioni disponibili\n");
                    return;
            }

            // Modifica il campo nel database
            bool successo = _db.ModificaDipendente(dipendenteId, campoDaModificare, nuovoValore);

            if (successo)
            {
                Console.WriteLine($"{campoDaModificare} aggiornato con successo per il dipendente con ID {dipendenteId}.");
            }
            else
            {
                Console.WriteLine("Errore durante la modifica del dipendente. Verifica l'ID o il campo inserito.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Errore non trattato: {e.Message}");
            Console.WriteLine($"CODICE ERRORE: {e.HResult}");
        }
    }




}

```


</details>

- Classe Persona -> dati anagrafici

<details>

<summary>Visualizza il codice class Persona</summary>

```c#

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


```


</details>

- Classe Dipendente -> eredita da classe Persona i dati anagrafici e aggiunge i campi  mansione, mail

<details>

<summary>Visualizza il codice class Dipendente</summary>

```c#

public class Dipendente : Persona
{
    public int Id { get; set; }
    public Mansione Mansione { get; set; }  // connette Dipendente a Mansione
    public string Mail { get; set; }
    public Statistiche Statistiche { get; set; } // connette Dipendente a Statistiche

    public Dipendente(int id,string nome, string cognome, string dataDiNascita, string mail, Mansione mansione, Statistiche statistiche = null)
        : base(nome, cognome, dataDiNascita) // Fa riferimento alla proprietà 'DataDiNascita' della classe base 'Persona'
    {
        this.Id =id;
        this.Mansione = mansione;
        this.Mail = mail;
        this.Statistiche = statistiche ?? new Statistiche(0, 0); // Imposta le statistiche
    }
}
```
</details>

- Classe Mansione -> include i campi titolo(della mansione) e stipendio

<details>

```c#

// classe che rappresenta la mansione di un dipendente con proprietà Id, Titolo e Stipendio
public class Mansione
{
    public int Id { get; set; }  // Proprietà 'Id' che rappresenta l'identificatore univoco della mansione
    public string Titolo { get; set; } // proprietà relativa al nome della mansione
    public double Stipendio { get; set; }    // proprietà relativa allo stipendio

    // Costruttore che accetta id titolo e stipendio
    // Questo costruttore viene usato quando abbiamo già un 'Id', probabilmente assegnato dal database
    public Mansione(int id, string titolo, double stipendio)
    {
        Id = id;
        Titolo = titolo;        // Assegna il titolo della mansione
        Stipendio = stipendio;  // Assegna lo stipendio della mansione
    }
    // Nuovo costruttore che accetta solo titolo e stipendio
    // Questo costruttore può essere usato quando l'Id non è necessario al momento della creazione
    public Mansione(string titolo, double stipendio)
    {
        Titolo = titolo;
        Stipendio = stipendio;
    }
}

```

</details>

- Classe Statistiche -> include i campi fatturato e presenze

<details>

```c#

// classe Statistiche con proprietà relative al fatturato e alle presenze del dipendente
public class Statistiche{
// Proprietà 'Id' per identificare in modo univoco ogni record di statistiche
    public int Id{ get; set; }

    // Proprietà per memorizzare il fatturato generato da un dipendente
    public double Fatturato{ get; set; }

 // Proprietà per memorizzare il numero di presenze di un dipendente
    public int Presenze{ get; set; }

// costruttore classe Statistiche
// Riceve i valori iniziali per 'Fatturato' e 'Presenze' e li assegna alle rispettive proprietà
    public Statistiche(double fatturato,int presenze){
        
        Fatturato = fatturato; // Assegna il valore di fatturato alla proprietà Fatturato
        Presenze = presenze;
    }
}

    ```

</details>

##IMPLEMENTAZIONE FUNZIONALITà

- Operazioni CRUD relative al dipendente

- Aggiungi dipendente -> Consente inserimento di un nuovo dipendente nel database
    
<details>


```c#

  private void AggiungiDipendente()
    {
        Console.WriteLine("Inserisci il nome:"); // Richiesta del nome dell'utente
        var nome = _view.GetInput(); // Lettura del nome dell'utente
         Console.WriteLine("Inserisci il cognome:"); 
        var cognome = _view.GetInput();
        Console.WriteLine("Inserisci la data di nascita DD/MM/YYYY:"); 
        var dataDiNascitaString = _view.GetInput();
         Console.WriteLine("Inserisci la mail aziendale:");
        var mail = _view.GetInput();

        var mansioni = _db.MostraMansioni();
    foreach (var mansione in mansioni)
    {
        // Stampa ogni mansione con il suo ID e altri dettagli
        Console.WriteLine($"ID: {mansione.Id}, Titolo: {mansione.Titolo}, Stipendio: {mansione.Stipendio}");
    }

         Console.WriteLine("Scegli tra le mansioni disponibili per id:");
        var mansioneInput = _view.GetInput();
         if (int.TryParse(mansioneInput, out int mansioneId))
    {
        var mansioneid = Console.ReadLine();

        if (DateTime.TryParseExact(dataDiNascitaString, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataDiNascita))
    {
        _db.AggiungiDipendente(nome, cognome, dataDiNascita,mail,mansioneId);
        Console.WriteLine("Dipendente aggiunto con successo."); // Aggiunta del dipendente al database
    }
    else
    {
        Console.WriteLine("Formato data di nascita non valido. Riprova.");
    }
   
    }else{
        Console.WriteLine("ID mansione non valido. Riprova.");
    }
    }


```
</details>
    - Mostra dipendenti -> Visualizza l'elenco di tutti i dipendenti con le informazioni dettagliate.

<details>

```c#
    private void MostraDipendenti()
    {
        var dipendenti = _db.GetUsers(); // Lettura degli utenti dal database
        var table = new Table();
         // Aggiungere le colonne alla tabella
    table.AddColumn("Nome");
    table.AddColumn("Cognome");
    table.AddColumn("Data di Nascita");
    table.AddColumn("Mansione");
    table.AddColumn("Stipendio annuale");
    table.AddColumn("Fatturato");
    table.AddColumn("Presenze");
    table.AddColumn("Email aziendale");
      //  _view.MostraDipendenti(dipendenti); // Visualizzazione degli utenti
      foreach (var dipendente in dipendenti)
    {
        table.AddRow(
            dipendente.Nome,
            dipendente.Cognome,
            dipendente.DataDiNascita,
            dipendente.Mansione.Titolo,
            $"{dipendente.Stipendio}",
            $"{dipendente.Statistiche.Fatturato}",
            $"{dipendente.Statistiche.Presenze}",
            dipendente.Mail
        );
    }

    // Visualizzare la tabella
    AnsiConsole.Write(table);
}

```

</details>

- Cerca dipendente -> Cerca un dipendente utilizzando l'email aziendale.

<details>

```c#

private void CercaDipendente(){
     Console.WriteLine("Cerca il dipendente usando la sua mail aziendale:");
      var cercaMail = _view.GetInput();
      var dipendente= _db.CercaDipendentePerMail(cercaMail);
      if(dipendente != null){

           var table = new Table();
        table.AddColumn("Nome");
        table.AddColumn("Cognome");
        table.AddColumn("Data di Nascita");
        table.AddColumn("Mansione");
        table.AddColumn("Stipendio");
        table.AddColumn("Fatturato");
        table.AddColumn("Presenze");
        table.AddColumn("Email");

        // Aggiungi i dati del dipendente nella tabella
        table.AddRow(
            dipendente.Nome,
            dipendente.Cognome,
            dipendente.DataDiNascita,
            dipendente.Mansione.Titolo,
            dipendente.Stipendio.ToString(),
            dipendente.Statistiche.Fatturato.ToString(),
            dipendente.Statistiche.Presenze.ToString(),
            dipendente.Mail
        );

        // Mostra la tabella
        AnsiConsole.Write(table);
       // Console.WriteLine("Dettagli del dipendente:");
       // Console.WriteLine(dipendente.ToString());
      }else{
        // Messaggio se il dipendente non viene trovato
        Console.WriteLine("Dipendente non trovato con questa email.");
      }

}


 ```


</details>

- Modifica Dipendente ->permette di modificare i dati di un dipendente, inclusi nome, cognome, data di nascita, mansione, stipendio, e-mail, e indicatori di performance.

   <details> 

   ```c#




   ```


   </details>
    
- Operazioni di calcolo e visualizzazione statistiche
    - Ordina Stipendi
    - Aggiungi indicatori
    - Tasso di presenza
    - Valutazione per fatturato
    - Incidenza percentuale

## VERSIONE SENZA ENTITY FRAMEWORK

<details>
<summary>View code</summary>

```c#

using System.Data.SQLite;
using Spectre.Console;

class Program
{
     // Metodo principale (entry point) del programma
    static void Main(string[] args)
    {
         // Inizializzazione del modello (Database)
        var db = new Database(); // Model
         // Inizializzazione della vista (View), che riceve il modello
        var view = new View(db); // View
        // Inizializzazione del controller, che collega il modello e la vista
        var controller = new Controller(db, view); // Controller
        controller.MainMenu(); // Menu principale dell'app

        
    }
}




using System.Data.SQLite;
using Spectre.Console;
// classe Controller funge da collegamento tra il Model Database e View
// gestice la logica dell'applicazione
class Controller
{
    private Database _db; // Riferimento al modello
    private View _view; // Riferimento alla vista

    // Costruttore della classe Controller
    public Controller(Database db, View view)
    {
        _db = db; // Inizializzazione del riferimento al modello
        _view = view; // Inizializzazione del riferimento alla vista
    }
    
  // Metodo principale che gestisce il menu tramite spectre console  richiamato dalla funzione MonstraMenuPrincipale
    public void MainMenu()
    {
        while (true)
        {
            // Chiede alla View di mostrare il menu e restituisce la scelta dell'utente
            var input = _view.MostraMenuPrincipale();

            // Esegui le operazioni in base alla scelta dell'utente
            if (input == "Aggiungi Dipendente")
            {
                AggiungiDipendente(); // Aggiunta di un utente
            }
            else if (input == "Mostra Dipendenti")
            {
                MostraDipendenti(); // Visualizzazione degli utenti
            }
            else if (input == "Rimuovi Dipendente")
            {
                RimuoviDipendente();    // Rimuove un dipendente
            }
            else if (input == "Cerca Dipendente")
            {
                CercaDipendente(); // Cerca un dipendente tramite email
            }
            else if (input == "Modifica Dipendente")
            {
                ModificaDipendente(); // Modifica un dipendente
            }
            else if (input == "Ordina stipendi")
            {
                OrdinaStipendi(); // Ordina i dipendenti per stipendio
            }
            else if (input == "Aggiungi indicatori")
            {
                AggiungiIndicatoriDipendente(); // Aggiungi indicatori fatturato e presenze a un dipendente
            }
            else if (input == "Tasso di presenza")
            {
                TassoDiPresenza(); // Calcola e visualizza il tasso di presenza in percentuale
            }
            else if (input == "Valutazione per fatturato")
            {
                ValutazioneFatturatoProdotto(); // Valutazione dei dipendenti per fatturato
            }
            else if (input == "Incidenza percentuale")
            {
                IncidenzaPercentuale(); // Da completare
            }
            else if (input == "Esci")
            {
                break; // Uscita dal programma
            }
        }
    }
 
// il metodo AggiungiDipendente gestisce l'intero processo di raccolta dei dati per un nuovo dipendente
// verifica la validità delle informazioni e inserisce il nuovo dipendente nel database del sistema.
    private void AggiungiDipendente()
    {
        Console.WriteLine("Inserisci il nome:"); // Richiesta del nome dell'utente
        var nome = _view.GetInput(); // Lettura del nome dell'utente
        Console.WriteLine("Inserisci il cognome:");
        var cognome = _view.GetInput();
        Console.WriteLine("Inserisci la data di nascita DD/MM/YYYY:");
        var dataDiNascitaString = _view.GetInput();
        Console.WriteLine("Inserisci la mail aziendale:");
        var mail = _view.GetInput();
         // Mostra le mansioni disponibili recuperate dal database per consentire all'utente di scegliere
        var mansioni = _db.MostraMansioni();
         // Ottiene la lista delle mansioni dal database
        foreach (var mansione in mansioni)
        {
            // Stampa ogni mansione con il suo ID e altri dettagli per visionare l'id di quale mansione  aggiungere
            Console.WriteLine($"ID: {mansione.Id}, Titolo: {mansione.Titolo}, Stipendio: {mansione.Stipendio}");
        }

        Console.WriteLine("Scegli tra le mansioni disponibili per id:");
        var mansioneInput = _view.GetInput();
        // Verifica se l'input della mansione è un intero valido
        if (int.TryParse(mansioneInput, out int mansioneId))
        {
            var mansioneid = Console.ReadLine();
            // Verifica se la data di nascita inserita è valida nel formato "dd/MM/yyyy"
            if (DateTime.TryParseExact(dataDiNascitaString, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataDiNascita))
            {
                // Aggiunta del dipendente nel database con nome, cognome, data di nascita, email e ID della mansione
                _db.AggiungiDipendente(nome, cognome, dataDiNascita, mail, mansioneId);
                Console.WriteLine("Dipendente aggiunto con successo."); // Aggiunta del dipendente al database
            }
            else
            {
                Console.WriteLine("Formato data di nascita non valido. Riprova.");
            }

        }
        else
        {
            Console.WriteLine("ID mansione non valido. Riprova.");
        }
    }

// il metodo RimuoviDipendente gestisce il processo di eliminazione di un dipendente dal database
//fornendo all'utente un elenco dei dipendenti verificando l'input e rimuovendo il dipendente selezionato in modo sicuro
    private void RimuoviDipendente()
    {
        // Mostra l'elenco dei dipendenti con il loro ID, nome e cognome
        Console.WriteLine("Elenco dei dipendenti:");
        var dipendentiConId = _db.GetDipendentiConId();
        // Itera attraverso i dipendenti e visualizza le loro informazioni
        foreach (var dipendente in dipendentiConId)
        {
            Console.WriteLine(dipendente);
        }
        Console.WriteLine("Inserisci l'ID del dipendente da rimuovere:");
        try
        {
            // Usa Convert.ToInt32 per convertire l'input dell'utnete (ID) in un intero
            var dipendenteId = Convert.ToInt32(Console.ReadLine());

            // Prova a rimuovere il dipendente con l'ID specificato
            bool successo = _db.RimuoviDipendente(dipendenteId);
            if (successo)
            {
                Console.WriteLine("Dipendente rimosso con successo.");
            }
            else
            {
                Console.WriteLine("Dipendente non trovato o ID non valido.");
            }
        }
        catch (FormatException)
        {
            // Gestisce l'errore se l'input non è un numero valido
            Console.WriteLine("ID non valido. Inserisci un numero.");
        }

    }

    //Il metodo CercaDipendente consente di cercare un dipendente nel database
    // utilizzando la sua email aziendale come chiave di ricerca.ad esempio nome.cognome@gmail.com
    private void CercaDipendente()
    {
        Console.WriteLine("Cerca il dipendente usando la sua mail aziendale:");
        // Lettura dell'input dell'utente
        var cercaMail = _view.GetInput();
        // Cerca il dipendente nel database usando la mail fornita
        var dipendente = _db.CercaDipendentePerMail(cercaMail);
        // Se il dipendente viene trovato
        if (dipendente != null)
        {
            // Crea una nuova tabella per visualizzare i dati del dipendente
            var table = new Table();
            table.AddColumn("Nome");
            table.AddColumn("Cognome");
            table.AddColumn("Data di Nascita");
            table.AddColumn("Mansione");
            table.AddColumn("Stipendio");
            table.AddColumn("Fatturato");
            table.AddColumn("Presenze");
            table.AddColumn("Email");

            // Aggiungi i dati del dipendente alla tabella come una nuova riga
            table.AddRow(
                dipendente.Nome,
                dipendente.Cognome,
                dipendente.DataDiNascita,
                dipendente.Mansione.Titolo,
                dipendente.Stipendio.ToString(),
                dipendente.Statistiche.Fatturato.ToString(),
                dipendente.Statistiche.Presenze.ToString(),
                dipendente.Mail
            );

            // Mostra la tabella con i dettagli del dipendente 
            AnsiConsole.Write(table);

        }
        else
        {
            // Messaggio se il dipendente non viene trovato
            Console.WriteLine("Dipendente non trovato con questa email.");
        }

    }

    // Il metodo MostraDipendenti visualizza a schermo l'elenco completo dei dipendenti con tutti i dati correlati
    private void MostraDipendenti()
    {
        var dipendenti = _db.GetUsers(); // Lettura di tutti i dipendenti dal database
        var table = new Table();
        // Aggiungere le colonne alla tabella
        table.AddColumn("Nome");
        table.AddColumn("Cognome");
        table.AddColumn("Data di Nascita");
        table.AddColumn("Mansione");
        table.AddColumn("Stipendio annuale");
        table.AddColumn("Fatturato");
        table.AddColumn("Presenze");
        table.AddColumn("Email aziendale");
        //  _view.MostraDipendenti(dipendenti); // Visualizzazione degli utenti

        // Itera attraverso ogni dipendente recuperato dal database
        foreach (var dipendente in dipendenti)
        {
            table.AddRow(
                dipendente.Nome,                            // Nome del dipendente
                dipendente.Cognome,                         // Cognome del dipendente
                dipendente.DataDiNascita,                   // Data di nascita
                dipendente.Mansione.Titolo,                 // Nome della mansione
                $"{dipendente.Stipendio}",                  // stipendio
                $"{dipendente.Statistiche.Fatturato}",      // Fatturato generato dal dipendente
                $"{dipendente.Statistiche.Presenze}",        // Giorni di presenza del dipendente
                dipendente.Mail                             // mail 
            );
        }


        AnsiConsole.Write(table);
    }

    // Il metodo TassoDiPresenza è utilizzato per calcolare e visualizzare il tasso di presenza dei dipendenti in percentuale 
    //rispetto al numero totale di giorni lavorativi in un anno (250 giorni).
    private void TassoDiPresenza()
    {
        Console.WriteLine("\nDi seguito l'elenco con il tasso di presenza per ogni dipendente su 250 giorni lavorativi equivalente ad 1 anno\n");
        int giorniLavorativiTotali = 250; // Numero di giorni lavorativi in un anno

        try
        {
            var dipendenti = _db.GetUsers(); // Ottieni la lista dei dipendenti dal database

            // Crea una tabella per visualizzare i risultati
            var table = new Table();
            table.AddColumn("Dipendente");
            table.AddColumn("Tasso di Presenza (%)");

            // Ordina i dipendenti dal tasso di presenza più alto al più basso
            dipendenti.Sort((y, x) => x.Statistiche.Presenze.CompareTo(y.Statistiche.Presenze));

            foreach (var dipendente in dipendenti)
            {
                int presenze = dipendente.Statistiche.Presenze;

                // Calcolo del tasso di presenza
                double tassoPresenza = ((double)presenze / giorniLavorativiTotali) * 100;
                tassoPresenza = Math.Round(tassoPresenza, 2); // Arrotonda il risultato a due cifre decimali

                // Aggiungi una riga alla tabella con il nome del dipendente e il tasso di presenza
                table.AddRow($"{dipendente.Nome} {dipendente.Cognome}", $"{tassoPresenza}%");
            }

            // Visualizza la tabella nella console
            AnsiConsole.Write(table);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Errore generale: {e.Message}");
        }
    }

    // metodo che divide in 2 gruppi i dipendenti in base al fatturato 
    //il primo gruppo sono i più performanti e l'ultimo gruppo i meno performanti
    // mostra anche il 15% dei dipendenti meno performanti
    private void ValutazioneFatturatoProdotto()
    {
        Console.WriteLine("\nDivide i dipendenti in 2 gruppi in base al fatturato prodotto");

        var dipendenti = _db.GetUsers(); // Ottiene la lista dei dipendenti dal database

        // Ordina i dipendenti per fatturato in ordine decrescente (dal più alto al più basso)
        dipendenti.Sort((x, y) => y.Statistiche.Fatturato.CompareTo(x.Statistiche.Fatturato));

        // Divide i dipendenti in due gruppi: i migliori e i peggiori
        int split = dipendenti.Count / 2;
        List<Dipendente> gruppoMigliori = dipendenti.GetRange(0, split);
        List<Dipendente> gruppoPeggiori = dipendenti.GetRange(split, dipendenti.Count - split);

        // Crea le tabelle per visualizzare i risultati
        var tableMigliori = new Table();
        tableMigliori.AddColumn("Dipendente");
        tableMigliori.AddColumn("Fatturato");

        var tablePeggiori = new Table();
        tablePeggiori.AddColumn("Dipendente");
        tablePeggiori.AddColumn("Fatturato");

        var tablePeggiori15 = new Table();
        tablePeggiori15.AddColumn("Dipendente");
        tablePeggiori15.AddColumn("Fatturato");

        // Aggiungi i dipendenti con fatturato più alto nella prima tabella
        foreach (var dipendente in gruppoMigliori)
        {
            tableMigliori.AddRow($"{dipendente.Nome} {dipendente.Cognome}", $"{dipendente.Statistiche.Fatturato}");
        }

        // Aggiungi i dipendenti con fatturato più basso nella seconda tabella
        foreach (var dipendente in gruppoPeggiori)
        {
            tablePeggiori.AddRow($"{dipendente.Nome} {dipendente.Cognome}", $"{dipendente.Statistiche.Fatturato}");
        }

        // Mostra la tabella dei dipendenti con fatturato più alto
        Console.WriteLine("\nGruppo con il fatturato più alto:\n");
        AnsiConsole.Write(tableMigliori);

        // Mostra la tabella dei dipendenti con fatturato più basso
        Console.WriteLine("\nGruppo con il fatturato più basso:\n");
        AnsiConsole.Write(tablePeggiori);

        // Ordina il gruppo con fatturato più basso per trovare il 15% delle performance peggiori
        gruppoPeggiori.Sort((x, y) => x.Statistiche.Fatturato.CompareTo(y.Statistiche.Fatturato));

        // Calcola il 15% dei dipendenti con fatturato più basso
        int index = (15 * gruppoPeggiori.Count) / 100;

        // Se il 15% è 0 ma ci sono dipendenti, mostra almeno un dipendente
        if (index == 0 && gruppoPeggiori.Count > 0)
        {
            index = 1;
        }

        // Aggiungi i dipendenti con il peggior 15% di fatturato alla terza tabella
        for (int i = 0; i < index; i++)
        {
            var dipendente = gruppoPeggiori[i];
            tablePeggiori15.AddRow($"{dipendente.Nome} {dipendente.Cognome}", $"{dipendente.Statistiche.Fatturato}");
        }

        // Mostra il 15% dei dipendenti con fatturato più basso
        Console.WriteLine("\nDi seguito il 15% delle performance peggiori per fatturato\n");
        AnsiConsole.Write(tablePeggiori15);
    }

    // metodo che mostra l'incidenza percentuale dello stipendi osul fatturato
    // il metodo è da costruire
    public void IncidenzaPercentuale()
    {
        /* Console.WriteLine("Calcolo dell'incidenza percentuale dello stipendio in rapporto al fatturato.");

         // Recupera i dipendenti dal database
         var dipendenti = _db.GetUsers();

         // Verifica se ci sono dipendenti nel database
         if (dipendenti.Count == 0)
         {
             Console.WriteLine("Non ci sono dipendenti nel sistema.");
             return;
         }

         // Chiedi l'inserimento del fatturato totale, potrebbe essere ottenuto anche da un altro metodo
         Console.WriteLine("Inserisci il fatturato totale dell'azienda:");
         double fatturatoTotale = Convert.ToDouble(Console.ReadLine());

         // Crea una tabella per visualizzare i dati
         var table = new Table();
         table.AddColumn("Nome");
         table.AddColumn("Cognome");
         table.AddColumn("Data di Nascita");
         table.AddColumn("Mansione");
         table.AddColumn("Stipendio");
         table.AddColumn("Incidenza sul fatturato (%)");
         table.AddColumn("Fatturato prodotto");
         table.AddColumn("Presenze");

         // Ordina i dipendenti per stipendio in ordine discendente
         dipendenti.Sort((x, y) => y.Stipendio.CompareTo(x.Stipendio));

         // Itera sui dipendenti e calcola l'incidenza percentuale
         foreach (var dipendente in dipendenti)
         {
             double stipendio = dipendente.Stipendio;

             // Calcola l'incidenza percentuale dello stipendio rispetto al fatturato totale
             double incidenza = (stipendio / fatturatoTotale) * 100;
             double incidenzaPercentuale = Math.Round(incidenza, 2); // Arrotonda a 2 cifre decimali

             // Aggiungi i dati del dipendente alla tabella
             table.AddRow(
                 dipendente.Nome,
                 dipendente.Cognome,
                 dipendente.DataDiNascita,
                 dipendente.Mansione.Titolo,
                 stipendio.ToString(),
                 $"{incidenzaPercentuale}%",
                 dipendente.Statistiche.Fatturato.ToString(),
                 dipendente.Statistiche.Presenze.ToString()
             );
         }

         // Mostra la tabella nella console
         AnsiConsole.Write(table); */
    }


    // metodo per aggiungere fatturato e presenze da assegnare al dipendente
    private void AggiungiIndicatoriDipendente()
    {
        Console.WriteLine("Elenco dei dipendenti:");

        // Recupera i dipendenti con ID
        var dipendentiConId = _db.GetUsers(); // Ottieni la lista dei dipendenti

        // Crea una tabella per visualizzare i dipendenti
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Nome");
        table.AddColumn("Cognome");
        table.AddColumn("Mansione");
        table.AddColumn("Stipendio");
        table.AddColumn("Fatturato");
        table.AddColumn("Presenze");

        // Aggiungi le righe con i dati dei dipendenti
        foreach (var dipendente in dipendentiConId)
        {
            table.AddRow(
                dipendente.Id.ToString(), // Visualizza l'ID del dipendente
                dipendente.Nome,
                dipendente.Cognome,
                dipendente.Mansione.Titolo,
                dipendente.Stipendio.ToString(),
                dipendente.Statistiche.Fatturato.ToString(),
                dipendente.Statistiche.Presenze.ToString()
            );
        }

        // Mostra la tabella
        AnsiConsole.Write(table);

        // Chiedi all'utente l'ID del dipendente
        Console.WriteLine("Inserisci l'ID del dipendente per aggiungere indicatori:");
        int dipendenteId = Convert.ToInt32(Console.ReadLine());

        // Chiedi i nuovi valori per fatturato e presenze
        Console.WriteLine("Inserisci il fatturato del dipendente:");
        double fatturato = Convert.ToDouble(Console.ReadLine());

        Console.WriteLine("Inserisci il numero di presenze del dipendente:");
        int presenze = Convert.ToInt32(Console.ReadLine());

        // Chiamata al metodo AggiungiIndicatori del Database
        _db.AggiungiIndicatori(dipendenteId, fatturato, presenze);

        Console.WriteLine("Indicatori aggiunti con successo.");
    }



    // da rivedere
    private void AggiornaIndicatoriDipendente()
    {
        // Mostra elenco dipendenti con ID
        Console.WriteLine("Elenco dei dipendenti:");
        var dipendentiConId = _db.GetDipendentiConId(); // Ottieni la lista dei dipendenti con ID
        foreach (var dipendente in dipendentiConId)
        {
            Console.WriteLine(dipendente); // Mostra ID e nome di ogni dipendente
        }

        // Richiedi l'ID del dipendente per il quale aggiornare gli indicatori
        Console.WriteLine("Inserisci l'ID del dipendente per aggiornare gli indicatori:");
        int dipendenteId = Convert.ToInt32(Console.ReadLine()); // Legge l'ID del dipendente

        // Chiedi i nuovi valori di fatturato e presenze
        Console.WriteLine("Inserisci il nuovo fatturato del dipendente:");
        double nuovoFatturato = Convert.ToDouble(Console.ReadLine()); // Legge il nuovo fatturato

        Console.WriteLine("Inserisci il nuovo numero di presenze del dipendente:");
        int nuovePresenze = Convert.ToInt32(Console.ReadLine()); // Legge il nuovo numero di presenze

        // Chiamata al metodo AggiornaIndicatori del Database
        _db.AggiornaIndicatori(dipendenteId, nuovoFatturato, nuovePresenze);

        // Conferma che gli indicatori sono stati aggiornati
        Console.WriteLine("Indicatori aggiornati con successo.");
    }

    // metodo che ordina nel terminale stipendi dal più alto al più basso
    private void OrdinaStipendi()
    {
        // Recupera i dipendenti dal database
        var dipendenti = _db.GetUsers();

        var table = new Table();

        table.AddColumn("Nome");
        table.AddColumn("Cognome");
        table.AddColumn("Stipendio");
        table.AddColumn("Mansione");
        table.AddColumn("Fatturato");
        table.AddColumn("Presenze");

        // Algoritmo bubble sort per ordinare i dipendenti in base allo stipendio in ordine discendente
        for (int i = 0; i < dipendenti.Count - 1; i++)
        {
            for (int j = 0; j < dipendenti.Count - i - 1; j++)
            {
                if (dipendenti[j].Stipendio < dipendenti[j + 1].Stipendio)
                {
                    // Scambia i dipendenti se lo stipendio del primo è inferiore a quello del successivo
                    var temp = dipendenti[j];
                    dipendenti[j] = dipendenti[j + 1];
                    dipendenti[j + 1] = temp;
                }
            }
        }

        foreach (var dipendente in dipendenti)
        {
            table.AddRow(
                dipendente.Nome,
                dipendente.Cognome,
                dipendente.Stipendio.ToString(),
                dipendente.Mansione.Titolo.ToString(),
                dipendente.Statistiche.Fatturato.ToString(),
                dipendente.Statistiche.Presenze.ToString()
            );
        }

        // Mostra la tabella ordinata
        Console.WriteLine("\nDipendenti ordinati per stipendio (dal più alto al più basso):\n");
        AnsiConsole.Write(table);
    }



    private void ModificaDipendente()
    {
        try
        {
            Console.WriteLine("Elenco dei dipendenti:");
            var dipendentiConId = _db.GetDipendentiConId();

            // Mostra l'elenco dei dipendenti con ID
            foreach (var dipendente in dipendentiConId)
            {
                Console.WriteLine(dipendente);
            }

            // Richiedi l'ID del dipendente
            Console.WriteLine("Inserisci l'ID del dipendente da modificare:");
            int dipendenteId = Convert.ToInt32(_view.GetInput());

            // Menu di selezione per i campi da modificare
            var inserimento = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("MODIFICA DIPENDENTE")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more)[/]")
                .AddChoices(new[] {
                "Cambia nome", "Cambia cognome", "Cambia data di nascita formato DD/MM/YYYY",
                "Cambia mansione", "Cambia stipendio", "Cambia fatturato", "Cambia giorni di presenza", "Cambia mail","Aggiorna indicatori", "Esci",
                }));

            string campoDaModificare = "";
            string nuovoValore = "";

            // Switch case per la modifica del campo selezionato
            switch (inserimento)
            {
                case "Cambia nome":
                    Console.WriteLine("Inserisci il nuovo nome");
                    campoDaModificare = "nome";
                    nuovoValore = Console.ReadLine().Trim();
                    break;

                case "Cambia cognome":
                    Console.WriteLine("Inserisci il nuovo cognome");
                    campoDaModificare = "cognome";
                    nuovoValore = Console.ReadLine().Trim();
                    break;

                case "Cambia data di nascita formato DD/MM/YYYY":
                    Console.WriteLine("Inserisci nuova data di nascita");
                    // Legge l'input dell'utente rimuove eventuali spazi vuoti e lo converte in un oggetto DateTime
                    // Utilizza il formato "dd/MM/yyyy" per garantire che la data sia inserita correttamente (giorno/mese/anno)
                    DateTime dataDiNascita = DateTime.ParseExact(Console.ReadLine().Trim(), "dd/MM/yyyy", null);
                    campoDaModificare = "dataDiNascita";
                    // Converte l'oggetto DateTime in una stringa formattata per essere compatibile con SQLite
                    nuovoValore = dataDiNascita.ToString("yyyy-MM-dd");  // Formattazione per SQLite
                    break;

                case "Cambia mansione":
                    Console.WriteLine("Inserisci la nuova mansione (ID):");
                    int mansioneId = Convert.ToInt32(Console.ReadLine().Trim());
                    campoDaModificare = "mansioneId";
                    nuovoValore = mansioneId.ToString();
                    break;

                case "Cambia stipendio":
                    Console.WriteLine("Inserisci il nuovo stipendio:");
                    campoDaModificare = "stipendio";
                    nuovoValore = Console.ReadLine().Trim();
                    break;

                case "Cambia fatturato":
                    Console.WriteLine("Inserisci il nuovo fatturato:");
                    campoDaModificare = "fatturato";
                    nuovoValore = Console.ReadLine().Trim();
                    break;

                case "Cambia giorni di presenza":
                    Console.WriteLine("Inserisci il numero di giorni di presenze:");
                    campoDaModificare = "presenze";
                    nuovoValore = Console.ReadLine().Trim();
                    break;

                case "Cambia mail":
                    Console.WriteLine("Inserisci il nuovo indirizzo email aziendale:");
                    campoDaModificare = "mail";
                    nuovoValore = Console.ReadLine().Trim();
                    break;



                case "Esci":
                    Console.WriteLine("\nL'applicazione si sta per chiudere\n");
                    return;

                default:
                    Console.WriteLine("\nScelta errata. Prego scegliere tra le opzioni disponibili\n");
                    return;
            }

            // Modifica il campo nel database
            bool successo = _db.ModificaDipendente(dipendenteId, campoDaModificare, nuovoValore);

            if (successo)
            {
                Console.WriteLine($"{campoDaModificare} aggiornato con successo per il dipendente con ID {dipendenteId}.");
            }
            else
            {
                Console.WriteLine("Errore durante la modifica del dipendente. Verifica l'ID o il campo inserito.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Errore non trattato: {e.Message}");
            Console.WriteLine($"CODICE ERRORE: {e.HResult}");
        }
    }




}

// importazione librerie per Sqlite per la gestone del database e Spectre Console per la visualizzazione in console
using System.Data.SQLite;
using Spectre.Console;

// creazione classe Database per creare e gestire il database del gestionale tramite operazioni CRUD 
class Database
{
    private SQLiteConnection _connection; // SQLiteConnection è una classe che rappresenta una connessione a un database SQLite si definisce classe



    // costruttore della classe Database  si occupa di aprire la connessione e creare le tabelle se non esistono
    public Database()
    {
        _connection = new SQLiteConnection("Data Source=database.db"); // Creazione di una connessione al database
        _connection.Open(); // Apertura della connessione

        // creazione della tabella principale chiamata dipendente se non esiste
        var command1 = new SQLiteCommand(
          "CREATE TABLE IF NOT EXISTS dipendente (id INTEGER PRIMARY KEY, nome TEXT, cognome TEXT, dataDiNascita DATE, mail TEXT,  mansioneId INTEGER,indicatoriId INTEGER,  FOREIGN KEY(mansioneId) REFERENCES mansione(id),FOREIGN KEY(indicatoriId) REFERENCES indicatori(id)) ;",
          _connection);
        command1.ExecuteNonQuery(); // Esecuzione del comando

        // Creazione della tabella mansione collegata alla tabella dipendente
        var command2 = new SQLiteCommand(
            "CREATE TABLE IF NOT EXISTS mansione (id INTEGER PRIMARY KEY AUTOINCREMENT, titolo TEXT UNIQUE,stipendio REAL);",
            _connection);
        command2.ExecuteNonQuery(); // Esecuzione del comando

        // creazione della tabella indicatori collegata alla tabella dipendente
        var command3 = new SQLiteCommand(
            "CREATE TABLE IF NOT EXISTS indicatori (id INTEGER PRIMARY KEY AUTOINCREMENT, fatturato REAL,presenze INTEGER);",
            _connection);
        command3.ExecuteNonQuery(); // Esecuzione del comando
        AggiungiMansioniPredefinite();  // metodo che aggiunge delle mansioni di default nella tabella mansione
    }

    // metodo AggiungiDipendente che permette l'aggiunta del dipendente nel database 
    //mettendo come input i valori richiesti  nome, cognome, data di nascita, email e ID della mansione
    
    public void AggiungiDipendente(string nome, string cognome, DateTime dataDiNascita, string mail, int mansioneId)
    {
        var command = new SQLiteCommand($"INSERT INTO dipendente (nome,cognome,dataDiNascita,mail,mansioneId) VALUES (@nome,@cognome,@dataDiNascita,@mail,@mansioneId)", _connection); // Creazione di un comando per inserire un nuovo utente
        command.Parameters.AddWithValue("@nome", nome);
        command.Parameters.AddWithValue("@cognome", cognome);
        command.Parameters.AddWithValue("@dataDiNascita", dataDiNascita.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("@mail", mail);
        command.Parameters.AddWithValue("@mansioneId", mansioneId);
        command.ExecuteNonQuery(); // Esecuzione del comando
    }

    // metodo AggiungiMansione che permette l'aggiunta della mansione alla tabella mansione con valori titolo e stipendio
    public int AggiungiMansione(Mansione mansione)
    {
        // Il comando SELECT last_insert_rowid():  seleziona l'ID della riga che è stata appena inserita
        var command = new SQLiteCommand("INSERT INTO mansione(titolo,stipendio) VALUES (@titolo,@stipendio);SELECT last_insert_rowid()", _connection);
        command.Parameters.AddWithValue("@titolo", mansione.Titolo);
        command.Parameters.AddWithValue("@stipendio", mansione.Stipendio);


        //esegue la query SQL e restituisce il valore della prima colonna della prima riga del risultato (in questo caso, l'ID appena inserito).
        //Convert.ToInt32(...) converte il risultato (che è di tipo object) in un tipo int.

        return Convert.ToInt32(command.ExecuteScalar());


    }

    // metodo AggiungiIndicatori che aggiunge valori del fatturato e delle presenze del dipendente nel database

    public void AggiungiIndicatori(int dipendenteId, double fatturato, int presenze)
    {
        // Creare un nuovo record nella tabella `indicatori` che memorizza il fatturato e le presenze del dipendente
        var commandIndicatori = new SQLiteCommand("INSERT INTO indicatori (fatturato, presenze) VALUES (@fatturato, @presenze); SELECT last_insert_rowid();", _connection);
        // Aggiunge i valori di fatturato e presenze come parametri per evitare SQL injection
        commandIndicatori.Parameters.AddWithValue("@fatturato", fatturato);
        commandIndicatori.Parameters.AddWithValue("@presenze", presenze);
        // Esegue l'inserimento e ottiene l'ID del nuovo record `indicatori` appena inserito.
        int indicatoriId = Convert.ToInt32(commandIndicatori.ExecuteScalar());

        // Aggiorna il record del dipendente associando il nuovo `indicatoriId` al campo `indicatoriId` nella tabella `dipendente`
        var commandDipendente = new SQLiteCommand("UPDATE dipendente SET indicatoriId = @indicatoriId WHERE id = @dipendenteId", _connection);
        commandDipendente.Parameters.AddWithValue("@indicatoriId", indicatoriId);
        commandDipendente.Parameters.AddWithValue("@dipendenteId", dipendenteId);
        commandDipendente.ExecuteNonQuery(); // Esegui l'aggiornamento
    }


    // Metodo AggiornaIndicatori che aggiorna i valori di fatturato e presenze di un dipendente specifico
    public void AggiornaIndicatori(int dipendenteId, double nuovoFatturato, int nuovePresenze)
    {
        // Aggiorna il record nella tabella `indicatori` collegato al dipendente
        // Si seleziona l'`indicatoriId` dalla tabella `dipendente`, corrispondente al dipendente con `dipendenteId` fornito
        var command = new SQLiteCommand("UPDATE indicatori SET fatturato = @fatturato, presenze = @presenze WHERE id = (SELECT indicatoriId FROM dipendente WHERE id = @dipendenteId)", _connection);
        command.Parameters.AddWithValue("@fatturato", nuovoFatturato);
        command.Parameters.AddWithValue("@presenze", nuovePresenze);
        command.Parameters.AddWithValue("@dipendenteId", dipendenteId);
        command.ExecuteNonQuery(); // Esegui l'aggiornamento
    }

    // metodo AggiungiMansioniPredefinite inserisce delle mansioni di default al database nella tabella mansione
    //Ogni mansione è un oggetto della classe Mansione  con valori predefiniti per titolo e stipendio

    private void AggiungiMansioniPredefinite()
    {
        // Crea una lista di mansioni di tipo Mansione predefinite con titolo e stipendio
        var mansioni = new List<Mansione>{
            new Mansione("impiegato",20000),
             new Mansione("programmatore",25000),
              new Mansione("dirigente",70000),
              new Mansione("receptionist",15000),
               new Mansione("general manager",120000),
                new Mansione("ceo",4000000)
        };
        // Itera attraverso ciascuna mansione predefinita 
        foreach (var mansione in mansioni)
        {

            // Crea un comando SQL per verificare se una mansione con lo stesso titolo esiste già nel database
            // SELECT COUNT(*) conta quante righe nella tabella `mansione` hanno un valore nel campo `titolo`
            var checkCommand = new SQLiteCommand("SELECT COUNT(*) FROM mansione WHERE titolo = @titolo", _connection);
            checkCommand.Parameters.AddWithValue("@titolo", mansione.Titolo);  // Aggiunge il titolo della mansione come parametro
                                                                               // Esegue la query e restituisce il conteggio delle righe con lo stesso titolo di mansione
                                                                               // Se il conteggio restituito è maggiore di 0, significa che esiste già una mansione con quel titolo e quindi non verrà aggiunta al database
            var count = Convert.ToInt32(checkCommand.ExecuteScalar());
            // Se il titolo della mansione non esiste nel database (count == 0), la mansione viene aggiunta

            if (count == 0)
            {
                AggiungiMansione(mansione);
            }

        }
    }

    // Metodo RimuoviDipendente per eliminare un dipendente dal database usando il suo ID
    public bool RimuoviDipendente(int dipendenteId)
    {
        var command = new SQLiteCommand("DELETE from dipendente WHERE id= @id", _connection);
        // Aggiunge il parametro `@id` al comando SQL e lo imposta come l'ID del dipendente che si vuole rimuovere
        command.Parameters.AddWithValue("@id", dipendenteId);
        // Esegue il comando di eliminazione e restituisce il numero di righe interessate
        int affectedRows = command.ExecuteNonQuery();
        // Restituisce `true` se almeno una riga è stata eliminata, altrimenti `false`
        return affectedRows > 0;

    }

    // Il metodo GetDipendentiConId ha il compito di recuperare l'ID, il nome e il cognome di tutti i dipendenti nel database e restituirli in una lista di stringhe.
    public List<string> GetDipendentiConId()
    {
        // Crea un comando SQL per selezionare l'ID, il nome e il cognome di tutti i dipendenti

        var command = new SQLiteCommand("SELECT id, nome, cognome FROM dipendente", _connection);
        var reader = command.ExecuteReader();

        // Crea una lista di stringhe per memorizzare i risultati (ID, Nome, Cognome dei dipendenti)
        var dipendenti = new List<string>();

        // Cicla attraverso i risultati letti dal database

        while (reader.Read())
        {
            // Crea una stringa con l'ID, il nome e il cognome del dipendente
            string info = $"ID: {reader.GetInt32(0)}, Nome: {reader.GetString(1)}, Cognome: {reader.GetString(2)}";
            // Aggiunge la stringa alla lista 'dipendenti'
            dipendenti.Add(info);
        }
        //restituisce lista dipendenti
        return dipendenti;
    }

    // Metodo GetUsers permette la lettura dei dati dei dipendenti dal database SQLite 
    //aggrega questi dati in oggetti Dipendente infine restituisce una lista di questi oggetti. 
    public List<Dipendente> GetUsers()
    {
        var command = new SQLiteCommand(
            "SELECT dipendente.id, dipendente.nome, dipendente.cognome, strftime('%d/%m/%Y', dataDiNascita) AS data_formattata, dipendente.mail, mansione.titolo, mansione.stipendio, indicatori.fatturato, indicatori.presenze " +
            "FROM dipendente " +
            "JOIN mansione ON dipendente.mansioneId = mansione.id " +           //JOIN  collega la tabella mansione alla tabella dipendente
            "LEFT JOIN indicatori ON dipendente.indicatoriId = indicatori.id;", //Left Join permette di includere anche valori nulli negli indicatori
            _connection);

        var reader = command.ExecuteReader();
        // Crea una lista vuota di oggetti Dipendente
        var dipendenti = new List<Dipendente>();

        while (reader.Read())
        {
            // Crea un oggetto Mansione
            var mansione = new Mansione(reader.GetString(5), reader.GetDouble(6));

            // Crea un oggetto Statistiche
            var statistiche = new Statistiche(
                reader.IsDBNull(7) ? 0 : reader.GetDouble(7),  // Fatturato
                reader.IsDBNull(8) ? 0 : reader.GetInt32(8)     // Presenze
            );

            // Crea un nuovo oggetto Dipendente utilizzando il nuovo costruttore con l'ID
            var dipendente = new Dipendente(
                reader.GetInt32(0),  // ID
                reader.GetString(1), // Nome
                reader.GetString(2), // Cognome
                reader.GetString(3), // Data di Nascita
                reader.GetString(4), // Mail
                mansione,            // Mansione
                statistiche          // Statistiche
            );

            dipendenti.Add(dipendente);
        }

        return dipendenti;
    }

    // metodo CercaDipendentePerMail  permette di cercare il dipendente tramite mail e mostra nel terminale i dettagli correlati una volta trovato

    public Dipendente CercaDipendentePerMail(string email)
    {
        var command = new SQLiteCommand(
            @"SELECT dipendente.id, 
                 dipendente.nome, 
                 dipendente.cognome, 
                 strftime('%d/%m/%Y', dataDiNascita) AS data_formattata, 
                 dipendente.mail, 
                 mansione.titolo, 
                 mansione.stipendio, 
                 indicatori.fatturato, 
                 indicatori.presenze
          FROM dipendente
          JOIN mansione ON dipendente.mansioneId = mansione.id
          LEFT JOIN indicatori ON dipendente.indicatoriId = indicatori.id 
          WHERE dipendente.mail = @mail;",
            _connection);

        command.Parameters.AddWithValue("@mail", email);
        var reader = command.ExecuteReader();

        if (reader.Read())
        {
            // Creazione dell'oggetto Mansione con i campi corretti
            var mansione = new Mansione(reader.GetString(5), reader.GetDouble(6));

            // Gestione dei campi indicatori con controlli per i valori NULL
            double fatturato = reader.IsDBNull(7) ? 0.0 : reader.GetDouble(7); // Usa GetDouble per i valori reali
            int presenze = reader.IsDBNull(8) ? 0 : reader.GetInt32(8);        // Usa GetInt32 per valori interi

            // Creazione dell'oggetto Statistiche con fatturato e presenze
            var statistiche = new Statistiche(fatturato, presenze);

            // Creazione dell'oggetto Dipendente con ID
            var dipendente = new Dipendente(
                reader.GetInt32(0),  // ID
                reader.GetString(1),  // Nome
                reader.GetString(2),  // Cognome
                reader.GetString(3),  // Data di Nascita
                reader.GetString(4),  // Mail
                mansione,             // Mansione
                statistiche           // Statistiche
            );
            // Restituisce la lista di dipendenti
            return dipendente;
        }

        // Restituisci null se il dipendente non viene trovato
        return null;
    }


    // metodo MostraMansioni permette la lettura dei dati dalla tabella mansione per id,titolo,stipendio 

    public List<Mansione> MostraMansioni()
    {
        var command = new SQLiteCommand("SELECT id, titolo, stipendio FROM mansione;", _connection);
        var reader = command.ExecuteReader(); // Esecuzione del comando e creazione di un oggetto per leggere i risultati
        var mansioni = new List<Mansione>(); // Creazione di una lista per memorizzare i nomi degli utenti
        while (reader.Read())
        {
            // Creazione di un oggetto Mansione e aggiunta alla lista
            var mansione = new Mansione(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(2)); // Aggiunta dati utente alla lista
            mansioni.Add(mansione);
        }
        return mansioni;
    }

    // il metodo ModificaDipendente permette di modificare un singolo campo del dipendente nel database 
    // Restituisce `true` se la modifica ha successo e `false` se si verifica un errore o nessuna riga viene modificata
    public bool ModificaDipendente(int dipendenteId, string campoDaModificare, string nuovoValore)
    {
        try
        {
            string query = "";  // Variabile per memorizzare la query SQL che verrà eseguita
            object indicatoriId = null;   // Variabile per memorizzare l'ID degli indicatori, se necessario

            // Verifica se stai modificando "fatturato" o "presenze", che appartengono alla tabella `indicatori`
            if (campoDaModificare == "fatturato" || campoDaModificare == "presenze")
            {
                // Prima controlla se il dipendente ha un ID indicatori associato
                var checkCommand = new SQLiteCommand("SELECT indicatoriId FROM dipendente WHERE id = @id", _connection);
                checkCommand.Parameters.AddWithValue("@id", dipendenteId);
                indicatoriId = checkCommand.ExecuteScalar(); // Esegue la query per recuperare l'ID degli indicatori
                                                             // Se il dipendente non ha un indicatoriId associato, restituisce un errore
                if (indicatoriId == null || indicatoriId == DBNull.Value)
                {
                    Console.WriteLine("Errore: Il dipendente non ha un ID indicatori associato.");
                    return false;
                }

                // Aggiorna la tabella `indicatori`
                query = $"UPDATE indicatori SET {campoDaModificare} = @nuovoValore WHERE id = @indicatoriId";
            }
            else
            {
                // Se il campo da modificare non è "fatturato" o "presenze", costruisce la query per aggiornare la tabella `dipendente`
                query = $"UPDATE dipendente SET {campoDaModificare} = @nuovoValore WHERE id = @id";
            }

            // Crea il comando SQLite utilizzando la query costruita

            using (var command = new SQLiteCommand(query, _connection))
            {
                // Aggiunge i parametri alla query
                command.Parameters.AddWithValue("@nuovoValore", nuovoValore);
                command.Parameters.AddWithValue("@id", dipendenteId);

                // Se stiamo aggiornando la tabella `indicatori`, aggiunge anche l'ID degli indicatori come parametro
                if (campoDaModificare == "fatturato" || campoDaModificare == "presenze")
                {
                    command.Parameters.AddWithValue("@indicatoriId", indicatoriId);
                }

                // Esegue l'aggiornamento e restituisce il numero di righe modificate

                int rowsAffected = command.ExecuteNonQuery();

                // Verifica se almeno una riga è stata modificata
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Aggiornamento riuscito.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Nessuna riga modificata.");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            // Se si verifica un errore, viene gestito qui
            Console.WriteLine("Errore durante la modifica del dipendente: " + ex.Message);
            return false;
        }
    }





}

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
// classe che rappresenta la mansione di un dipendente con proprietà Id, Titolo e Stipendio
public class Mansione
{
    public int Id { get; set; }  // Proprietà 'Id' che rappresenta l'identificatore univoco della mansione
    public string Titolo { get; set; } // proprietà relativa al nome della mansione
    public double Stipendio { get; set; }    // proprietà relativa allo stipendio

    // Costruttore che accetta id titolo e stipendio
    // Questo costruttore viene usato quando abbiamo già un 'Id', probabilmente assegnato dal database
    public Mansione(int id, string titolo, double stipendio)
    {
        Id = id;
        Titolo = titolo;        // Assegna il titolo della mansione
        Stipendio = stipendio;  // Assegna lo stipendio della mansione
    }
    // Nuovo costruttore che accetta solo titolo e stipendio
    // Questo costruttore può essere usato quando l'Id non è necessario al momento della creazione
    public Mansione(string titolo, double stipendio)
    {
        Titolo = titolo;
        Stipendio = stipendio;
    }
}

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

// classe Statistiche con proprietà relative al fatturato e alle presenze del dipendente
public class Statistiche{
// Proprietà 'Id' per identificare in modo univoco ogni record di statistiche
    public int Id{ get; set; }

    // Proprietà per memorizzare il fatturato generato da un dipendente
    public double Fatturato{ get; set; }

 // Proprietà per memorizzare il numero di presenze di un dipendente
    public int Presenze{ get; set; }

// costruttore classe Statistiche
// Riceve i valori iniziali per 'Fatturato' e 'Presenze' e li assegna alle rispettive proprietà
    public Statistiche(double fatturato,int presenze){
        
        Fatturato = fatturato; // Assegna il valore di fatturato alla proprietà Fatturato
        Presenze = presenze;
    }
}

```

</details>

# COMANDI UTILI

- //libreria di Spectre
- dotnet add package Spectre.Console
- // dipendenza SQLite
- dotnet add package System.Data.SQLite
- dotnet add package Microsoft.EntityFrameworkCore
- dotnet add package Microsoft.EntityFrameworkCore.Sqlite
- // dipendenza per poter usare il comando dotnet ef database update
- dotnet add package Microsoft.EntityFrameworkCore.Design
- dotnet add package Microsoft.EntityFrameworkCore.Tools
- // installare ef globalmente
- dotnet tool install --global dotnet-ef

- migrazione del database
- dotnet ef migrations add InitialCreate
- aggiornamento del database
- dotnet ef database update