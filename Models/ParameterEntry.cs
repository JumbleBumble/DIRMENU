﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIRMENU.Models
{
    public class ParameterEntry
    {
        private string label;
        public string? Label { get { return label; } set { this.label = label ?? string.Empty; } }

        private string value;
        public string? Value
        {
            get { return value; }
            set
            {
                this.value = value ?? string.Empty;
                if (label != null && paramFloats.ContainsKey(label))
                {
                    if (float.TryParse(this.value, out float floatValue))
                    {
                        paramFloats[label] = floatValue;
                    }
                }
                else if (label != null && paramBools.ContainsKey(label))
                {
                    if (int.TryParse(this.value, out int boolIntValue))
                    {
                        paramBools[label] = boolIntValue;
                    }
                }
            }
        }

        private Dictionary<string, float> paramFloats;
        private Dictionary<string, int> paramBools;

        public ParameterEntry(string label, string value, Dictionary<string, float> paramFloats, Dictionary<string, int> paramBools)
        {
            this.label = label;
            this.value = value;
            this.paramFloats = paramFloats;
            this.paramBools = paramBools;
        }
    }
}
