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