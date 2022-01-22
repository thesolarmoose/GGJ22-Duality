﻿using System.Collections.Generic;
using UnityEngine;

namespace Dialogues
{
    [CreateAssetMenu(fileName = "DialogueSequenceRandom", menuName = "Dialogues/DialogueSequenceRandom", order = 0)]
    public class DialogueSequenceRandom : DialogueSequenceBase
    {
        [SerializeField] private List<DialogueSequenceBase> dialogueSequences;

        public override List<Dialogue> Dialogues()
        {
            int length = dialogueSequences.Count;
            int randomIndex = Random.Range(0, length);
            var dialogueSequence = dialogueSequences[randomIndex];
            return dialogueSequence.Dialogues();
        }
    }
}