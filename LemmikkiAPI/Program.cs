using LemmikkiTietokanta;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var db = new LemmikitDB();

// POST: Lisää omistaja.
app.MapPost("/owners", (OwnerRequest owner) =>
{
    int ownerId = db.AddOwner(owner.Name, owner.Phone);
    return Results.Created($"/owners/{ownerId}", new { OwnerId = ownerId, Message = "Omistaja lisätty onnistuneesti" });
});

// POST: Lisää lemmikki.
app.MapPost("/pet", (PetRequest pet) =>
{
    db.AddPet(pet.Name, pet.Species, pet.OwnerId);
    return Results.Ok("Lemmikki lisätty onnistuneesti");

});

// GET: Hae omistajan puhelin lemmikin nimen perusteella.
app.MapGet("/pet/owner", (string petName) =>
{
    int? phone = db.FindPhone(petName);

    if (phone != null)
    {
        return Results.Ok(new { PetName = petName, Phone = phone });
    }
    else
    {
        return Results.NotFound($"Lemmikkiä {petName} ei löytynyt");
    }
});

app.Run();

public record OwnerRequest(string Name, int Phone);
public record PetRequest(string Name, string Species, int OwnerId);