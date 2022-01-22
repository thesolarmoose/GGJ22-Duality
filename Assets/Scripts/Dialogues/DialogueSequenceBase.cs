﻿using System.Collections.Generic;
using UnityEngine;

namespace Dialogues
{
    public abstract class DialogueSequenceBase : ScriptableObject
    {
        public abstract List<Dialogue> Dialogues();
    }
}