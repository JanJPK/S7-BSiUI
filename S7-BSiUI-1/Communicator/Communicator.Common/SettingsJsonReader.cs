using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Communicator.Common
{
    /// <summary>
    ///     Basic key-value config extraction.
    /// </summary>
    public class SettingsJsonReader
    {
        protected readonly List<KeyValuePair<string, string>> Pairs;

        public SettingsJsonReader(string path)
        {
            var text = File.ReadAllText(path);
            Pairs = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(text).ToList();

            if (!Pairs.Any())
            {
                throw new ArgumentException("Json contains no values.");
            }
        }

        public string GetValue(string key)
        {
            return Pairs
                  .SingleOrDefault(p => p.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                  .Value;
        }
    }
}