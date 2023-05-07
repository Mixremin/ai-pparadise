using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemStackManager : MonoBehaviour
{
    [SerializeField] private List<ItemStack> stacks = new List<ItemStack>();

    public IReadOnlyList<ItemStack> AllStacks => stacks;

    public int AllItemsCount()
    {
        return stacks.Sum(stack => stack.Items.Count);
    }

    public List<Transform> ObjectList()
    {
        List<Transform> list = new List<Transform>();

        for (int z = 0; z < stacks.Count; z++)
            list.AddRange(stacks[z].Items);

        return list;
    }

/*    public bool FindNPCByState(NPCStates _state, out ItemStack _stack, out int _index, out Transform _npc)
    {
        _stack = null;
        _index = -1;
        _npc = null;

        foreach (ItemStack stack in AllStacks)
            if (stack.Items.Count > 0)
                for (int z = 0; z < stack.Items.Count; z++)
                    if (stack.Items[z].TryGetComponent (out NPCMainScript nms))
                        if (nms.CurrentState == _state)
                        {
                            _stack = stack;
                            _index = z;
                            _npc = stack.Items[z];
                            return true;
                        }

        return false;
    }

    public ItemStack GetRandomStack(bool includeEmpty)
    {
        var selectedStacks = includeEmpty ? stacks : stacks.Where(s => s.Items.Count != 0);
        var randomIndex = Random.Range(0, selectedStacks.Count());
        return selectedStacks.ElementAt(randomIndex);
    }*/

    public ItemStack GetLowestStack()
    {
        var lowestStack = stacks[0];
        for (var i = 1; i < stacks.Count; i++)
        {
            if (stacks[i].Items.Count < lowestStack.Items.Count)
            {
                lowestStack = stacks[i];
            }
        }

        return lowestStack;
    }
}