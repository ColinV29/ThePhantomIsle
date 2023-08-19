using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public Sequence() { }

    public override Status Process()
    {
        Status childStatus = children[currentChild].Process();
        if (childStatus == Status.RUNNING) return Status.RUNNING;
        if (childStatus == Status.FAILURE)
        {
            currentChild = 0;
            return Status.FAILURE;
        }

        if (childStatus == Status.SUCCESS) currentChild++;
        if (currentChild >= children.Count)
        {
            currentChild = 0;
            return Status.SUCCESS;
        }
        return Status.RUNNING;
    }
}
