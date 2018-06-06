using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cards.Core
{
    public class Temp
    {

        [JsonProperty]
        public string hello;

        [JsonIgnore]
        public string ignored;

        public int value;

        public List<string> strings;

        public Temp()
        {

        }

        // if you match the name of the property with 
        // the name of the properties here, they 
        // will be passed via this constructor 
        [JsonConstructor]
        public Temp(string hello, int value)
        {

        }


    }
}
