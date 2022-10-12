﻿using Database;
using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetStartDupes
{
    class HoldMyString : KMonoBehaviour
    {
        [Serialize]
        public SkillGroup Group = null;
        public string NAME()
        {
            if (Group == null)
                return "";
            else
            {
                return Strings.Get("STRINGS.DUPLICANTS.SKILLGROUPS." + Group.Id.ToUpper() + ".NAME");
            }
        }
        public string RelevantAttribute()
        {
            if (Group == null) return "";
            return Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + Group.relevantAttributes.First().Id.ToUpper() + ".NAME");
        }

        public Trait CurrentTrait = null;


    }
}