using System;
using System.Linq;
using SQLite4Unity3d;

/// <summary>
/// In-memory snapshot of the data you want to reflect to UI across rooms.
/// Add more properties here later (money, gacha coins, etc.).
/// </summary>
public class GameDataCache
{
    public event Action OnChanged;

    // Common references
    public int GrowingPetId { get; private set; }
    public int StateId { get; private set; }

    // State (0-100)
    public int Moisture { get; private set; }
    public int Nutrients { get; private set; }
    public int Sunlight { get; private set; }
    public int Pest { get; private set; }
    public int Health { get; private set; }
    public int Stress { get; private set; }
    public int Decay { get; private set; }

    // Example: money (rename the SQL column to match your schema)
    public int SeedCoin { get; private set; }

    public void ReloadAll(SQLiteConnection conn)
    {
        // userInfo: growing pet id + money
        var user = conn.Query<UserInfoLite>(
            "SELECT user_growing_pet, user_seed_coin FROM userInfo_table ORDER BY user_id LIMIT 1"
        ).FirstOrDefault();

        GrowingPetId = user?.user_growing_pet ?? 0;
        SeedCoin     = user?.user_seed_coin ?? 0;

        if (GrowingPetId <= 0)
        {
            StateId = 0;
            SetStateEmpty();
            RaiseChanged();
            return;
        }

        // pet -> state_id
        var pet = conn.Query<PetLite>(
            "SELECT pet_state FROM pet_table WHERE pet_id = ? LIMIT 1", GrowingPetId
        ).FirstOrDefault();

        StateId = pet?.pet_state ?? 0;

        if (StateId <= 0)
        {
            SetStateEmpty();
            RaiseChanged();
            return;
        }

        // state values
        var s = conn.Query<StateRow>(
            "SELECT state_moisture, state_nutrients, state_sunlight, state_pest, state_health, state_stress, state_decay " +
            "FROM state_table WHERE state_id = ? LIMIT 1", StateId
        ).FirstOrDefault();

        if (s == null)
        {
            SetStateEmpty();
            RaiseChanged();
            return;
        }

        Moisture  = s.state_moisture;
        Nutrients = s.state_nutrients;
        Sunlight  = s.state_sunlight;
        Pest      = s.state_pest;
        Health    = s.state_health;
        Stress    = s.state_stress;
        Decay     = s.state_decay;

        RaiseChanged();
    }

    private void SetStateEmpty()
    {
        Moisture = Nutrients = Sunlight = Pest = Health = Stress = Decay = 0;
    }

    private void RaiseChanged() => OnChanged?.Invoke();

    // ===== minimal row models =====
    private class UserInfoLite
    {
        public int user_growing_pet { get; set; }

        // TODO: rename to your actual column
        public int user_seed_coin { get; set; }
    }

    private class PetLite { public int pet_state { get; set; } }

    private class StateRow
    {
        public int state_moisture { get; set; }
        public int state_nutrients { get; set; }
        public int state_sunlight { get; set; }
        public int state_pest { get; set; }
        public int state_health { get; set; }
        public int state_stress { get; set; }
        public int state_decay { get; set; }
    }
}
