using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spell_NPC_SummonZombies is a script that handles the Summon Zombies spell.
/// Upon being cast, this spell summons a handful of basic zombie minions.
/// This spell is only available to NPC units.
/// </summary>
[CreateAssetMenu(menuName = "Spell/NPC Spell/Summon Zombies")]
public class Spell_NPC_SummonZombies : Spell
{
    [SerializeField]
    private GameObject ZombieMinion; // Zombie prefab
    private int MinionAmount; // Amount of minions we can summon
    // List of nodes around the caster
    private List<GameObject> nodes = new List<GameObject>();

    public override void Activate(CharacterManager source, List<CharacterManager> targetList)
    {
        RankRead(); // Update to the current Rank
        // Check if the casting unit has enough to cast the spell
        if (source.skillController.SpendMana(MPCost))
        {
            nodes.Clear(); // Clear the nodes list
            // Cast an Overlapsphere around the caster, looking for all "objects tagged with Node"
            Collider[] colliders = Physics.OverlapSphere(source.transform.position, aoeRange);
            foreach(Collider collider in colliders)
            {
                if (string.Equals(collider.tag, "Node")) { nodes.Add(collider.gameObject); }
            }

            // Find legal locations to summon
            FindSummonLocations(source);
        }

        // Can't cast the spell
        else { Debug.Log("Failed to cast the spell..."); }
    }

    /// <summary>
    /// FindSummonLocations looks for random locations to summon our minions on.
    /// The locations must be legal, that is, they're walkable nodes and unoccupied by
    /// another unit.
    /// </summary>
    /// <param name="source"></param>
    private void FindSummonLocations(CharacterManager source)
    {
        // Find all illegal nodes
        for (int f = 0; f < nodes.Count; f++)
        {
            // Grab the node function
            Node node = nodes[f].GetComponent<Node>();
            // If a node is not walkable OR selectable (i.e. already occupied)
            if (!node.walkable && !node.selectable)
            {
                // Remove it - reduce f by one to go back a step in the loop
                nodes.RemoveAt(f);
                f--;
            }
        }

        int num = nodes.Count; // Find the # of nodes we're left with
        List<int> usedNumbers = new List<int>(); // List of random indices
        // For loop, generate an amount of random numbers equal to the # of minions we want to summon
        for (int i = 0; i < MinionAmount; i++)
        {
            // We check to make sure we don't select a number multiple times
            int randomNode = Random.Range(0, num);
            while (usedNumbers.Contains(randomNode)) { randomNode = Random.Range(0, num); }
            // If legal, add them to the usedNumbers list
            usedNumbers.Add(randomNode);
        }

        //With a list of legal nodes, summon the zombies
        SummonZombies(usedNumbers, source);
    }
    
    /// <summary>
    /// SummonZombies() receives a list of indices indicating an empty node.
    /// On each, place a zombie minion on it.
    /// </summary>
    /// <param name="usedNumbers"> List of int variables representing indices </param>
    /// <param name="source"> Unit casting the spell </param>
    private void SummonZombies(List<int> usedNumbers, CharacterManager source)
    {
        // Grab the turn manager script
        TurnManager turnManager = source.master.turnManager;
        // Create a quaternion for the zombies to face
        Quaternion quaternion = new Quaternion(0, 0, 0, 1);

        // For each number in the list of numbers
        foreach (int num in usedNumbers)
        {
            // Grab the position of the nodem and instantiate the zombie on it.
            // We also use the quaternion we made here.
            Vector3 nodePosition = new Vector3(nodes[num].transform.position.x, 0.8825f, nodes[num].transform.position.z);
            GameObject newZombie = Instantiate(ZombieMinion, nodePosition, quaternion);

            // We grab the new unit's charMan script and hand it to the turn manager's new unit function
            CharacterManager zombieManager = newZombie.GetComponent<CharacterManager>();
            turnManager.AddNewUnit(zombieManager, source);
        }
    }

    // Rank read finds the MP cost and the amount of minions summoned by this spell
    public override void RankRead()
    {
        aoeRange = 7f;

        switch (rank)
        {
            case Rank.E:
                MPCost = 1;
                MinionAmount = 1;
                break;
            case Rank.D:
                MPCost = 2;
                MinionAmount = 2;
                break;
            case Rank.C:
                MPCost = 3;
                MinionAmount = 3;
                break;
            case Rank.B:
                MPCost = 4;
                MinionAmount = 4;
                break;
            case Rank.A:
                MPCost = 5;
                MinionAmount = 5;
                break;
            case Rank.EX:
                MPCost = 5;
                MinionAmount = 6;
                break;
            default: // Somehow have no rank? All equals 0
                MPCost = 0;
                MinionAmount = 0;
                break;
        }
    }
}
