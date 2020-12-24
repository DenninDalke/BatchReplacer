namespace BatchReplacer.Data
{
	using System.Collections.Generic;
	
	public record ReplaceInfo(string Search, string Replace);
	public record Config(IEnumerable<string> Paths, string Filter, ReplaceInfo FileName, IEnumerable<ReplaceInfo> Contents);
}