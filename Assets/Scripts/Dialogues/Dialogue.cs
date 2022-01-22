﻿using System.Collections.Generic;
using UnityEngine;

namespace Dialogues
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogues/Dialogue", order = 0)]
    public class Dialogue : DialogueSequenceBase
    {
        [SerializeField] private string text;

        public string Text => text;
        
        public override List<Dialogue> Dialogues()
        {
            return new List<Dialogue>{this};
        }
    }
}