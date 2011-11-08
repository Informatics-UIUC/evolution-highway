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
