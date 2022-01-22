﻿using System.Collections.Generic;
using UnityEngine;

namespace Dialogues
{
    [CreateAssetMenu(fileName = "DialogueSequence", menuName = "Dialogues/DialogueSequence", order = 0)]
    public class DialogueSequence : DialogueSequenceBase
    {
        [SerializeField] private List<Dialogue> dialogues;

        public override List<Dialogue> Dialogues()
        {
            return dialogues;
        }
    }
}