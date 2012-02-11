using System;
using System.Collections.Generic;

namespace EvolutionHighwayApp.Models
{
    public class Delimiter
    {
        public string Label { get; set; }
        public char Char { get; set; }

        public override string ToString()
        {
            return Label;
        }

        public override bool Equals(object obj)
        {
            if (obj == this) return true;
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (Delimiter) obj;
            return Label.Equals(other.Label) && Char.Equals(other.Char);
        }

        public override int GetHashCode()
        {
            return (Label != null ? Label.GetHashCode() : 0) * 31 + Char.GetHashCode();
        }
    }

    public class Delimiters : List<Delimiter>
    {
        public Delimiters() : base(new[] 
                                    {
                                        new Delimiter { Char = '\t', Label = "Tab" },
                                        new Delimiter { Char = ',',  Label = "Comma" },
                                        new Delimiter { Char = '|',  Label = "Vertical Line" }
                                    })
        {
        }
    }
}
