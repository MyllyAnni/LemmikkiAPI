using Microsoft.Data.Sqlite;
namespace LemmikkiTietokanta;

public class LemmikitDB
{
    private static string _connectionString = "Data Source = LemmikitDB.db";

    public LemmikitDB()
    {
        // luo lemmikki tietokannan.
        // Muodostetaan yhteys tietokantaan.
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            // Luodoaan taulut, jos niitä ei ole vielä olemassa. 
            // Taulu: Owners id, name, phone.
            // Taulu Pets id, name, species, owners_id.

            var createOwnersTableCmd = connection.CreateCommand();
            createOwnersTableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Owners (
            id INTEGER PRIMARY KEY,
            name TEXT,
            phone INTEGER
            )";

            // Suoritetaan taulun luominen.
            createOwnersTableCmd.ExecuteNonQuery();

            var createPetsTableCmd = connection.CreateCommand();
            createPetsTableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Pets (
            id INTEGER PRIMARY KEY,
            pet_name TEXT,
            species TEXT,
            owners_id INTEGER,
            FOREIGN KEY (owners_id) REFERENCES Owners(id)
            )";

            // Suoritetaan taulun luominen.
            createPetsTableCmd.ExecuteNonQuery();

        }
    }

    // Lisää kantaan Omistajia (id, nimi, puhelin).
    public int AddOwner(string name, int phone)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            var insertOwnerCmd = connection.CreateCommand();
            insertOwnerCmd.CommandText = @"
                INSERT INTO Owners (name, phone)
                VALUES ($name, $phone)";
            insertOwnerCmd.Parameters.AddWithValue("$name", name);
            insertOwnerCmd.Parameters.AddWithValue("$phone", phone);
            insertOwnerCmd.ExecuteNonQuery();

            // Haetaan äskettäin lisätyn omistajan id.
            var lastOwnerIdCmd = connection.CreateCommand();
            lastOwnerIdCmd.CommandText = "SELECT last_insert_rowid()";
            var ownerId = Convert.ToInt32(lastOwnerIdCmd.ExecuteScalar());
            return ownerId;
        }
    }

    // Lisää kantaan lemmikkejä(id, nimi, laji, omistajan_id).
    public void AddPet(string name, string species, int ownerId)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            var insertPetCmd = connection.CreateCommand();
            insertPetCmd.CommandText = @"
                INSERT INTO Pets (pet_name, species, owners_id)
                VALUES ($petName, $species, $ownerId)";
            insertPetCmd.Parameters.AddWithValue("$petName", name);
            insertPetCmd.Parameters.AddWithValue("$species", species);
            insertPetCmd.Parameters.AddWithValue("$ownerId", ownerId);
            insertPetCmd.ExecuteNonQuery();

        }
    }

    // Päivittää omistajan puhelinnumeron.
    public void UpdatePhone(int ownerId, int newPhone)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            var updatePhoneCmd = connection.CreateCommand();
            updatePhoneCmd.CommandText = @"
                UPDATE Owners
                SET phone = $newPhone
                WHERE id = $ownerId";
            updatePhoneCmd.Parameters.AddWithValue("$newPhone", newPhone);
            updatePhoneCmd.Parameters.AddWithValue("$ownerId", ownerId);
            updatePhoneCmd.ExecuteNonQuery();
        }
    }

    // Etsii lemmikin nimen perusteella omistajan puhelinnumeron.
    public int? FindPhone(string petName)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            var findPhoneCmd = connection.CreateCommand();
            findPhoneCmd.CommandText = @"
                SELECT Owners.phone
                FROM Owners
                JOIN Pets ON Owners.id = Pets.owners_id
                WHERE Pets.pet_name = $petName";
            findPhoneCmd.Parameters.AddWithValue("$petName", petName);

            var phone = findPhoneCmd.ExecuteScalar();

            if (phone == null)
            {
                return null;
            }

            return Convert.ToInt32(phone); // Muuntaa object int:ksi.
        }
    }
}