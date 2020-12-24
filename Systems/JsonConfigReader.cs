namespace BatchReplacer.Systems
{
	using System.IO;
	using Newtonsoft.Json;
    using Data;
	
	public class JsonConfigReader : IConfigReader
	{
        private const string FILE_PATH = "batch-replacer.json";

        public Config Read() => JsonConvert.DeserializeObject<Config>(File.ReadAllText(FILE_PATH));
	}
}