using System.Collections.Generic;

/// <summary>
/// AbilityInstance is a class that handles an instance of an attack or ability instance that's passed to the target
/// Target then looks over the information for any relevant points
/// For example, if a unit has advantage to rolls against an ability with the Ice property
/// the target can then check it this way and apply any relevant bonuses
/// </summary>

public abstract class AbilityInstance
{
    protected CharacterManager source; // source unit
    protected CharacterManager target; // target unit
    protected MasterControl masterControl; // Reference to master (if needed)

    protected Rank rank; // Rank of the attack/ability/effect/etc
    protected Element element; // Damage type, mostly for spells

    // Source and target unit tags
    protected List<string> sourceUnitTags;
    protected List<string> targetUnitTags;

    // Return functions
    public Rank ReturnRank() { return rank; }
    public Element ReturnElement() { return element; }
    public CharacterManager ReturnSource() { return source; }
    public CharacterManager ReturnTarget() { return target; }
    public MasterControl ReturnMC() { return masterControl; }
    public List<string> ReturnSourceTags() { return sourceUnitTags; }
    public List<string> ReturnTargetTags() { return targetUnitTags; }
}