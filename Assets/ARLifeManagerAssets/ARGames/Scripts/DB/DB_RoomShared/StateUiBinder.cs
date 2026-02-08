using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum StateKey
{
    Moisture,
    Nutrients,
    Sunlight,
    Pest,
    Health,
    Stress,
    Decay,
}

[Serializable]
public class StateTextBinding
{
    public StateKey key;
    public TMP_Text text;
}

/// <summary>
/// Attach this to the room UI object that displays state.
/// Bind TMP_Text fields via inspector.
/// It updates on:
/// - OnEnable (room becomes active)
/// - Cache OnChanged events (when data is reloaded)
/// </summary>
public class StateUiBinder : MonoBehaviour, IDbUiBinder
{
    [SerializeField] private List<StateTextBinding> bindings = new();

    private void OnEnable()
    {
        if (GameDbContext.IsReady)
            Apply(GameDbContext.Cache);
        else
            Clear();

        GameDbContext.Cache.OnChanged += HandleChanged;
    }

    private void OnDisable()
    {
        GameDbContext.Cache.OnChanged -= HandleChanged;
    }

    private void HandleChanged()
    {
        if (!isActiveAndEnabled) return;
        Apply(GameDbContext.Cache);
    }

    public void Apply(GameDataCache c)
    {
        foreach (var b in bindings)
        {
            if (b.text == null) continue;

            int v = b.key switch
            {
                StateKey.Moisture => c.Moisture,
                StateKey.Nutrients => c.Nutrients,
                StateKey.Sunlight => c.Sunlight,
                StateKey.Pest => c.Pest,
                StateKey.Health => c.Health,
                StateKey.Stress => c.Stress,
                StateKey.Decay => c.Decay,
                _ => 0
            };

            b.text.text = $"{v}%";
        }
    }

    public void Clear()
    {
        foreach (var b in bindings)
            if (b.text != null) b.text.text = "--";
    }
}
